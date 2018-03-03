using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        float maxSeconds;


        public delegate void Func();

        public event Func OnTimerStart;
        public event Func OnTimerStop;

        public float Seconds { get { return seconds; } }
        public float MaxSeconds { get { return maxSeconds; } }
        public bool IsStarted { get { return isStarted; } }
        public bool IsStopped { get { return isStarted && seconds <= 0.0f; } }
        public bool IsPaused { get { return isPaused; } }


        float seconds;

        bool isStarted;
        bool isPaused;


        void Update()
        {
            if (!isStarted || isPaused) {
                return;
            }

            seconds -= 1.0f * Time.deltaTime;

            if (IsStopped) {
                Stop();
                _FireEvent_OnTimerStop();
            }
        }

        void Destroy()
        {
            OnTimerStart = null;
            OnTimerStop = null;
        }

        void _FireEvent_OnTimerStart()
        {
            if (OnTimerStart != null) {
                OnTimerStart();
            }
        }

        void _FireEvent_OnTimerStop()
        {
            if (OnTimerStop != null) {
                OnTimerStop();
            }
        }

        public void CountDown()
        {
            seconds = maxSeconds;
            isStarted = true;
            _FireEvent_OnTimerStart();
        }

        public void Pause(bool value)
        {
            isPaused = value;
        }

        public void Stop()
        {
            seconds = 0.0f;
            isStarted = false;
        }
    }
}
