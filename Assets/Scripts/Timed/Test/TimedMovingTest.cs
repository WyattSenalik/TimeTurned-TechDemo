using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace TimeTurned.Test
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimedTransform))]
    public class TimedMovingTest : TimedBehaviour
    {
        [SerializeField, Min(0.0f)] private float m_speed = 2;
        [SerializeField, Min(0.0f)] private float m_fireCooldown = 1.0f;
        [SerializeField, Required] private GameObject m_bulletPref = null;
        [SerializeField, Required] private Transform m_spawnTrans = null;

        [ShowNonSerializedField] private Vector2 m_fireDir = Vector2.up;
        private float m_prevFireTime = float.MinValue;


        public override void TimedUpdate(float deltaTime)
        {
            base.TimedUpdate(deltaTime);

            float hori = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");
            float fire = Input.GetAxis("Fire1");

            Vector2 pos2D = transform.position;
            Vector2 moveDir = new Vector2(hori, vert).normalized;
            transform.position = pos2D + moveDir * (m_speed * deltaTime);

            if (moveDir.sqrMagnitude > 0.0f) { m_fireDir = moveDir; }

            if (fire > 0.0f && m_prevFireTime + m_fireCooldown <= curTime)
            {
                float zAngle = Mathf.Rad2Deg * Mathf.Atan2(m_fireDir.y, m_fireDir.x);
                Instantiate(m_bulletPref, m_spawnTrans.position,
                    Quaternion.Euler(0.0f, 0.0f, zAngle));
                m_prevFireTime = curTime;
            }
        }
    }
}
