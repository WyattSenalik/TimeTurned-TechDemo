using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public class SnapshotScrapbook<TSnap, TSnapType> :
        ISnapshotScrapbook<TSnap, TSnapType>
        where TSnap : ISnapshot<TSnapType, TSnap>
    {
        private const bool IS_DEBUGGING = true;

        private List<TSnap> m_snapshots = null;


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
            /*
            // Find the index of the snapshot that has a time closest to the 
            // given time.
            int index = BinarySearchForClosestSnapshot(snapshot.time);
            // There are no snapshots yet, so just add this one.
            if (index < 0)
            {
                m_snapshots.Add(snapshot);
                return;
            }

            TSnap existingSnap = m_snapshots[index];
            if (existingSnap.time == snapshot.time)
            {
                CustomDebug.LogWarning($"{GetType().Name} snapshot with duplicate " +
                    $"time was trying to be added to the scrapbook.");
                return;
            }

            int insertIndex = index + 1;
            // Insert index is further than the list, so just append the
            // newest snapshot to the end.
            if (insertIndex >= m_snapshots.Count)
            {
                m_snapshots.Add(snapshot);
                return;
            }
            // Insert after that snapshot.
            m_snapshots.Insert(insertIndex, snapshot);
            */
        }
        public TSnap GetSnapshot(float time)
        {
            int index = BinarySearchForClosestSnapshot(time);
            // There are no snapshots.
            if (index < 0)
            {
                CustomDebug.LogWarning($"{nameof(GetSnapshot)} called when " +
                    $"there are no snapshots yet.");
                return default;
            }
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
        public float GetLatestTime()
        {
            if (m_snapshots.Count <= 0) { return -1.0f; }
            return m_snapshots[m_snapshots.Count - 1].time;
        }



        /// <summary>
        /// 
        /// Pre Conditons - Assumes <see cref="m_snapshots"/> is sorted.
        /// Post Conditions - Returns the index of the snapshot with the nearest 
        /// time to the given time. If the list is empty, returns -1.
        /// </summary>
        /// <returns></returns>
        private int BinarySearchForClosestSnapshot(float time)
        {
            // Return -1 if there are no snapshots yet.
            if (m_snapshots.Count <= 0) return -1;

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
                    min = max + 1;
                }
                else
                {
                    // Happens to be the exact time we are looking for
                    return min;
                }
            }

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
        /// <summary>
        /// Helper function for <see cref="BinarySearchForClosestSnapshot"/>
        /// that recursively searches through <see cref="m_snapshots"/> to find
        /// the index of the snapshot with a time exactly equal to or just below 
        /// the given time.
        /// </summary>
        /// <param name="time">Time to search for.</param>
        /// <param name="lowerIndex">Lower index currently being searched.</param>
        /// <param name="higherIndex">Higher index currently being searched.</param>
        private int BinarySearchForClosestSnapshotRecursive(float time, 
            int lowerIndex, int higherIndex)
        {
            // Start in the middle of the two indices.
            int pivotIndex = (lowerIndex + higherIndex) / 2;

            #region Asserts
            CustomDebug.AssertIndexIsInRange(pivotIndex, m_snapshots,
                GetType().Name);
            #endregion Asserts
            TSnap curVal = m_snapshots[pivotIndex];

            // Base condition 0: The lower index is no longer lower than the higher
            // index. This means the current pivot is the closest time to the
            // desired time.
            if (lowerIndex >= higherIndex)
            {
                return DetermineClosestSnapshotIndexAround(pivotIndex, time);
            }


            if (curVal.time < time)
            {
                // Go left.
                return BinarySearchForClosestSnapshotRecursive(time,
                    lowerIndex, pivotIndex - 1);
            }
            else if (curVal.time > time)
            {
                // Go right.
                return BinarySearchForClosestSnapshotRecursive(time,
                    pivotIndex + 1, higherIndex);
            }
            // Base condition 1: The given time exactly equals this time.
            else
            {
                // Happens to be exactly the right time.
                return pivotIndex;
            }
        }
        /// <summary>
        /// Helper function for 
        /// <see cref="BinarySearchForClosestSnapshotRecursive"/>.
        /// Meant to be called when the final pivot has been reached.
        /// 
        /// Determines if the snapshot at the final pivot, left of the final pivot,
        /// or right of the final pivot is closer to the given time and returns it.
        /// Returns -2 if something goes wrong.
        /// </summary>
        /// <param name="index">Center index. The snapshot stored at this 
        /// index and the indices to the right and left of this index will be 
        /// compared and whichever has a snapshot closer to the given time will
        /// be returned.</param>
        /// <param name="time">Time to find closest snapshot to.</param>
        private int DetermineClosestSnapshotIndexAround(int index, float time)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(index, m_snapshots,
                GetType().Name);
            #endregion Asserts
            TSnap curVal = m_snapshots[index];

            // If the current value is great than the 
            if (curVal.time > time)
            {
                return Mathf.Max(0, index - 1);
            }
            return index;

            /*
            // Distances to determine which snapshot is actually closest.
            float distToCur = Mathf.Abs(curVal.time - time);
            float distToRight = float.PositiveInfinity;
            float distToLeft = float.PositiveInfinity;
            // Find distance to right if in range
            int rightIndex = index + 1;
            if (rightIndex < m_snapshots.Count)
            {
                TSnap rightVal = m_snapshots[rightIndex];
                distToRight = Mathf.Abs(rightVal.time - time);
            }
            // Find distance to left if in range
            int leftIndex = index - 1;
            if (leftIndex >= 0)
            {
                TSnap leftVal = m_snapshots[leftIndex];
                distToLeft = Mathf.Abs(leftVal.time - time);
            }

            float minDist = Mathf.Min(distToCur, distToRight, distToLeft);
            if (minDist == distToCur) { return index; }
            if (minDist == distToRight) { return rightIndex; }
            if (minDist == distToLeft) { return leftIndex; }

            #region Asserts
            CustomDebug.ThrowAssertionFailure($"{minDist} did not equal " +
                $"{nameof(distToCur)}({distToCur}), " +
                $"{nameof(distToRight)}({distToRight}), or " +
                $"{nameof(distToLeft)}({distToLeft})", this);
            #endregion Asserts
            return -2;
            */
        }
        /// <summary>
        /// Gets the snapshot on the opposite side of the time.
        /// 
        /// If the original snapshot takes place before given time,
        /// the snapshot after the given snapshot will be returned.
        /// Vise versa, if the original snapshot takes place after the given
        /// time, the snapshot before the snapshot will be returned.
        /// 
        /// Pre Conditions - The given snapshot must be the snapshot
        /// either directly after or directly before the given time.
        /// The given index must be in range.
        /// </summary>
        /// <param name="snapIndex">Index of the snap we want 
        /// the opposite of it.</param>
        private TSnap GetOppositeSnapshot(int snapIndex, float time)
        {
            #region Logs
            CustomDebug.LogForObject($"{nameof(GetOppositeSnapshot)} " +
                $"{ToString()}", this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            CustomDebug.AssertIndexIsInRange(snapIndex, m_snapshots,
                GetType().Name);
            #endregion Asserts
            TSnap snap = m_snapshots[snapIndex];
            if (snap.time < time)
            {
                int rightIndex = snapIndex + 1;
                // If the right index is out of bounds, there is no opposite snap,
                // so just return the snap we are trying to get the opposite of.
                if (rightIndex >= m_snapshots.Count)
                {
                    #region Logs
                    CustomDebug.Log($"{GetType().Name} Tried to get RIGHT snap " +
                        $"with index ({rightIndex}) but DID NOT exist.",
                        IS_DEBUGGING);
                    #endregion Logs
                    return snap;
                }

                TSnap rightSnap = m_snapshots[rightIndex];
                #region Logs
                CustomDebug.Log($"{GetType().Name} Returning snap at RIGHT " +
                    $"index ({rightIndex}) with time {rightSnap.time}",
                    IS_DEBUGGING);
                #endregion Logs
                return rightSnap;
            }
            else
            {
                int leftIndex = snapIndex - 1;
                // If the left index is out of bounds, there is no opposite snap,
                // so just return the snap we are trying to get the opposite of.
                if (leftIndex < 0)
                {
                    #region Logs
                    CustomDebug.Log($"{GetType().Name} Tried to get LEFT snap " +
                        $"with index ({leftIndex}) but DID NOT exist.",
                        IS_DEBUGGING);
                    #endregion Logs
                    return snap;
                }

                TSnap leftSnap = m_snapshots[leftIndex];
                #region Logs
                CustomDebug.Log($"{GetType().Name} Returning snap at LEFT " +
                    $"index ({leftIndex}) with time {leftSnap.time}",
                    IS_DEBUGGING);
                #endregion Logs
                return leftSnap;
            }
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
