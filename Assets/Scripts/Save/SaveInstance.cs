using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG
{
    public abstract class SaveInstance : MonoBehaviour, ISaveAble
    {
        [SerializeField]
        bool makeUnique;

        [SerializeField]
        protected string key;

        [SerializeField]
        protected uint id;


        public static string SavePath = "";
        public delegate void SaveAndLoadFunc();

        public static event SaveAndLoadFunc OnBeginSave;
        public static event SaveAndLoadFunc OnSave;
        /* public static event SaveAndLoadFunc OnFinishSave; */

        public static event SaveAndLoadFunc OnBeginLoad;
        public static event SaveAndLoadFunc OnLoad;
        /* public static event SaveAndLoadFunc OnFinishLoad; */


        public static Dictionary<string, Dictionary<string, Dictionary<uint, string>>> saveGameDict;

        public string Key { get { return key; } }
        public uint ID { get { return id; } set { id = value; } }

        protected string masterKey;


        public static void FireEvent_OnSave()
        {
            if (OnBeginSave != null) {
                OnBeginSave();
            }

            if (OnSave != null) {
                OnSave();
            }

            _SaveToDisk();
        }

        public static void FireEvent_OnLoad()
        {
            if (_LoadFromDisk()) {
                if (OnBeginLoad != null) {
                    OnBeginLoad();
                }

                if (OnLoad != null) {
                    OnLoad();
                }
            }
            else {
                Debug.Log("Can't load data from path : " + SavePath);
            }
        }

        public static void ClearEvent_OnSave()
        {
            OnSave = null;
        }

        public static void ClearEvent_OnLoad()
        {
            OnLoad = null;
        }

        public static void InsertRecord(string masterKey, string key, uint id, string json)
        {
            _EnsureKeyRequirement(masterKey, key, id);
            saveGameDict[masterKey][key][id] = json;
        }

        public static string LoadRecord(string masterKey, string key, uint id)
        {
            if (saveGameDict != null) {
                _EnsureKeyRequirement(masterKey, key, id);
                return saveGameDict[masterKey][key][id];
            }
            else {
                return "";
            }
        }

        static bool IsPathExist(string path)
        {
            return File.Exists(path);
        }

        static void _EnsureKeyRequirement(string masterKey, string key, uint id)
        {
            if (saveGameDict == null) {
                saveGameDict = new Dictionary<string, Dictionary<string, Dictionary<uint, string>>>();
            }

            if (!saveGameDict.ContainsKey(masterKey)) {
                saveGameDict.Add(masterKey, new Dictionary<string, Dictionary<uint, string>>());
            }

            if (!saveGameDict[masterKey].ContainsKey(key)) {
                saveGameDict[masterKey].Add(key, new Dictionary<uint, string>());
            }

            if (!saveGameDict[masterKey][key].ContainsKey(id)) {
                saveGameDict[masterKey][key].Add(id, "");
            }
        }

        static void _SaveToDisk()
        {
            var path = Application.persistentDataPath + "/savegame";
            var result = JsonConvert.SerializeObject(saveGameDict, Formatting.None);
            File.WriteAllText(path, result, Encoding.UTF8);
        }

        static bool _LoadFromDisk()
        {
            if (IsSaveFileExists()) {
                var path = Application.persistentDataPath + "/savegame";
                saveGameDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<uint, string>>>>(File.ReadAllText(path));
                return true;
            }
            else {
                saveGameDict = new Dictionary<string, Dictionary<string, Dictionary<uint, string>>>();
                return false;
            }
        }

        public static void DeleteSave()
        {
            saveGameDict.Clear();
            var path = Application.persistentDataPath + "/savegame";
            if (IsPathExist(path)) {
                File.Delete(path);
            }
        }

        public static bool IsSaveFileExists()
        {
            return IsPathExist(Application.persistentDataPath + "/savegame");
        }

        public virtual void Save()
        {

        }

        public virtual void Load()
        {

        }

        void Awake()
        {
            SaveInstance.SavePath = Application.persistentDataPath + "/savegame";
            masterKey = makeUnique ? "unique" : SceneManager.GetActiveScene().name;
            _Subscribe_Events();
        }

        void OnDestroy()
        {
            _Unsubscribe_Events();
        }

        void _Subscribe_Events()
        {
            SaveInstance.OnSave += Save;
            SaveInstance.OnLoad += Load;
        }

        void _Unsubscribe_Events()
        {
            SaveInstance.OnSave -= Save;
            SaveInstance.OnLoad -= Load;
        }
    }
}
