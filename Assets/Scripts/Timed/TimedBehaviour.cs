using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public abstract class TimedBehaviour : TimedComponent, ITimedBehaviour
    {
        public abstract void UpdateToTime(float time);
        public virtual void OnRecordingStop(float time) { }
    }
}
