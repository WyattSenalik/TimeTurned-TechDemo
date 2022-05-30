using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public interface ISnapshotScrapbook<TSnap, TSnapType>
        where TSnap : ISnapshot<TSnapType, TSnap>
    {
        void AddSnapshot(TSnap snapshot);
        TSnap GetSnapshot(float time);
        float GetLatestTime();
    }
}
