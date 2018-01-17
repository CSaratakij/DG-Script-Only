using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class Mainmenu : MonoBehaviour
    {
        [SerializeField]
        Button continueButton;

        void Start()
        {
            var isEnable = SaveInstance.IsSaveFileExists();
            continueButton.enabled = isEnable;

            if (continueButton.gameObject.activeSelf != isEnable) {
                continueButton.gameObject.SetActive(isEnable);
            }
        }
    }
}
