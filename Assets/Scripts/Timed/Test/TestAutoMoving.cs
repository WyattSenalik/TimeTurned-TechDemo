using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace TimeTurned
{
    [RequireComponent(typeof(TimedTransform))]
    public class TestAutoMoving : TimedComponent
    {
        [SerializeField] private Vector3 m_speed = new Vector3(-1, 0, 0);


        private void Update()
        {
            // If not recording, don't change position here
            if (!isRecording) { return; }

            transform.position += m_speed * Time.deltaTime;
        }
    }
}