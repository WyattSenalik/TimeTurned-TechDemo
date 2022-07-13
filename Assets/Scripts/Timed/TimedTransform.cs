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
    public class TimedTransform : 
        TimedRecorder<TransformDataSnapshot, TransformData>
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField] private bool m_shouldTrackRotation = false;
        [Tooltip("How often (in seconds) until the next snapshot is taken. " +
            "Needed only for tracking rotation. Position and scale will take" +
            "snapshots. An object that rotates quickly should have a small " +
            "frequency. An object that rotates slowly can have a larger " +
            "frequency.")]
        [SerializeField, Min(0f), ShowIf(nameof(m_shouldTrackRotation))]
        private float m_snapshotFreq = 0.1f;

        private VelocityMonitor m_velMon = null;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_velMon = GetComponent<VelocityMonitor>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_velMon, this);
            #endregion Asserts
        }


        protected override TransformDataSnapshot CreateNewSnapshot(float time)
        {
            return new TransformDataSnapshot(time, transform);
        }
        protected override void ApplySnapshotData(TransformData snapData)
        {
            #region Logs
            CustomDebug.LogForComponent($"Applying transform data ({snapData}) " +
                $"at time {curTime}.", this, IS_DEBUGGING);
            #endregion Logs
            snapData.ApplyGlobal(transform);
        }
        protected override bool ShouldTakeNewSnapshot(float time)
        {
            // If velocity changed since last time, we need to take more snapshots.
            if (m_velMon.DidPositionVelocityChange() ||
                m_velMon.DidScaleVelocityChange())
            {
                CustomDebug.LogForComponent($"A velocity changed", this, 
                    IS_DEBUGGING);
                return true;
            }
            // If we are tracking rotation and rotation is/was changing,
            // then update based on frequency
            if (m_shouldTrackRotation && m_velMon.IsOrWasRotationChanging() &&
                time >= lastSnapTime + m_snapshotFreq)
            {
                #region Logs
                CustomDebug.LogForComponent($"Frequency reached. Taking " +
                    $"snapshots at time {time}", this, IS_DEBUGGING);
                #endregion Logs
                return true;
            }

            return false;
        }
    }
}