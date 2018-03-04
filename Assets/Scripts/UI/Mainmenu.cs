using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DG
{
    public class Mainmenu : MonoBehaviour
    {
        [SerializeField]
        Button btnContinue;

        [SerializeField]
        Button btnNewGame;

        [SerializeField]
        Button btnCredits;

        [SerializeField]
        GameObject dialogConfirm;

        [SerializeField]
        GameObject panelCredit;

        [SerializeField]
        RectTransform panelBG;

        [SerializeField]
        EventSystem eventObj;


        void Start()
        {
            var isHasProgress = SaveInstance.IsSaveFileExists();
            btnContinue.enabled = isHasProgress;

            if (eventObj) {
                if (!eventObj.firstSelectedGameObject) {
                    eventObj.firstSelectedGameObject = (isHasProgress) ? btnContinue.gameObject : btnNewGame.gameObject;
                }
            }
        }

        void Update()
        {
            _SubMenuHandler();

            if (panelBG.gameObject.activeSelf != dialogConfirm.activeSelf) {
                panelBG.gameObject.SetActive(dialogConfirm.activeSelf);
            }
        }

        void _SubMenuHandler()
        {
            if (panelCredit.activeSelf) {
                if (Input.GetButtonDown("Cancel")) {
                    panelCredit.SetActive(false);
                    eventObj.SetSelectedGameObject(btnCredits.gameObject, new BaseEventData(eventObj));
                }
            }
        }

        public void UnSelectAnything()
        {
            eventObj.SetSelectedGameObject(null, new BaseEventData(eventObj));
        }
    }
}
