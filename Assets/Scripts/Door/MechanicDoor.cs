using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    [RequireComponent(typeof(SwitchSignalReceiver))]
    public class MechanicDoor : Door
    {
        [SerializeField]
        bool useInstantOpen = true;

        public bool IsAllowOpen { get { return signalReceiver.IsTurnOn; } }


        SwitchSignalReceiver signalReceiver;


        protected override void Awake()
        {
            base.Awake();
            signalReceiver = GetComponent<SwitchSignalReceiver>();
        }

        void Start()
        {
            _ToggleOpen();
        }

        void Update()
        {
            _ToggleOpen();
        }

        void _ToggleOpen()
        {
            if (useInstantOpen) {
                if (IsAllowOpen) {
                    Open();
                }
                else {
                    Close();
                }
            }
        }
    }
}
