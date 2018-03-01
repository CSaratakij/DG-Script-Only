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
            public uint totalCoin;
        }


        void _SerializeToJson()
        {
            var info = new GameSaveInfo();
            var scene = SceneManager.GetActiveScene();

            info.lastSceneName = scene.name;
            info.totalCoin = Coin.TotalPoint;

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<GameSaveInfo>(json);
            LastActiveScene = info.lastSceneName;
            Coin.TotalPoint = info.totalCoin;
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
            else {
                LastActiveScene = "";
            }
        }
    }
}
