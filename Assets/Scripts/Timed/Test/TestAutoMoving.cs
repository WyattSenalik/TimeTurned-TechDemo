using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace TimeTurned
{
    [RequireComponent(typeof(TimedTransform))]
    public class TestAutoMoving : TimedBehaviour
    {
        [SerializeField] private Vector2 m_moveSpeed = new Vector2(0.0f, 0.0f);
        [SerializeField] private float m_rotSpeed = 0.0f;
        [SerializeField] private Vector2 m_scaleSpeed = new Vector2(0.0f, 0.0f);


        public override void TimedUpdate(float deltaTime)
        {
            // Change position
            Vector3 curPos3D = transform.position;
            Vector2 curPos = new Vector2(curPos3D.x, curPos3D.y);
            transform.position = curPos + m_moveSpeed * deltaTime;

            // Change rotation
            Vector3 newEulers = transform.eulerAngles;
            newEulers.z += m_rotSpeed * deltaTime;
            transform.eulerAngles = newEulers;

            // Change scale
            Vector3 curScale3D = transform.localScale;
            Vector2 curScale = new Vector2(curScale3D.x, curScale3D.y);
            curScale3D = curScale + m_scaleSpeed * deltaTime;
            curScale3D.z = 1;
            transform.localScale = curScale3D;
        }
    }
}