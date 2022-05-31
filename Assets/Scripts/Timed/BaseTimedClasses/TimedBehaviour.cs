// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Implementation of <see cref="ITimedBehaviour"/> that can be extended to get 
    /// some easy references to the <see cref="ITimedObject"/> that this script is
    /// attached to. Also makes the various functions required by 
    /// <see cref="ITimedBehaviour"/> to either required (abstract) or option 
    /// (virtual) as needed.
    /// </summary>
    public abstract class TimedBehaviour : TimedComponent, ITimedBehaviour
    {
        public abstract void UpdateToTime(float time);
        public virtual void OnRecordingStop(float time) { }
    }
}
