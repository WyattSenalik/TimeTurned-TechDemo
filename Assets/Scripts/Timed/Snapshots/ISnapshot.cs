using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public interface ISnapshot<T, TSelf> where TSelf : ISnapshot<T, TSelf>
    {
        float time { get; }
        T data { get; }

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
