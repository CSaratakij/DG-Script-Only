using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class PlayerSaveAgent : SaveInstance
    {
        string json = "";

        struct PlayerSaveInfo
        {
            public float[] lastPosition;
        }


        void _SerializeToJson()
        {
            var info = new PlayerSaveInfo();

            info.lastPosition = new float[3] {
                transform.position.x,
                transform.position.y,
                transform.position.z
            };

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<PlayerSaveInfo>(json);
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

        public override void FinishLoad()
        {
            if (GameController.expectDoor != null) {
                transform.position = GameController.expectDoor.transform.position;
                GameController.expectDoorSaveKey = "";
                GameController.expectDoorSaveID = -1;
                GameController.expectDoor = null;
            }
            else {
                Debug.Log("Can't find expect door.");
            }
        }
    }
}
