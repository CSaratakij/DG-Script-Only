using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class Switch : MonoBehaviour, ISwitch
    {
        protected bool isTurnOn;

        public bool IsTurnOn { get { return isTurnOn; } set { isTurnOn = value; } }


        public Switch()
        {
            isTurnOn = false;
        }

        public void TurnOn()
        {
            isTurnOn = true;
        }

        public void TurnOff()
        {
            isTurnOn = false;
        }

        public void Toggle()
        {
            isTurnOn = !isTurnOn;
        }
    }
}
