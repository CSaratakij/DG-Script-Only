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
        EventSystem eventObj;


        void Start()
        {
            var isHasProgress = SaveInstance.IsSaveFileExists();

            btnContinue.enabled = isHasProgress;
            btnContinue.gameObject.SetActive(isHasProgress);

            if (eventObj) {
                if (!eventObj.firstSelectedGameObject) {
                    eventObj.firstSelectedGameObject = (isHasProgress) ? btnContinue.gameObject : btnNewGame.gameObject;
                }
            }
        }

        void Update()
        {
            _SubMenuHandler();
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
