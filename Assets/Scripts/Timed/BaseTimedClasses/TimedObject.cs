using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Implementation of <see cref="ITimedObject"/> that allows
    /// <see cref="ITimedComponent"/>s and <see cref="ITimedBehaviour"/>s
    /// to be attached to this gameobject.
    /// </summary>
    [DisallowMultipleComponent]
    public class TimedObject : MonoBehaviour, ITimedObject
    {
        private ITimedBehaviour[] m_timedBehavs = null;
        // Farthest time we had gotten in the recording.
        private float m_farthestTime = -1.0f;
        // Time from previous UpdateToTime call
        private float m_prevTime = 0.0f;
        private GlobalTimeManager m_timeMan = null;

        public bool isRecording { get; private set; } = true;
        public bool wasRecording { get; private set; } = true;


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
            // Remove this from its global time manager when it is destroyed.
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

                // Call timed update
                float deltaTime = time - m_prevTime;
                foreach (ITimedBehaviour behav in m_timedBehavs)
                {
                    behav.TimedUpdate(deltaTime);
                }
                m_prevTime = time;
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

            wasRecording = isRecording;
        }
    }
}