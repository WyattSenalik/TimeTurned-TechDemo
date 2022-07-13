using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
// Original Authors - Wyatt Senalik

namespace TimeTurned.Test
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimeDisplay : TimedObserver
    {
        private TextMeshProUGUI m_textMesh = null;
        private GlobalTimeManager m_globalTimeMan = null;
        private MultiPlayerController m_controller = null;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_textMesh = GetComponent<TextMeshProUGUI>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_textMesh, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_globalTimeMan = GlobalTimeManager.instance;
            m_controller = MultiPlayerController.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_globalTimeMan, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_controller, this);
            #endregion Asserts
        }


        public override void TimedUpdate(float deltaTime)
        {
            SetText(curTime);
        }


        private void SetText(float time)
        {
            ITimedObject activePlayer = m_controller.activeClone;
            m_textMesh.text = time.ToString();
            Color col = Color.white;
            if (activePlayer != null && !activePlayer.isRecording)
            {
                if (m_globalTimeMan.timeScale > 0)
                {
                    col = Color.green;
                }
                else if (m_globalTimeMan.timeScale < 0)
                {
                    col = Color.red;
                }
            }
            m_textMesh.color = col;
        }
    }
}
