// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Snapshot that stores data and the time the data was stored.
    /// </summary>
    /// <typeparam name="T">Data that is being stored in the snapshot.</typeparam>
    /// <typeparam name="TSelf">Type of the snapshot.</typeparam>
    public interface ISnapshot<T, TSelf> where TSelf : ISnapshot<T, TSelf>
    {
        /// <summary>
        /// Timestamp the snapshot was taken.
        /// </summary>
        float time { get; }
        /// <summary>
        /// Data stored in the snapshot.
        /// </summary>
        T data { get; }

        /// <summary>
        /// Interpolates two snapshots and returns the result of the
        /// interpolation.
        /// </summary>
        /// <param name="other">The other snapshot to interpolate for.</param>
        /// <param name="targetTime">Target time to interpolate to.</param>
        /// <returns></returns>
        TSelf Interpolate(TSelf other, float targetTime);
    }

    public static class ISnapshotExtensions
    {
        public static TSelf Lerp<T, TSelf>(this TSelf a, TSelf b, float t)
            where TSelf : ISnapshot<T, TSelf>
        {
            float targetTime = (a.time + b.time) * t;
            return a.Interpolate(b, targetTime);
        }
    }
}
