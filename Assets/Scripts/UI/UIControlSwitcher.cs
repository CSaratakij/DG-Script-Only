using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class UIControlSwitcher : MonoBehaviour
    {
        [SerializeField]
        GameObject keyboardUI;

        [SerializeField]
        GameObject gamepadUI;


        void Update()
        {
            _ToggleUI();
        }

        void _ToggleUI()
        {
            if (GamepadWatcher.isGamepadConnected) {

                if (keyboardUI.activeSelf) {
                    keyboardUI.SetActive(false);
                }

                if (!gamepadUI.activeSelf) {
                    gamepadUI.SetActive(true);
                }
            }
            else {
                if (!keyboardUI.activeSelf) {
                    keyboardUI.SetActive(true);
                }

                if (gamepadUI.activeSelf) {
                    gamepadUI.SetActive(false);
                }
            }
        }
    }
}
