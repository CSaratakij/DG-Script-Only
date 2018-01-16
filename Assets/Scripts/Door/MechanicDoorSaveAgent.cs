using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class MechanicDoorSaveAgent : SaveInstance
    {
        string json = "";

        MechanicDoor door;
        SwitchSignalReceiver signalReceiver;

        struct MechanicDoorSaveInfo
        {
            public bool open;
            public bool allowEnter;
            public bool useInstantOpen;
            public bool[] switchStates;
        }

        void _SerializeToJson()
        {
            var info = new MechanicDoorSaveInfo();
            var switchList = new List<Switch>();

            door = GetComponent<MechanicDoor>();
            signalReceiver = GetComponent<SwitchSignalReceiver>();

            if (signalReceiver && door) {
                info.open = door.IsOpen;
                info.allowEnter = door.IsAllowEnter;
                info.useInstantOpen = door.IsUseInstantOpen;

                foreach (var obj in signalReceiver.Switches) {
                    if (obj) {
                        switchList.Add(obj);
                    }
                }

                info.switchStates = new bool[switchList.Count];

                for (int i = 0; i < switchList.Count; i++) {
                    info.switchStates[i] = switchList[i].IsTurnOn;
                }
            }

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<MechanicDoorSaveInfo>(json);

            door = GetComponent<MechanicDoor>();
            signalReceiver = GetComponent<SwitchSignalReceiver>();

            if (signalReceiver && door) {
                 door.IsOpen = info.open;
                 door.IsAllowEnter = info.allowEnter;
                 door.IsUseInstantOpen = info.useInstantOpen;

                for (int i = 0; i < info.switchStates.Length; i++) {
                    signalReceiver.Switches[i].IsTurnOn = info.switchStates[i];
                }
            }
        }

        public override void Save()
        {
            _SerializeToJson();
            SaveInstance.InsertRecord(masterKey, key, id, json);
        }

        public override void Load()
        {
            var temp = SaveInstance.LoadRecord(masterKey, key, id);

            if (temp != "") {
                _DeserializeToInfo(temp);
            }
        }
    }
}
