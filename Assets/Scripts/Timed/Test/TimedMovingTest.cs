using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned.Test
{
    [RequireComponent(typeof(TimedTransform))]
    public class TimedMovingTest : TimedComponent
    {
        [SerializeField] private float m_speed = 2;


        private void Update()
        {
            // If not recording, don't change position here
            if (!isRecording) { return; }

            float hori = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");

            transform.position += new Vector3(hori, vert, 0) * (m_speed * Time.deltaTime); 
        }
    }
}
