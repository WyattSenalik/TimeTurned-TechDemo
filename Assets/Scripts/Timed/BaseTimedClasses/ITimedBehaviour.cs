// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Behaviour meant to be attached to a <see cref="ITimedObject"/>.
    /// Has functions that will be called by <see cref="ITimedObject"/>.
    /// </summary>
    public interface ITimedBehaviour : ITimedComponent
    {
        /// <summary>
        /// Updates the ITimedBehaviour to be at the given time.
        /// </summary>
        void UpdateToTime(float time);
        /// <summary>
        /// Called when the <see cref="ITimedObject"/> when the recording stops.
        /// Recording stops when a jump to a previous time occurs or when
        /// time starts moving in reverse.
        /// </summary>
        /// <param name="time">Time when the recording stopped.</param>
        void OnRecordingStop(float time);
        /// <summary>
        /// Called every frame by <see cref="ITimedObject"/> when recording.
        /// </summary>
        /// <param name="deltaTime">Time since the last TimedUpdate.</param>
        void TimedUpdate(float deltaTime);
    }
}
