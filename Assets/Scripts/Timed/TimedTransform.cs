using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public class TimedTransform : TimedBehaviour
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField, Min(0f)] private float m_snapshotFreq = 0.1f;

        private ISnapshotScrapbook<TransformDataSnapshot, TransformData>
            m_scrapbook = new SnapshotScrapbook<TransformDataSnapshot, TransformData>();
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


        private void ChangeTransformToTime(float time)
        {
            TransformDataSnapshot snap = m_scrapbook.GetSnapshot(time);
            TransformData data = snap.data;
            //#region Logs
            //CustomDebug.LogForComponent($"Applying transform data ({data}) at " +
            //    $"time {time}.", this, IS_DEBUGGING);
            //#endregion Logs
            data.ApplyGlobal(transform);
        }
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