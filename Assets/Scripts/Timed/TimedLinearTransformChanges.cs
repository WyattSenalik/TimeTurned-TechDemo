using UnityEngine;

using Helpers;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    /// <summary>
    /// Updates the transform linearly over time based on the Timed system.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LinearTransformChanges))]
    public class TimedLinearTransformChanges : TimedBehaviour
    {
        private LinearTransformChanges m_linearTransChanges = null;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_linearTransChanges = GetComponent<LinearTransformChanges>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_linearTransChanges, this);
            #endregion Asserts
        }

        public override void TimedUpdate(float deltaTime)
        {
            base.TimedUpdate(deltaTime);

            m_linearTransChanges.UpdateChanges(deltaTime);
        }
    }
}