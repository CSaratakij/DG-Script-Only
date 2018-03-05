using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class ChestSaveAgent : SaveInstance
    {
        string json = "";
        ChestController chest;

        struct ChestSaveInfo
        {
            public bool isUnlocked;
        }

        void _SerializeToJson()
        {
            var info = new ChestSaveInfo();
            chest = GetComponent<ChestController>();

            if (chest) {
                info.isUnlocked = chest.IsUnlocked;
            }

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<ChestSaveInfo>(json);
            chest = GetComponent<ChestController>();

            if (chest) {
                chest.IsUnlocked = info.isUnlocked;
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
