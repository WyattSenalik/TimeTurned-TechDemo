using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// <see cref="ISnapshot{T, TSelf}"/> for storing <see cref="TransformData"/>.
    /// </summary>
    public class TransformDataSnapshot : ISnapshot<TransformData, TransformDataSnapshot>
    {
        private const bool IS_DEBUGGING = false;

        public float time { get; private set;}
        public TransformData data { get; private set; }


        public TransformDataSnapshot(float timeOfData, TransformData transformData)
        {
            time = timeOfData;
            data = transformData;
        }
        public TransformDataSnapshot(float timeOfData,
            Vector3 worldPos, Quaternion worldRot, Vector3 localScale):
            this(timeOfData, new TransformData(worldPos, worldRot, localScale))
        { }
        public TransformDataSnapshot(float timeOfData, Transform transform):
            this(timeOfData, new TransformData(transform))
        { }

        /// <summary>
        /// Linearly interpolates (Lerps) this snapshot with the given
        /// snapshot to more closesly match the given time.
        /// 
        /// For best results, the two snapshots should be adjacent to each other
        /// and the given time should be between the two snapshots.
        /// </summary>
        /// <param name="other">Snapshot to Lerp with.</param>
        /// <param name="targetTime">Target time in seconds that will be lerped to.
        /// This is NOT the same as t in Vector3.Lerp.</param>
        public TransformDataSnapshot Interpolate(TransformDataSnapshot other,
            float targetTime)
        {
            // Determine which time is the earlier one and which is the later.
            float earlierTime;
            float laterTime;
            if (time < other.time)
            {
                earlierTime = time;
                laterTime = other.time;
            }
            else
            {
                earlierTime = other.time;
                laterTime = time;
            }

            // Interpolation percent. Commonly referred to as t.
            // For interpolation, this value should be in the range [0, 1].
            // For extrapolation, this value should be within the ranges
            // (-infinity, 0) U (1, infinity).
            float denom = (laterTime - earlierTime);
            float t = 0;
            if (denom != 0)
            {
                t = (targetTime - earlierTime) / (laterTime - earlierTime);
            }
            #region Logs
            CustomDebug.Log($"Interpolating snapshot with time {time} and " +
                $"other snapshot with time {other.time} to the target time " +
                $"{targetTime}. The t value found is {t}", IS_DEBUGGING);
            #endregion Logs
            TransformData interpolatedData = TransformData.LerpUnclamped(data,
                other.data, t);
            return new TransformDataSnapshot(targetTime, interpolatedData);
        }


        public override string ToString()
        {
            return data.ToString() + $" ({nameof(time)}={time})";
        }
    }
}