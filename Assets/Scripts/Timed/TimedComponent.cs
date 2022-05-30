using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    [RequireComponent(typeof(TimedObject))]
    public class TimedComponent : MonoBehaviour
    {
        public TimedObject timedObject { get; private set; } = null;
        public bool isRecording => timedObject.isRecording;


        protected virtual void Awake()
        {
            timedObject = GetComponent<TimedObject>();
        }
    }
}