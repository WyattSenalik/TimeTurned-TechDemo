using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned
{
    public class MultiPlayerController : MonoBehaviour
    {
        [SerializeField] private TimedObject[] m_playerObjs = new TimedObject[3];

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

            SpriteRenderer sprRend;
            // Turn off all other clones
            foreach (TimedObject obj in m_playerObjs)
            {
                obj.shouldRecord = false;
                sprRend = obj.GetComponent<SpriteRenderer>();
                #region Asserts
                CustomDebug.AssertComponentOnOtherIsNotNull(sprRend, 
                    obj.gameObject, this);
                #endregion Asserts
                sprRend.color = Color.gray;
            }
            // Turn on clone that should be active
            TimedObject activeObj = m_playerObjs[activeCloneIndex];
            activeObj.shouldRecord = true;
            sprRend = activeObj.GetComponent<SpriteRenderer>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(sprRend, 
                activeObj.gameObject, this);
            #endregion Asserts
            sprRend.color = Color.white;

            m_timeMan.SetTime(activeObj.farthestTime);
        }
        private bool IsCloneActive(int activeCloneIndex)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(activeCloneIndex, m_playerObjs, this);
            #endregion Asserts

            TimedObject activeObj = m_playerObjs[activeCloneIndex];
            return activeObj.shouldRecord;
        }
    }
}
