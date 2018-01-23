using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class FocusUnlockerSaveAgent : SaveInstance
    {
        string json = "";
        FocusUnlocker focusUnlocker;


        struct FocusUnlockerSaveInfo
        {
            public bool isUsed;
        }


        void _SerializeToJson()
        {
            var info = new FocusUnlockerSaveInfo();
            focusUnlocker = GetComponent<FocusUnlocker>();

            if (focusUnlocker) {
                info.isUsed = focusUnlocker.IsUsed;
            }

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<FocusUnlockerSaveInfo>(json);
            focusUnlocker = GetComponent<FocusUnlocker>();

            if (focusUnlocker) {
                focusUnlocker.IsUsed = info.isUsed;
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
