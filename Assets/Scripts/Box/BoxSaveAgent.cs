using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class BoxSaveAgent : SaveInstance
    {
        string json = "";

        struct BoxSaveInfo
        {
            public float[] lastPosition;
        }

        void _SerializeToJson()
        {
            var info = new BoxSaveInfo();

            info.lastPosition = new float[3] {
                transform.position.x,
                transform.position.y,
                transform.position.z
            };

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<BoxSaveInfo>(json);
            var loadPos = new Vector3(info.lastPosition[0], info.lastPosition[1], info.lastPosition[2]);
            transform.position = loadPos;
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
