// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Object that <see cref="ITimedComponent"/>s and 
    /// <see cref="ITimedBehaviour"/> can be attached to.
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
        /// Updates all <see cref="ITimedBehaviour"/> attached to this 
        /// <see cref="ITimedObject"/> to the given time.
        /// </summary>
        public void UpdateToTime(float time);
    }
}
