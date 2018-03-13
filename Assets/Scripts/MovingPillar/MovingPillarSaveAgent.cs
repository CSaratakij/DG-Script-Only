using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class MovingPillarSaveAgent : SaveInstance
    {
        string json = "";
        SwitchSignalReceiver signalReceiver;

        struct MovingPillarSaveInfo
        {
            public bool[] switchStates;
        }

        void _SerializeToJson()
        {
            var info = new MovingPillarSaveInfo();
            var switchList = new List<Switch>();

            signalReceiver = GetComponent<SwitchSignalReceiver>();

            if (signalReceiver) {

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
            var info = JsonConvert.DeserializeObject<MovingPillarSaveInfo>(json);
            signalReceiver = GetComponent<SwitchSignalReceiver>();

            if (signalReceiver) {
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
