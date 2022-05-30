using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public interface ITimedBehaviour
    {
        TimedObject timedObject { get; }
        bool isRecording { get; }

        void UpdateToTime(float time);
        /// <summary>
        /// Called when the <see cref="TimedObject"/> when the recording stops.
        /// Recording stops when a jump to a previous time occurs or when
        /// time starts moving in reverse.
        /// </summary>
        /// <param name="time">Time when the recording stopped.</param>
        void OnRecordingStop(float time);
    }
}
