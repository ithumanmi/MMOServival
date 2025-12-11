using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core.Utilities
{
    public class Timer
    {
        readonly Action m_Callback;
        float m_Time, m_CurrentTime;
        public float normalizedProgress
        {
            get { return Mathf.Clamp(m_CurrentTime / m_Time, 0f, 1f); }
        }
        public Timer(float newTime, Action onElapsed = null)
        {
            SetTime(newTime);
            m_CurrentTime = 0f;
            m_Callback += onElapsed;
        }
        public virtual bool Tick(float deltaTime)
        {
            return AssessTime(deltaTime);
        }
        protected bool AssessTime(float deltaTime)
        {
            m_CurrentTime += deltaTime;
            if (m_CurrentTime >= m_Time)
            {
                FireEvent();
                return true;
            }

            return false;
        }
        public void Reset()
        {
            m_CurrentTime = 0;
        }
        public void FireEvent()
        {
            m_Callback.Invoke();
        }
        public void SetTime(float newTime)
        {
            m_Time = newTime;

            if (newTime <= 0)
            {
                m_Time = 0.1f;
            }
        }
    }


}
