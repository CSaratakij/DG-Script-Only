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
        public bool IsUseInstantOpen { get { return useInstantOpen; } set { useInstantOpen = value; } }
        public SwitchSignalReceiver Receiver { get { return signalReceiver; } }


        SwitchSignalReceiver signalReceiver;


        protected override void Awake()
        {
            base.Awake();
            signalReceiver = GetComponent<SwitchSignalReceiver>();
        }

        protected override void Update()
        {
            base.Update();
            isAllowEnter = IsAllowOpen;
            _ToggleOpen();
        }

        void Start()
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
