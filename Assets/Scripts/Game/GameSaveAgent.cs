using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

namespace DG
{
    public class GameSaveAgent : SaveInstance
    {
        public string LastActiveScene { get; private set; }


        string json = "";

        struct GameSaveInfo
        {
            public string lastSceneName;
        }


        void _SerializeToJson()
        {
            var info = new GameSaveInfo();
            info.lastSceneName = SceneManager.GetActiveScene().name;
            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<GameSaveInfo>(json);
            LastActiveScene = info.lastSceneName;
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
