using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
// Original Authors - Wyatt Senalik

namespace TimeTurned.Test
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimeDisplay : TimedBehaviour
    {
        private TextMeshProUGUI m_textMesh = null;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_textMesh = GetComponent<TextMeshProUGUI>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_textMesh, this);
            #endregion Asserts
        }


        public override void UpdateToTime(float time)
        {
            SetText(time);
        }


        private void SetText(float time)
        {
            m_textMesh.text = time.ToString();
            Color col = Color.white;
            if (!timedObject.isRecording)
            {
                col = Color.red;
            }
            m_textMesh.color = col;
        }
    }
}
