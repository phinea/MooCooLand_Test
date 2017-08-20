using System;
using System.Collections.Generic;
using UnityEngine;

namespace MooCooEngine
{
    public enum CountdownState
    {
        NotStarted,
        IsCounting, 
        IsPaused,
        IsFinished
    }


    public class Countdown 
    {
        private List<DateTime> startTimes; // In case of interruptions
        private List<DateTime> stopTimes;
        private CountdownState countdownState = CountdownState.NotStarted;
        private double maxTimeInSec;
        /// <summary>
        /// Counts down from a given value.
        /// </summary>
        /// <param name="maxTime">This is the delta time to count down from.</param>
        public Countdown()
        {
        }
        
        public void Start(double deltaTimeToCountDownFrom)
        {
            startTimes = new List<DateTime>();
            startTimes.Add(DateTime.UtcNow);
            stopTimes = new List<DateTime>();

            countdownState = CountdownState.IsCounting;
            maxTimeInSec = deltaTimeToCountDownFrom;
            UpdateOnCountDownStarted(TimeInSec);
        }

        //public void Update()
        //{
        //    if (TimeInSec > maxTimeInSec)
        //    {
        //        Debug.Log(">> COUNTDOWN: " + TimeInSec);
        //        UpdateOnCountDownFinished(TimeInSec);
        //    }
        //}

        public void Stop()
        {
            if (countdownState != CountdownState.IsFinished)
            {
                stopTimes.Add(DateTime.UtcNow);
                countdownState = CountdownState.IsFinished;
                UpdateOnCountDownFinished(TimeInSec);
            }
        }

        public void Pause()
        {
            if (countdownState == CountdownState.IsCounting)
            {
                stopTimes.Add(DateTime.UtcNow);
                countdownState = CountdownState.IsPaused;
                UpdateOnCountDownPaused(TimeInSec);
            }
        }

        public void Continue()
        {
            if (countdownState == CountdownState.IsPaused)
            {
                startTimes.Add(DateTime.UtcNow);
                countdownState = CountdownState.IsCounting;
                UpdateOnCountDownContinued(TimeInSec);
            }
        }

        #region Properties to access the countdown
        public double TimeInSec
        {
            get
            {
                switch (countdownState)
                {
                    case CountdownState.NotStarted: return -1;
                    case CountdownState.IsCounting: 
                    case CountdownState.IsFinished: 
                    case CountdownState.IsPaused: return SummedUpCountdown;
                }

                return -2;
            }
        }

        public bool IsActive
        {
            get
            {
                return (countdownState != CountdownState.NotStarted);
            }
        }

        /// <summary>
        /// Returns the rounded number of Countdown.TimeInSec. 
        /// </summary>
        public double TimeInFullSec
        {
            get
            {
                return Math.Round(TimeInSec);
            }
        }

        private double SummedUpCountdown
        {
            get
            {
                double summedCntdwn = 0;
                for (int i = 0; i < stopTimes.Count; i++)
                {
                    summedCntdwn += (stopTimes[i] - startTimes[i]).TotalSeconds;
                }

                if(startTimes.Count == stopTimes.Count + 1) // Meaning: It's still in counting and thus, we're missing one end time
                    summedCntdwn += (DateTime.UtcNow - startTimes[stopTimes.Count]).TotalSeconds;

                summedCntdwn = maxTimeInSec - summedCntdwn;

                if (summedCntdwn <= 0)
                    Stop();
                return summedCntdwn;
            }
        }
        #endregion

        #region Custom countdown events        
        public delegate void CountdownUpdateHandler(object sender, CountDownEventArgs e);
        public event CountdownUpdateHandler OnCountDownStarted;
        public event CountdownUpdateHandler OnCountDownPaused;
        public event CountdownUpdateHandler OnCountDownContinued;
        public event CountdownUpdateHandler OnCountDownFinished;

        private void UpdateOnCountDownStarted(double remainingTimeInSec)
        {
            // Make sure someone is listening to event
            if (OnCountDownStarted != null)
                OnCountDownStarted(this, new CountDownEventArgs(remainingTimeInSec));
        }

        private void UpdateOnCountDownPaused(double remainingTimeInSec)
        {
            // Make sure someone is listening to event
            if (OnCountDownPaused != null)
                OnCountDownPaused(this, new CountDownEventArgs(remainingTimeInSec));
        }

        private void UpdateOnCountDownContinued(double remainingTimeInSec)
        {
            // Make sure someone is listening to event
            if (OnCountDownContinued != null)
                OnCountDownContinued(this, new CountDownEventArgs(remainingTimeInSec));
        }

        private void UpdateOnCountDownFinished(double remainingTimeInSec)
        {
            // Make sure someone is listening to event
            if (OnCountDownFinished != null)
                OnCountDownFinished(this, new CountDownEventArgs(remainingTimeInSec));
        }
        #endregion

    }

    public class CountDownEventArgs : System.EventArgs
    {
        public double RemainingTime { get; private set; }

        public CountDownEventArgs(double timeInSec)
        {
            RemainingTime = timeInSec;
        }
    }
}