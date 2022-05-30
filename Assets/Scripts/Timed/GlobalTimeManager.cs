using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public class GlobalTimeManager : SingletonMonoBehaviour<GlobalTimeManager>
    {
        [SerializeField] private float m_timeScale = 1.0f;

        private readonly List<TimedObject> m_objects = new List<TimedObject>();
        private float m_curTime = 0.0f;


        private void Update()
        {
            m_curTime += m_timeScale * Time.deltaTime;
            m_curTime = Mathf.Max(0, m_curTime);

            UpdateTimedObjects(m_curTime);

            if (Input.GetKey(KeyCode.T))
            {
                UpdateTimedObjects(0);
                m_curTime = 0;
            }
        }


        public void AddTimeObject(TimedObject obj)
        {
            obj.UpdateToTime(m_curTime);
            m_objects.Add(obj);
        }
        public void RemoveTimeObject(TimedObject obj)
        {
            m_objects.Remove(obj);
        }


        private void UpdateTimedObjects(float time)
        {
            foreach (TimedObject obj in m_objects)
            {
                obj.UpdateToTime(time);
            }
        }
    }
}
