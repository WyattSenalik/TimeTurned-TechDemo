using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Moves <see cref="ITimedObject"/> along its up.
    /// </summary>
    public class TimedMoveRight : TimedBehaviour
    {
        [SerializeField] private float m_speed = 1.0f;


        public override void TimedUpdate(float deltaTime)
        {
            base.TimedUpdate(deltaTime);

            transform.position += transform.right * (m_speed * deltaTime);
        }
    }
}