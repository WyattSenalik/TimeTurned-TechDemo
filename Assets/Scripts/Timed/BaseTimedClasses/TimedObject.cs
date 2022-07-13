using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Implementation of <see cref="ITimedObject"/> that allows
    /// <see cref="ITimedComponent"/>s, <see cref="ITimedObserver"/>s, and 
    /// <see cref="ITimedRecorder"/>s to be attached to this gameobject.
    /// </summary>
    [DisallowMultipleComponent]
    public class TimedObject : MonoBehaviour, ITimedObject
    {
        private const bool IS_DEBUGGING = false;

        public bool isRecording { get; private set; } = true;
        public bool wasRecording { get; private set; } = true;
        public bool shouldRecord { get => m_shouldRecord; set => m_shouldRecord = value; }
        public float curTime => m_timeMan != null ? m_timeMan.curTime : -1.0f;
        public float spawnTime => m_spawnTime;
        public float farthestTime => m_farthestTime != float.MinValue ?
            m_farthestTime : 0.0f;

        [SerializeField] private bool m_shouldRecord = true;
        [Tooltip("If the timed object should always record when shouldRecord is " +
            "true. Will cause data to be overwritten.")]
        [SerializeField] private bool m_alwaysRecordWhenShould = false;

        private ITimedObserver[] m_timedObservers = null;
        private ITimedRecorder[] m_timedRecorders = null;
        // The spawn time of this object
        private float m_spawnTime = float.MaxValue;
        // Farthest time we had gotten in the recording.
        private float m_farthestTime = float.MinValue;
        // Time from previous UpdateToTime call
        private float m_prevTime = 0.0f;
        private GlobalTimeManager m_timeMan = null;


        // Domestic Initialization
        private void Awake()
        {
            m_timedObservers = GetComponents<ITimedObserver>();
            m_timedRecorders = GetComponents<ITimedRecorder>();
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


        public void SetToTime(float time)
        {
            // We want to disable the gameobject and not call updateToTime
            // if either we are before it should spawn or we are after the
            // furthest time and we shouldn't be recording.
            bool t_shouldBeActive = DetermineShouldBeActive(time);
            gameObject.SetActive(t_shouldBeActive);
            // Do nothing if inactive.
            if (!gameObject.activeSelf) { return; }

            // Update is recording.
            wasRecording = isRecording;
            isRecording = DetermineIfIsRecording(time);

            // Update recorders and observers.
            UpdateObservers(time);
            UpdateRecorders(time);

            m_prevTime = time;
        }


        /// <summary>
        /// Calls functions of <see cref="ITimedRecorder"/> when appropriate.
        /// </summary>
        /// <param name="time">Current global time.</param>
        private void UpdateRecorders(float time)
        {
            // Which functions of ITimedBehaviour should be called
            bool t_doCallTrimDataAfter = false;
            bool t_doCallOnRecordingResume = false;
            bool t_doCallOnRecordingStop = false;

            // We record data if shouldRecord is true and either:
            // 1. m_alwaysRecordWhenShould is true OR
            // 2. time > m_farthestTime
            if (isRecording)
            {
                // If the time is before the farthest time, trim the data
                if (time <= m_farthestTime)
                {
                    // Assumed that if this is true, so is m_alwaysRecordWhenShould.
                    #region Asserts
                    CustomDebug.AssertIsTrueForComponent(m_alwaysRecordWhenShould,
                        $"Record time should only ever be before the furthest " +
                        $"time if {nameof(m_alwaysRecordWhenShould)} is true " +
                        $"but it is false.", this);
                    #endregion Asserts
                    t_doCallTrimDataAfter = true;
                }
                m_farthestTime = time;

                // If we resumed the recording.
                t_doCallOnRecordingResume = !wasRecording;
            }
            // If we stopped recording
            else if (wasRecording)
            {
                t_doCallOnRecordingStop = true;
            }

            // Call the ITimedRecorder functions
            foreach (ITimedRecorder t_behav in m_timedRecorders)
            {
                if (t_doCallTrimDataAfter) { t_behav.TrimDataAfter(time); }
                if (t_doCallOnRecordingResume) { t_behav.OnRecordingResume(time); }
                if (t_doCallOnRecordingStop) { t_behav.OnRecordingStop(time); }
                // Always call set to time (unless inactive)
                t_behav.SetToTime(time);
            }

            wasRecording = isRecording;
            m_prevTime = time;
        }
        /// <summary>
        /// Calls timed update on all observers if recording.
        /// </summary>
        private void UpdateObservers(float time)
        {
            if (isRecording)
            {
                float t_deltaTime = time - m_prevTime;
                foreach (ITimedObserver t_obs in m_timedObservers)
                {
                    t_obs.TimedUpdate(t_deltaTime);
                }
            }
        }
        /// <summary>
        /// We record data if shouldRecord is true and either:
        /// 1. m_alwaysRecordWhenShould is true OR
        /// 2. time > m_farthestTime
        /// </summary>
        private bool DetermineIfIsRecording(float time)
        {
            // Should always record when we should.
            if (m_alwaysRecordWhenShould)
            {
                return shouldRecord;
            }
            // We only record data for new times that are
            // farther than the any previous time.
            else
            {
                return time > m_farthestTime && shouldRecord;
            }
        }
        /// <summary>
        /// We want to be inactive if either we are before it should spawn or we 
        /// are after the furthest time and we shouldn't be recording.
        /// </summary>
        private bool DetermineShouldBeActive(float time)
        {
            bool t_isTimeBeforeSpawn = time < m_spawnTime;
            bool t_isTimeAfterRecording = time > m_farthestTime;
            bool t_shouldBeInactive = t_isTimeBeforeSpawn ||
                (t_isTimeAfterRecording && !shouldRecord);
            #region Logs
            CustomDebug.LogForComponent(
                $"{nameof(t_isTimeBeforeSpawn)}={t_isTimeBeforeSpawn}; " +
                $"{nameof(t_isTimeAfterRecording)}={t_isTimeAfterRecording}; " +
                $"{nameof(t_shouldBeInactive)}={t_shouldBeInactive}",
                this, IS_DEBUGGING);
            #endregion Logs
            return !t_shouldBeInactive;
        }
    }
}