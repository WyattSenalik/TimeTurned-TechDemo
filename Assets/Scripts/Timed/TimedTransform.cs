using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Controls the transform this is attached to in order to allow for rewinding
    /// and going back in time.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VelocityMonitor))]
    public class TimedTransform : TimedBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] private bool m_shouldTrackRotation = false;
        [Tooltip("How often (in seconds) until the next snapshot is taken. " +
            "Needed only for tracking rotation. Position and scale will take" +
            "snapshots. An object that rotates quickly should have a small " +
            "frequency. An object that rotates slowly can have a larger " +
            "frequency.")]
        [SerializeField, Min(0f), ShowIf(nameof(m_shouldTrackRotation))]
        private float m_snapshotFreq = 0.1f;


        [ShowNonSerializedField]
        private int m_amountSnapshots = 0;

        private VelocityMonitor m_velMon = null;
        // Scrapbook that holds the transform snapshots.
        private ISnapshotScrapbook<TransformDataSnapshot, TransformData>
            m_scrapbook = new SnapshotScrapbook<TransformDataSnapshot, TransformData>();
        // When the last snapshot was taken.
        private float m_lastSnapTime = float.MinValue;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_velMon = GetComponent<VelocityMonitor>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_velMon, this);
            #endregion Asserts
        }


        public override void UpdateToTime(float time)
        {
            // If we are not recording, just update the transform to the time.
            if (!isRecording)
            {
                ChangeTransformToTime(time);
                return;
            }

            // If no snapshots have been taken (first update), take one
            if (m_scrapbook.Count < 1)
            {
                TakeSnapshot(time);
                return;
            }
            // If velocity changed since last time, we need to take more snapshots.
            if (m_velMon.DidPositionVelocityChange() ||
                m_velMon.DidScaleVelocityChange())
            {
                #region Logs
                CustomDebug.LogForComponent($"Velocity changed. Taking " +
                    $"snapshots at time {time}", this, IS_DEBUGGING);
                #endregion Logs
                TakeSnapshot(time);
                return;
            }
            // If we are tracking rotation and rotation is/was changing,
            // then update based on frequency
            if (m_shouldTrackRotation && m_velMon.IsOrWasRotationChanging() &&
                time >= m_lastSnapTime + m_snapshotFreq)
            {
                #region Logs
                CustomDebug.LogForComponent($"Frequency reached. Taking " +
                    $"snapshots at time {time}", this, IS_DEBUGGING);
                #endregion Logs
                TakeSnapshot(time);
                return;
            }
        }
        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            // When the recording stops, take a snapshot.
            TakeSnapshot(time);
        }


        /// <summary>
        /// Gets the closest snapshot and applies it to the transform.
        /// </summary>
        private void ChangeTransformToTime(float time)
        {
            TransformDataSnapshot snap = m_scrapbook.GetSnapshot(time);
            TransformData data = snap.data;
            #region Logs
            CustomDebug.LogForComponent($"Applying transform data ({data}) at " +
                $" time {time}.", this, IS_DEBUGGING);
            #endregion Logs
            data.ApplyGlobal(transform);
        }
        /// <summary>
        /// Takes a snapshot of the transform's current values
        /// and saves it for the given time.
        /// </summary>
        /// <param name="time"></param>
        private void TakeSnapshot(float time)
        {
            // Don't retake any snapshots
            if (time == m_lastSnapTime) { return; }

            #region Logs
            CustomDebug.LogForComponent($"Creating snapshot at time {time} ",
                this, IS_DEBUGGING);
            #endregion Logs
            m_lastSnapTime = time;

            TransformDataSnapshot snap = new TransformDataSnapshot(time, transform);
            m_scrapbook.AddSnapshot(snap);
            m_amountSnapshots = m_scrapbook.Count;
        }
    }
}