using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DG
{
    public class SwitchSignalReceiver : Switch
    {
        [SerializeField]
        Switch[] switches;


        void Update()
        {
            _CheckAllSwitch();
        }

        void _CheckAllSwitch()
        {
            var isTurnOn = false;

            foreach (Switch obj in switches) {
                if (obj) {
                    isTurnOn = obj.IsTurnOn;

                    if (isTurnOn == false) {
                        break;
                    }
                }
            }

            this.isTurnOn = isTurnOn;
        }

        void OnDrawGizmosSelected()
        {
            foreach (Switch obj in switches) {
                if (obj) {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(obj.transform.position, transform.position);
                    Handles.Label(obj.transform.position + (Vector3.down * 0.2f), "Switch"); 
                }
            }
        }

    }
}
