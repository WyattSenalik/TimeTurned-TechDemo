using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Controls the transform this is attached to in order to allow for rewinding
    /// and going back in time.
    /// </summary>
    [DisallowMultipleComponent]
    public class TimedTransform : TimedBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [Tooltip("How often (in seconds) until the next snapshot is taken. " +
            "Transforms that will have complicated changes should be have a " +
            "low frequency. Transforms that will have simple changes can have " +
            "a higher frequency.")]
        [SerializeField, Min(0f)] private float m_snapshotFreq = 0.1f;

        // Scrapbook that holds the transform snapshots.
        private ISnapshotScrapbook<TransformDataSnapshot, TransformData>
            m_scrapbook = new SnapshotScrapbook<TransformDataSnapshot, TransformData>();
        // When the last snapshot was taken.
        private float m_lastSnapTime = float.MinValue;


        public override void UpdateToTime(float time)
        {
            // If we are not recording, just update the transform to the time.
            if (!timedObject.isRecording)
            {
                ChangeTransformToTime(time);
                return;
            }

            // Dont always take snapshots
            if (time < m_lastSnapTime + m_snapshotFreq) { return; }
            TakeSnapshot(time);
        }
        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            // Don't take a new snap if we literally just took one.
            if (time == m_lastSnapTime) { return; }
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
                $"time {time}.", this, IS_DEBUGGING);
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
            #region Logs
            CustomDebug.LogForComponent($"Creating snapshot at time {time} ",
                this, IS_DEBUGGING);
            #endregion Logs
            m_lastSnapTime = time;

            TransformDataSnapshot snap = new TransformDataSnapshot(time,
                TransformData.CreateGlobalTransformData(transform));
            m_scrapbook.AddSnapshot(snap);
        }
    }
}