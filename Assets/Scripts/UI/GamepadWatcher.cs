using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class GamepadWatcher : MonoBehaviour
    {
        public static bool isGamepadConnected = false;

        [SerializeField]
        bool lessFrequent;


        void Update()
        {
            if (lessFrequent) {
                if (Input.anyKeyDown) {
                    _CheckConnectedGamepad();
                }
            }
            else {
                _CheckConnectedGamepad();
            }
        }

        void _CheckConnectedGamepad()
        {
            foreach (var name in Input.GetJoystickNames()) {
                if (name != "") {
                    isGamepadConnected = true;
                    break;
                }
                isGamepadConnected = false;
            }
        }
    }
}
