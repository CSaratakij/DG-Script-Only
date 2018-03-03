using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        float maxSeconds;


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
            }
        }

        public void CountDown()
        {
            seconds = maxSeconds;
            isStarted = true;
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
