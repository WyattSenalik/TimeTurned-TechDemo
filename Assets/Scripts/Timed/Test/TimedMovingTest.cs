using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned.Test
{
    [RequireComponent(typeof(TimedTransform))]
    public class TimedMovingTest : TimedBehaviour
    {
        [SerializeField] private float m_speed = 2;


        public override void TimedUpdate(float deltaTime)
        {
            base.TimedUpdate(deltaTime);

            // If not recording, don't change position here
            if (!isRecording) { return; }

            float hori = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");

            Vector2 pos2D = transform.position;
            transform.position = pos2D + new Vector2(hori, vert) * (m_speed * deltaTime);
        }
    }
}
