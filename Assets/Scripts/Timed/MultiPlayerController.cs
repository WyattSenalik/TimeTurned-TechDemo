using System.Collections;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public class MultiPlayerController : SingletonMonoBehaviour<MultiPlayerController>
    {
        public TimedObject activeClone => m_activeClone;
        private TimedObject m_activeClone = null;

        [SerializeField] private TimedObject[] m_playerObjs = new TimedObject[3];
        [SerializeField] private Color[] m_playerCols = new Color[3]
        {
            new Color(1.0f, 0.0f, 0.0f),
            new Color(0.0f, 1.0f, 0.0f),
            new Color(0.0f, 0.0f, 1.0f)
        };

        private GlobalTimeManager m_timeMan = null;


        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_timeMan, this);
            #endregion Asserts
            SwitchActiveClone(0);
        }
        private void Update()
        {
            int activeCloneIndex = 0;
            for (KeyCode i = KeyCode.Alpha1; i <= KeyCode.Alpha3; ++i)
            {
                if (Input.GetKeyDown(i))
                {
                    if (IsCloneActive(activeCloneIndex)) { continue; }
                    SwitchActiveClone(activeCloneIndex);
                    break;
                }
                ++activeCloneIndex;
            }
        }


        private void SwitchActiveClone(int activeCloneIndex)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(activeCloneIndex, m_playerObjs, this);
            #endregion Asserts

            // Turn off all other clones
            for (int i = 0; i < m_playerObjs.Length; ++i)
            {
                DisableClone(i);
            }

            RewindTime(activeCloneIndex);
            //m_timeMan.SetTime(activeObj.farthestTime);
        }
        private bool IsCloneActive(int activeCloneIndex)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(activeCloneIndex, m_playerObjs, this);
            #endregion Asserts

            TimedObject activeObj = m_playerObjs[activeCloneIndex];
            return activeObj.shouldRecord;
        }
        private void DisableClone(int cloneIndex)
        {
            TimedObject cloneObj = m_playerObjs[cloneIndex];
            cloneObj.shouldRecord = false;
            SpriteRenderer sprRend = cloneObj.GetComponent<SpriteRenderer>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(sprRend,
                cloneObj.gameObject, this);
            #endregion Asserts
            Color col = m_playerCols[cloneIndex] * 0.3f;
            col.a = 1.0f;
            sprRend.color = col;
        }
        private void EnableClone(int cloneIndex)
        {
            // Turn on clone that should be active
            m_activeClone = m_playerObjs[cloneIndex];
            m_activeClone.shouldRecord = true;
            SpriteRenderer sprRend = m_activeClone.GetComponent<SpriteRenderer>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(sprRend,
                m_activeClone.gameObject, this);
            #endregion Asserts
            Color col = m_playerCols[cloneIndex];
            sprRend.color = col;
        }


        private void RewindTime(int activeCloneIndex)
        {
            StartCoroutine(RewindTimeCoroutine(activeCloneIndex));
        }
        private IEnumerator RewindTimeCoroutine(int activeCloneIndex)
        {
            TimedObject activeObj = m_playerObjs[activeCloneIndex];
            float farthestTime = activeObj.farthestTime;

            
            if (m_timeMan.curTime > farthestTime)
            {
                float curScale = -1;
                while (m_timeMan.curTime > farthestTime)
                {
                    m_timeMan.timeScale = curScale;
                    curScale -= Time.deltaTime * 5;
                    yield return null;
                }
            }
            else if (m_timeMan.curTime < farthestTime)
            {
                float curScale = 1;
                while (m_timeMan.curTime < farthestTime)
                {
                    m_timeMan.timeScale = curScale;
                    curScale += Time.deltaTime * 5;
                    yield return null;
                }
            }

            m_timeMan.timeScale = 1;
            EnableClone(activeCloneIndex);
        }
    }
}
