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
    }
}
