using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class CutsceneSaveAgent : SaveInstance
    {
        string json = "";
        CutsceneTrigger cutsceneTrigger;

        struct CutsceneSaveInfo
        {
            public bool isPlayed;
        }

        protected override void Awake()
        {
            base.Awake();
            cutsceneTrigger = GetComponent<CutsceneTrigger>();
        }

        void _SerializeToJson()
        {
            var info = new CutsceneSaveInfo();
            info.isPlayed = cutsceneTrigger.IsPlayed;
            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<CutsceneSaveInfo>(json);
            cutsceneTrigger.IsPlayed = info.isPlayed;
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
