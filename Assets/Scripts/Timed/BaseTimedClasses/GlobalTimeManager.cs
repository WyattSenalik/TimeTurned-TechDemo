using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Manages the current time for all <see cref="ITimedObject"/>s.
    /// Updates them to the current time every frame.
    /// </summary>
    [DisallowMultipleComponent]
    public class GlobalTimeManager : DynamicSingletonMonoBehaviour<GlobalTimeManager>
    {
        private const bool IS_DEBUGGING = false;

        // Serialized just for testing
        [SerializeField] private float m_timeScale = 1.0f;

        private readonly List<ITimedObject> m_objects = new List<ITimedObject>();
        private float m_curTime = 0.0f;


        private void Update()
        {
            // Update the current time and apply it to all timed objects
            m_curTime += m_timeScale * Time.deltaTime;
            m_curTime = Mathf.Max(0, m_curTime);
            UpdateTimedObjects(m_curTime);

            // For testing
            if (Input.GetKey(KeyCode.T))
            {
                UpdateTimedObjects(0);
                m_curTime = 0;
            }
        }


        /// <summary>
        /// Adds a <see cref="ITimedObject"/> to be synced to this
        /// <see cref="GlobalTimeManager"/>'s update.
        /// </summary>
        public void AddTimeObject(ITimedObject obj)
        {
            obj.UpdateToTime(m_curTime);
            m_objects.Add(obj);
        }
        /// <summary>
        /// Removes a <see cref="ITimedObject"/> to no longer be synced to this
        /// <see cref="GlobalTimeManager"/>'s update.
        /// </summary>
        public void RemoveTimeObject(ITimedObject obj)
        {
            m_objects.Remove(obj);
        }


        /// <summary>
        /// Calls <see cref="ITimedObject.UpdateToTime(float)"/> for each
        /// <see cref="ITimedObject"/>.
        /// </summary>
        /// <param name="time">The time to update the 
        /// <see cref="ITimedObject"/>s to.</param>
        private void UpdateTimedObjects(float time)
        {
            #region Logs
            CustomDebug.LogForComponent($"Updating objects to time {time}",
                this, IS_DEBUGGING);
            #endregion Logs
            foreach (ITimedObject obj in m_objects)
            {
                obj.UpdateToTime(time);
            }
        }
    }
}
