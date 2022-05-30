using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public class TimedObject : MonoBehaviour
    {
        private ITimedBehaviour[] m_timedBehavs = null;
        // Farthest time we had gotten in the recording.
        private float m_farthestTime = -1.0f;
        private GlobalTimeManager m_timeMan = null;

        public bool isRecording { get; private set; } = true;


        // Domestic Initialization
        private void Awake()
        {
            m_timedBehavs = GetComponents<ITimedBehaviour>();
        }
        // Foreign Initialization
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_timeMan, this);
            #endregion Asserts
            m_timeMan.AddTimeObject(this);
        }
        private void OnDestroy()
        {
            if (m_timeMan != null)
            {
                m_timeMan.RemoveTimeObject(this);
            }
        }


        public void UpdateToTime(float time)
        {
            bool prevRecording = isRecording;
            // We only record data for new times.
            isRecording = time > m_farthestTime;
            if (isRecording)
            {
                m_farthestTime = time;
            }
            // If we stopped recording
            else if (prevRecording)
            {
                foreach (ITimedBehaviour behav in m_timedBehavs)
                {
                    behav.OnRecordingStop(m_farthestTime);
                }
            }

            foreach (ITimedBehaviour behav in m_timedBehavs)
            {
                behav.UpdateToTime(time);
            }
        }
    }
}