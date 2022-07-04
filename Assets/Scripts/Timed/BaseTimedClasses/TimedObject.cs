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
        private const bool IS_DEBUGGING = false;

        [SerializeField] private bool m_shouldRecord = true;

        private ITimedBehaviour[] m_timedBehavs = null;
        // The spawn time of this object
        private float m_spawnTime = float.MaxValue;
        // Farthest time we had gotten in the recording.
        private float m_farthestTime = float.MinValue;
        // Time from previous UpdateToTime call
        private float m_prevTime = 0.0f;
        private GlobalTimeManager m_timeMan = null;

        public bool isRecording { get; private set; } = true;
        public bool wasRecording { get; private set; } = true;
        public bool shouldRecord { get => m_shouldRecord; set => m_shouldRecord = value; }
        public float curTime => m_timeMan != null ? m_timeMan.curTime : -1.0f;
        public float spawnTime => m_spawnTime;
        public float farthestTime => m_farthestTime != float.MinValue ?
            m_farthestTime : 0.0f;


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

            m_spawnTime = m_timeMan.curTime;
            m_farthestTime = m_spawnTime;
            m_prevTime = m_spawnTime;

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
            float deltaTime = time - m_prevTime;
            // We only record data for new times that are
            // farther than the any previous time.
            isRecording = time > m_farthestTime && shouldRecord;
            if (isRecording)
            {
                m_farthestTime = time;

                // Call timed update
                foreach (ITimedBehaviour behav in m_timedBehavs)
                {
                    behav.TimedUpdate(deltaTime);
                }
            }
            // If we stopped recording
            else if (prevRecording)
            {
                foreach (ITimedBehaviour behav in m_timedBehavs)
                {
                    behav.OnRecordingStop(m_farthestTime);
                }
            }

            // We want to disable the gameobject and not call updateToTime
            // if either we are before it should spawn or we are after the
            // furthest time and we shouldn't be recording.
            bool isTimeBeforeSpawn = time < m_spawnTime;
            bool isTimeAfterRecording = time > m_farthestTime;
            bool shouldBeInactive = isTimeBeforeSpawn || (isTimeAfterRecording && !shouldRecord);
            bool shouldBeActive = !shouldBeInactive;
            #region Logs
            CustomDebug.LogForComponent(
                $"{nameof(isTimeBeforeSpawn)}={isTimeBeforeSpawn}; " +
                $"{nameof(isTimeAfterRecording)}={isTimeAfterRecording}; " +
                $"{nameof(shouldBeInactive)}={shouldBeInactive}",
                this, IS_DEBUGGING);
            #endregion Logs
            gameObject.SetActive(shouldBeActive);
            if (shouldBeActive)
            {
                foreach (ITimedBehaviour behav in m_timedBehavs)
                {
                    behav.UpdateToTime(time);
                }
            }            

            wasRecording = isRecording;
            m_prevTime = time;
        }
    }
}