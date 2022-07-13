using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Scrapbook that holds snapshots in chronological order.
    /// </summary>
    /// <typeparam name="TSnap">Snapshot class.</typeparam>
    /// <typeparam name="TSnapType">Type of data the snapshot class has.</typeparam>
    public class SnapshotScrapbook<TSnap, TSnapType> :
        ISnapshotScrapbook<TSnap, TSnapType>
        where TSnap : ISnapshot<TSnapType, TSnap>
    {
        private const bool IS_DEBUGGING = false;

        public int Count => m_snapshots != null ? m_snapshots.Count : 0;

        // List of snapshots. Should always be sorted in increasing time order.
        private readonly List<TSnap> m_snapshots = null;


        public SnapshotScrapbook()
        {
            m_snapshots = new List<TSnap>();
        }
        public SnapshotScrapbook(params TSnap[] snaps) : this()
        {
            // Add the given snapshots to the binder
            foreach (TSnap s in snaps)
            {
                AddSnapshot(s);
            }
        }


        /// <summary>
        /// Adds the given snapshot to the end of the scrapbook.
        /// Must be after the latest snapshot.
        /// </summary>
        public void AddSnapshot(TSnap snapshot)
        {
            #region Asserts
            if (m_snapshots.Count > 0)
            {
                TSnap t_latestSnap = m_snapshots[m_snapshots.Count - 1];
                CustomDebug.AssertIsTrueForObj(t_latestSnap.time < snapshot.time,
                    $"the given snapshot (with time {snapshot.time}) to " +
                    $"{nameof(AddSnapshot)} to be at a time later than " +
                    $"the all other snapshots ({t_latestSnap.time})", this);
            }
            #endregion Asserts
            m_snapshots.Add(snapshot);
        }
        /// <summary>
        /// Gets the snapshot at the current time.
        /// If there is no snapshot at the given time (most likely), 
        /// will return an interpolated snapshot using the snapshots before 
        /// and after the given time.
        /// </summary>
        /// <param name="time">Time to get a snapshot for.</param>
        public TSnap GetSnapshot(float time)
        {
            int t_index = FindClosestSnapshot(time);
            // There are no snapshots.
            if (t_index < 0)
            {
                CustomDebug.LogWarning($"{nameof(GetSnapshot)} called when " +
                    $"there are no snapshots yet.");
                return default;
            }
            #region Logs
            CustomDebug.LogForObject($"Closest snapshot to time {time} had time " +
                $"{m_snapshots[t_index].time} at index {t_index}", this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            CustomDebug.AssertIndexIsInRange(t_index, m_snapshots,
                GetType().Name);
            #endregion Asserts
            // Closest snap is always less than the given time.
            TSnap t_closestSnap = m_snapshots[t_index];
            // This means the other snap is just the snap after that one.
            int t_otherIndex = t_index + 1;
            // If the other index is out of bounds, it should just be the closest.
            TSnap t_otherSnap = t_closestSnap;
            if (t_otherIndex < m_snapshots.Count)
            {
                t_otherSnap = m_snapshots[t_otherIndex];
            }

            return t_closestSnap.Interpolate(t_otherSnap, time);
        }
        /// <summary>
        /// Gets the time of the snapshot that takes place latest in time.
        /// 
        /// Returns -1 if there are not any snapshots yet.
        /// </summary>
        public float GetLatestTime()
        {
            if (m_snapshots.Count <= 0) { return -1.0f; }
            return m_snapshots[m_snapshots.Count - 1].time;
        }
        public int RemoveSnapshotsAfter(float time)
        {
            // Get the index directly before the time.
            int t_index = FindClosestSnapshot(time);
            // There are no snapshots.
            if (t_index < 0) { return 0; }

            int t_amountRemoved = 0;
            while (t_index + 1 < Count)
            {
                // Remove last snapshot
                m_snapshots.RemoveAt(Count - 1);
                ++t_amountRemoved;
            }
            // Now the scrapbook contains snapshots up to (and including)
            // the snapshot at the found index. Its possible that the snapshot
            // is for exactly the sought time.
            TSnap t_snap = m_snapshots[Count - 1];
            if (t_snap.time == time)
            {
                m_snapshots.RemoveAt(Count - 1);
                ++t_amountRemoved;
            }
            else
            {
                // Be extra careful and check that our assumption is not wrong.
                #region Asserts
                CustomDebug.AssertIsTrueForObj(t_snap.time <= time, 
                    $"found snapshot {t_snap} at time {t_snap.time} to be " +
                    $"before the sought time of {time}.", this);
                #endregion Asserts
            }

            return t_amountRemoved;
        }



        /// <summary>
        /// Finds the index of the closest snapshot that is less than the given 
        /// time. 
        /// 
        /// Pre Conditons - Assumes <see cref="m_snapshots"/> is sorted in 
        /// increasing time order.
        /// Post Conditions - Returns the index of the snapshot with the nearest 
        /// time to the given time. If the list is empty, returns -1.
        /// </summary>
        private int FindClosestSnapshot(float time)
        {
            // Return -1 if there are no snapshots yet.
            if (m_snapshots.Count <= 0) return -1;

            // Use binary search to find the snapshot closest to the given time.
            int t_min = 0;
            int t_max = m_snapshots.Count - 1;
            int t_mid = -1;
            while (t_min <= t_max)
            {
                t_mid = (t_min + t_max) / 2;
                TSnap t_curVal = m_snapshots[t_mid];
                if (time < t_curVal.time)
                {
                    // Go left because list is sorted in DECREASING order.
                    t_max = t_mid - 1;
                }
                else if (time > t_curVal.time)
                {
                    // Go right because list is sorted in DECREASING order.
                    t_min = t_mid + 1;
                }
                else
                {
                    // Happens to be the exact time we are looking for
                    return t_mid;
                }
            }
            // Since we will most likely not find a snapshot at exactly the given
            // time, we use the last mid index to determine the closest snapshot.

            // It is possible the final snapshot is actually greater than the time
            // we want, but the index right below it should be the correct one
            // if that is the case.
            TSnap t_finalVal = m_snapshots[t_mid];
            // If the final time is greater, than we want the value to the left.
            // That value is then the closest value under the given time.
            if (t_finalVal.time > time)
            {
                return Mathf.Max(0, t_mid - 1);
            }
            // Final val is already lower.
            return t_mid;
        }


        public override string ToString()
        {
            string t_str = GetType().Name + " {";
            foreach (TSnap snap in m_snapshots)
            {
                t_str += $"({snap}); ";
            }
            t_str = t_str.Substring(0, t_str.Length - 2);
            t_str += "}";
            return t_str;
        }
    }
}
