using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class GamepadWatcher : MonoBehaviour
    {
        public static bool isGamepadConnected = false;


        void Update()
        {
            _CheckConnectedGamepad();
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
