using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class PermernentSwitch : MonoBehaviour
    {
        [SerializeField]
        bool isOn;

        Switch _switch;


        void Awake()
        {
            _switch = GetComponent<Switch>();
        }

        void Start()
        {
            if (_switch) {
                if (isOn) {
                    _switch.TurnOn();
                }
                else {
                    _switch.TurnOff();
                }
            }
        }
    }
}
