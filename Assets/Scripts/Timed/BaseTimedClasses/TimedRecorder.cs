using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    [RequireComponent(typeof(ITimedObject))]
    public abstract class TimedRecorder<TSnap, TSnapType> : 
        TimedComponent, ITimedRecorder where TSnap : ISnapshot<TSnapType, TSnap>
    {
        private const bool IS_DEBUGGING = true;

        // When the last snapshot was taken.
        public float lastSnapTime { get; private set; } = float.MinValue;

        #region Debug Fields
        [ShowNonSerializedField] private int m_amountSnapshots = 0;
        #endregion Debug Fields

        [SerializeField] private bool m_takeSnapOnRecordingResume = false;
        [SerializeField] private bool m_takeSnapOnRecordingStop = true;

        // Scrapbook that holds the transform snapshots.
        private ISnapshotScrapbook<TSnap, TSnapType> m_scrapbook = null;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_scrapbook = new SnapshotScrapbook<TSnap, TSnapType>();
        }


        #region ITimedRecorder
        public virtual void TrimDataAfter(float time)
        {
            m_scrapbook.RemoveSnapshotsAfter(time);
        }
        public virtual void OnRecordingResume(float time)
        {
            if (m_takeSnapOnRecordingResume)
            {
                // When the recording starts, take a snapshot.
                TakeSnapshot(time);
            }
        }
        public virtual void OnRecordingStop(float time)
        {
            if (m_takeSnapOnRecordingStop)
            {
                // When the recording stops, take a snapshot.
                TakeSnapshot(time);
            }
        }
        public virtual void SetToTime(float time)
        {
            // If we are not recording, just update the transform to the time.
            if (!isRecording)
            {
                TSnap t_snap = m_scrapbook.GetSnapshot(time);
                ApplySnapshotData(t_snap.data);
                return;
            }

            // If no snapshots have been taken (first update), take one
            if (m_scrapbook.Count < 1)
            {
                TakeSnapshot(time);
                return;
            }
            // If we should take a new snapshot because something
            // has changed since last time.
            if (ShouldTakeNewSnapshot(time))
            {
                TakeSnapshot(time);
                return;
            }
        }
        #endregion ITimedRecorder


        /// <summary>
        /// Called when a snapshot needs to be taken of the current state.
        /// 
        /// Should create and return a new snapshot for the given (current) time.
        /// </summary>
        protected abstract TSnap CreateNewSnapshot(float time);
        /// <summary>
        /// Called when not currently recording and instead is trying to replay.
        /// 
        /// Should apply the given snapshot data.
        /// </summary>
        protected abstract void ApplySnapshotData(TSnapType snapData);
        /// <summary>
        /// Called when trying to determine if the state of what is being recorded
        /// has changed enough to warrant taking a new snapshot.
        /// </summary>
        protected abstract bool ShouldTakeNewSnapshot(float time);


        /// <summary>
        /// Tries to take a new snapshot for the given (current) time.
        /// </summary>
        /// <param name="time"></param>
        private void TakeSnapshot(float time)
        {
            // Don't retake any snapshots
            if (time == lastSnapTime)
            {
                #region Logs
                CustomDebug.LogForComponent($"Avoided retaking snapshot at " +
                    $"time {time}", this, IS_DEBUGGING);
                #endregion Logs
                return;
            }

            #region Logs
            CustomDebug.LogForComponent($"Creating snapshot at time {time} ",
                this, IS_DEBUGGING);
            #endregion Logs
            lastSnapTime = time;

            TSnap t_snap = CreateNewSnapshot(time);
            m_scrapbook.AddSnapshot(t_snap);
            m_amountSnapshots = m_scrapbook.Count;
        }
    }
}
