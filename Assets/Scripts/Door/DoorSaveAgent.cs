using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class DoorSaveAgent : SaveInstance
    {
        string json = "";
        Door door;

        struct DoorSaveInfo
        {
            public bool open;
            public bool allowEnter;
        }

        void _SerializeToJson()
        {
            var info = new DoorSaveInfo();
            door = GetComponent<Door>();

            if (door) {
                info.open = door.IsOpen;
                info.allowEnter = door.IsAllowEnter;
            }

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<DoorSaveInfo>(json);
            door = GetComponent<Door>();

            if (door) {
                door.IsOpen = info.open;
                door.IsAllowEnter = info.allowEnter;
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

        public override void FinishLoad()
        {
            var expectKey = GameController.expectDoorSaveKey;
            var expectID = (int)GameController.expectDoorSaveID;

            if (expectKey == key) {
                if (expectID == (int)id) {
                    GameController.expectDoor = this.gameObject;
                }
            }
        }
    }
}

