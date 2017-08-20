using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace MooCooEngine.Utils
{
    /// <summary>
    /// A dispatcher to run actions asynchronous or to be posted to UI thread.
    /// Refer to an implementation from
    /// http://answers.unity3d.com/questions/305882/how-do-i-invoke-functions-on-the-main-thread.html
    /// </summary>
    public class DispatcherBehavior : MonoBehaviour
    {
        private const int MaxThreads = 8;

        private int numThreads;
        private bool _hasLoaded = false;

        // TODO<ningx>: replace these list or queues with ConcurrentXXX
        private List<Action> _actions = new List<Action>();
        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();
        private List<Action> _currentActions = new List<Action>();

        private struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }

        protected void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        protected void QueueOnMainThread(Action action, float time)
        {
            if (time != 0)
            {
                lock (_delayed)
                {
                    _delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (_actions)
                {
                    _actions.Add(action);
                }
            }
        }

#if WINDOWS_UWP
        protected System.Threading.Tasks.Task RunAsync(Action a)
        {
            return System.Threading.Tasks.Task.Run(()=> 
            {
                RunAction(a);
            });
        }
#else
        protected Thread RunAsync(Action a)
        {
            while (numThreads >= MaxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }
#endif
        private void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }
        }

        protected virtual void Start()
        {
            _hasLoaded = true;
        }

        protected virtual void Update()
        {
            if (_hasLoaded == false)
            {
                Start();
            }

            lock (_actions)
            {
                _currentActions.Clear();
                if(_actions.Count > 0)
                {
                    _currentActions.AddRange(_actions);
                    _actions.Clear();
                }
            }

            foreach (var a in _currentActions)
            {
                a();
            }

            lock (_delayed)
            {
                _currentDelayed.Clear();
                if(_delayed.Count > 0)
                {
                    _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                    foreach (var item in _currentDelayed)
                    {
                        _delayed.Remove(item);
                    }
                }
            }

            foreach (var delayed in _currentDelayed)
            {
                delayed.action();
            }
        }

        protected virtual void OnDestroy()
        {

        }
    }
}
