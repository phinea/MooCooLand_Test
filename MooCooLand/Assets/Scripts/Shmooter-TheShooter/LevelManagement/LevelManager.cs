using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MooCooEngine.Game
{
    public class LevelManager : MonoBehaviour {
        private static LevelManager _instance;
        public static LevelManager Instance { get { return _instance; } }

        public int currentLevelIndex;
        public int currentScore;

        public Level[] LevelNames;
        public TextMesh UI_Countdown;
        public TextMesh UI_LevelName;
        public TextMesh UI_Score;   // TODO: Extra class that handles displaying UI score


        private Countdown countDown;

        //private DateTime timeLevelStarted;
        //private double countdownInSec;
        //private double countdownMaxInSec;

        //# Move to user class?
        public Dictionary<int, int> LevelIndexAndScorePerUser = new Dictionary<int, int>();

        // Use this for initialization
        void Start() {
            if (_instance == null)
                _instance = this;

            StartLevel();
        }

        void ShowLevelName()
        {
            if ((UI_LevelName != null) && (CurrentLevel != null))
                UI_LevelName.text = CurrentLevel.LevelName;
        }

        #region Score -- TODO: ScoreManager???
        public void Score_Assign(int score)
        {
            currentScore = score;

            if (UI_Score != null)
                UI_Score.text = "" + currentScore;
        }

        public void Score_Increment(int addToScore)
        {
            currentScore += addToScore;
            
            if (UI_Score != null)
                UI_Score.text = ""+ currentScore;
        }
        #endregion


        //TODO: Should be an individual level class
        public void ShowCountdown()
        {
            if ((UI_Countdown != null)&&(countDown!=null)&&(countDown.IsActive))
            {
                //countdownInSec = countdownMaxInSec - (DateTime.UtcNow - timeLevelStarted).TotalSeconds;
                UI_Countdown.text = ""+countDown.TimeInFullSec;
            }
        }


        // Update is called once per frame
        void Update() {
            ShowCountdown();
        }

        #region THIS SHOULD BE INDIVIDUALLY HANDLED BY THE RESPECTIVE GAMES
        // TODO: THIS SHOULD BE INDIVIDUALLY HANDLED BY THE RESPECTIVE GAMES
        public void StartLevel()
        {
            currentScore = 0;
            Debug.Log("Start new level: " + currentLevelIndex);
            if ((LevelNames != null) && (currentLevelIndex < LevelNames.Length))
            {
                Level lvl = LevelNames[currentLevelIndex];
                lvl.StartLevel();

                if (lvl.isTimed)
                {
                    countDown = new Countdown();
                    countDown.Start(lvl.timerMaxInSec);
                    countDown.OnCountDownFinished += CountDown_OnCountDownFinished;
                    //timeLevelStarted = DateTime.UtcNow;  //TODO: Create class for handling countdown
                    //countdownMaxInSec = lvl.timerMaxInSec;
                }
                ShowLevelName();
            }
        }

        public Level CurrentLevel
        {
            get
            {
                if ((LevelNames != null) && (currentLevelIndex < LevelNames.Length))
                {
                    return LevelNames[currentLevelIndex];
                }
                return null;
            }
        }

        public int minPointsToProceedToNextLevel = -1; // TODO: Should be with the individual levels???

        private void CountDown_OnCountDownFinished(object sender, CountDownEventArgs e)
        {
            UI_Countdown.text = "TIME IS OUT";

            if (currentScore > minPointsToProceedToNextLevel)
                ProceedToNextLevel();
            else
            {
                Debug.Log("Wooom, woom... Next time more luck!");
                GameManager.Instance.GoTo_Highscore();
            }
        }

        public void ProceedToNextLevel()
        {
            Level lvl = LevelNames[currentLevelIndex];
            lvl.FinishLevel();

            if (NextLevelIsAvailable())
            {
                Debug.Log("SUCCESS! Proceed to next level! ");
                currentLevelIndex++;
                StartLevel();
            }
            else
            {
                Debug.Log("SUCCESS! No more levels! Game over!!! ");
                if (GameManager.Instance == null)
                {
                    Debug.Log("GAMEMANAGER IS NULL!!!");
                }else
                    GameManager.Instance.GoTo_Highscore();
            }
        }

        private bool NextLevelIsAvailable()
        {
            if (currentLevelIndex + 1 < LevelNames.Length)
                return true;
            else
                return false;
        }

        public void CancelLevel()
        {
            Debug.Log("User-initiated cancellation!");
            // TODO: ADD SOME UI TO ASK WHETHER USER IS SURE...
            GameManager.Instance.GoTo_MainMenu();
        }

        public void FinishLevel_Success()
        {
            Debug.Log("FINISHED level! Yay!");
            // TODO: ADD SOME UI TO SHOW SCORE; ASK TO RETRY; ...
            new WaitForSeconds(2f);

            ProceedToNextLevel();
        }

        public void FinishLevel_Fail()
        {
            Debug.Log("Wooom, woom... Next time more luck!");
            // TODO: ADD SOME UI TO SHOW SCORE; ASK TO RETRY; ...
            new WaitForSeconds(2f);

            GameManager.Instance.GoTo_Highscore();
        }
        #endregion
    }
}