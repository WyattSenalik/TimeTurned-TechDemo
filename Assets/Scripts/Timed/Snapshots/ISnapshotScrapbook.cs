// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Collection of snapshots.
    /// </summary>
    /// <typeparam name="TSnap">Snapshot type.</typeparam>
    /// <typeparam name="TSnapType">Type of the snapshot's data.</typeparam>
    public interface ISnapshotScrapbook<TSnap, TSnapType>
        where TSnap : ISnapshot<TSnapType, TSnap>
    {
        int Count { get; }
        int Length => Count;
        int Size => Count;

        /// <summary>
        /// Adds the given snapshot to the scrapbook.
        /// </summary>
        void AddSnapshot(TSnap snapshot);
        /// <summary>
        /// Gets an interpolated snapshot at the given time.
        /// Uses other snapshot data to determine what this time should be.
        /// </summary>
        /// <param name="time">Time to get a snapshot for.</param>
        TSnap GetSnapshot(float time);
        /// <summary>
        /// Gets the time of the latest (in time) snapshot added.
        /// </summary>
        float GetLatestTime();
        /// <summary>
        /// Removes all snapshots after (and including) the given time.
        /// </summary>
        /// <returns>The amount of snapshots removed from the scrapbook.</returns>
        int RemoveSnapshotsAfter(float time);
    }
}
