// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Object that <see cref="ITimedComponent"/>s, <see cref="ITimedObserver"/>s,
    /// and <see cref="ITimedRecorder"/>s can be attached to.
    /// </summary>
    public interface ITimedObject : IMonoBehaviour
    {
        /// <summary>
        /// If time is advancing past this time object has seen yet.
        /// Indicates when a script can change data freely (true) and when
        /// it instead needs to look at past stored data (false) and update itself
        /// using that past data.
        /// </summary>
        public bool isRecording { get; }
        /// <summary>
        /// <see cref="isRecording"/> from the previous UpdateToTime call.
        /// </summary>
        public bool wasRecording { get; }
        /// <summary>
        /// If this <see cref="ITimedObject"/> should be recording when possible.
        /// </summary>
        public bool shouldRecord { get; set; }
        /// <summary>
        /// Current time.
        /// </summary>
        public float curTime { get; }
        /// <summary>
        /// Time this <see cref="ITimedObject"/> was created.
        /// </summary>
        public float spawnTime { get; }
        /// <summary>
        /// Furthest time this timed object has seen.
        /// </summary>
        public float farthestTime { get; }

        /// <summary>
        /// Updates all <see cref="ITimedObserver"/>s and
        /// <see cref="ITimedRecorder"/>s attached to this 
        /// <see cref="ITimedObject"/> to the given time.
        /// </summary>
        public void SetToTime(float time);
    }
}
