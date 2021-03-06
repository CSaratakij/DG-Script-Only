﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class CameraSaveAgent : SaveInstance
    {
        string json = "";

        struct CameraSaveInfo
        {
            public float[] lastPosition;
        }

        void _SerializeToJson()
        {
            var info = new CameraSaveInfo();

            info.lastPosition = new float[3] {
                transform.position.x,
                transform.position.y,
                transform.position.z
            };

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<CameraSaveInfo>(json);
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
