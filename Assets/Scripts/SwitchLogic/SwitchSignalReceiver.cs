using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class SwitchSignalReceiver : Switch
    {
        [SerializeField]
        Switch[] switches;


        public Switch[] Switches { get { return switches; } }


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

#if UNITY_EDITOR
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
#endif

    }
}
