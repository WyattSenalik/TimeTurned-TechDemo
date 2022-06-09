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
                TSnap latestSnap = m_snapshots[m_snapshots.Count - 1];
                CustomDebug.AssertIsTrueForObj(latestSnap.time < snapshot.time,
                    $"the given snapshot (with time {snapshot.time}) to " +
                    $"{nameof(AddSnapshot)} to be at a time later than " +
                    $"the all other snapshots ({latestSnap.time})", this);
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
            int index = FindClosestSnapshot(time);
            // There are no snapshots.
            if (index < 0)
            {
                CustomDebug.LogWarning($"{nameof(GetSnapshot)} called when " +
                    $"there are no snapshots yet.");
                return default;
            }
            #region Logs
            CustomDebug.LogForObject($"Closest snapshot to time {time} had time " +
                $"{m_snapshots[index].time} at index {index}", this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            CustomDebug.AssertIndexIsInRange(index, m_snapshots,
                GetType().Name);
            #endregion Asserts
            // Closest snap is always less than the given time.
            TSnap closestSnap = m_snapshots[index];
            // This means the other snap is just the snap after that one.
            int otherIndex = index + 1;
            // If the other index is out of bounds, it should just be the closest.
            TSnap otherSnap = closestSnap;
            if (otherIndex < m_snapshots.Count)
            {
                otherSnap = m_snapshots[otherIndex];
            }

            return closestSnap.Interpolate(otherSnap, time);
        }
        /// <summary>
        /// Gets the time of the snapshot that takes place latest in time.
        /// </summary>
        public float GetLatestTime()
        {
            if (m_snapshots.Count <= 0) { return -1.0f; }
            return m_snapshots[m_snapshots.Count - 1].time;
        }



        /// <summary>
        /// Finds the index of the closest snapshot that is less than the given 
        /// time. 
        /// 
        /// Pre Conditons - Assumes <see cref="m_snapshots"/> is sorted in 
        /// increasing time roder.
        /// Post Conditions - Returns the index of the snapshot with the nearest 
        /// time to the given time. If the list is empty, returns -1.
        /// </summary>
        private int FindClosestSnapshot(float time)
        {
            // Return -1 if there are no snapshots yet.
            if (m_snapshots.Count <= 0) return -1;

            // Use binary search to find the snapshot closest to the given time.
            int min = 0;
            int max = m_snapshots.Count - 1;
            int mid = -1;
            while (min <= max)
            {
                mid = (min + max) / 2;
                TSnap curVal = m_snapshots[mid];
                if (time < curVal.time)
                {
                    // Go left because list is sorted in DECREASING order.
                    max = mid - 1;
                }
                else if (time > curVal.time)
                {
                    // Go right because list is sorted in DECREASING order.
                    min = mid + 1;
                }
                else
                {
                    // Happens to be the exact time we are looking for
                    return mid;
                }
            }
            // Since we will most likely not find a snapshot at exactly the given
            // time, we use the last mid index to determine the closest snapshot.

            // It is possible the final snapshot is actually greater than the time
            // we want, but the index right below it should be the correct one
            // if that is the case.
            TSnap finalVal = m_snapshots[mid];
            // If the final time is greater, than we want the value to the left.
            // That value is then the closest value under the given time.
            if (finalVal.time > time)
            {
                return Mathf.Max(0, mid - 1);
            }
            // Final val is already lower.
            return mid;
        }


        public override string ToString()
        {
            string str = GetType().Name + " {";
            foreach (TSnap snap in m_snapshots)
            {
                str += $"({snap}); ";
            }
            str = str.Substring(0, str.Length - 2);
            str += "}";
            return str;
        }
    }
}
