using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        GameObject[] expectReuseOnWholeGame;

        [SerializeField]
        GameObject[] expectSpawnOnGameStart;


        public static GameController instance = null;

        public static bool isGameInit = false;
        public static bool isGameStarted = false;
        public static float loadingProgress = 0.0f;

        public delegate void LoadingSceneFunc();

        public static event LoadingSceneFunc OnLoadingScene;
        public static event LoadingSceneFunc OnLoadedScene;

        public static int expectDoorWrapID = -1;
        public static GameObject expectDoor = null; 

        GameObject[] runtimeExpectSpawnOnGameStart;

        AsyncOperation longOperation;
        GameSaveAgent gameSaveAgent;


        void Awake()
        {
            gameSaveAgent = GetComponent<GameSaveAgent>();

            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);

                SaveInstance.OnSaveDeleted += _OnSaveDeleted;
            }
            else {
                Destroy(this.gameObject);
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void Start()
        {
            _Spawn_Reuse_Object();
            SaveInstance.FireEvent_OnLoad();
        }

        void _Spawn_Reuse_Object()
        {
            for (int i = 0; i < expectReuseOnWholeGame.Length; i++) {
                var obj = (GameObject)Instantiate(expectReuseOnWholeGame[i]);
                DontDestroyOnLoad(obj);
            }
        }

        void _Spawn_Reuse_Object_Only_In_Game()
        {
            runtimeExpectSpawnOnGameStart = new GameObject[expectSpawnOnGameStart.Length];

            for (int i = 0; i < expectSpawnOnGameStart.Length; i++) {
                runtimeExpectSpawnOnGameStart[i] = (GameObject)Instantiate(expectSpawnOnGameStart[i]);
                DontDestroyOnLoad(runtimeExpectSpawnOnGameStart[i].gameObject);
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SaveInstance.FireEvent_OnLoad();
        }

        void Update()
        {
            if (GameController.isGameInit) {
                if (!GameController.isGameStarted && gameSaveAgent) {
                    _Spawn_Reuse_Object_Only_In_Game();
                    _StartGameHandler();
                    GameController.GameStart(true);
                }
            }
        }

        void _StartGameHandler()
        {
            if (SaveInstance.IsSaveFileExists()) {
                string targetSceneName = gameSaveAgent.LastActiveScene;

                if (targetSceneName != null) {
                    MoveToScene(targetSceneName, 2.0f, false);
                }
                else {
                    MoveToScene(2, 2.0f, false);
                }
            }
            else {
                MoveToScene(2, 2.0f, false);
            }
        }

        IEnumerator _LoadSceneAsync(string target, float delay)
        {
            longOperation = SceneManager.LoadSceneAsync(target);
            longOperation.allowSceneActivation = false;

            while (!longOperation.isDone) {
                GameController.loadingProgress = longOperation.progress;

                if (longOperation.progress >= 0.9f) {
                    break;
                }
                yield return null;
            }

            yield return new WaitForSeconds(delay);
            longOperation.allowSceneActivation = true;

            if (OnLoadedScene != null) {
                OnLoadedScene();
            }
        }

        IEnumerator _LoadSceneAsync(int target, float delay)
        {
            longOperation = SceneManager.LoadSceneAsync(target);
            longOperation.allowSceneActivation = false;

            while (!longOperation.isDone) {
                GameController.loadingProgress = longOperation.progress;

                if (longOperation.progress >= 0.9f) {
                    break;
                }
                yield return null;
            }

            yield return new WaitForSeconds(delay);
            longOperation.allowSceneActivation = true;

            if (OnLoadedScene != null) {
                OnLoadedScene();
            }
        }

        public static void GameStart(bool value)
        {
            isGameStarted = value;
        }

        public static void GameInit(bool value)
        {
            isGameInit = value;
        }

        public void MoveToScene(string sceneName, float delay, bool needSaveCurrent = true)
        {
            if (needSaveCurrent) {
                SaveInstance.FireEvent_OnSave();
            }

            GameController.loadingProgress = 0.0f;

            if (OnLoadingScene != null) {
                OnLoadingScene();
            }

            StartCoroutine(_LoadSceneAsync(sceneName, delay));
        }

        public void MoveToScene(int id, float delay, bool needSaveCurrent = true)
        {
            if (needSaveCurrent) {
                SaveInstance.FireEvent_OnSave();
            }

            GameController.loadingProgress = 0.0f;

            if (OnLoadingScene != null) {
                OnLoadingScene();
            }

            StartCoroutine(_LoadSceneAsync(id, delay));
        }

        public float GetLoadSceneProgress()
        {
            if (longOperation != null) {
                return longOperation.progress;
            }
            else {
                return 0.0f;
            }
        }

        public void ClearSpawnOnGameStartObject()
        {
            foreach (GameObject obj in runtimeExpectSpawnOnGameStart) {
                if (obj) {
                    Destroy(obj);
                }
            }
        }

        public void RestartGameFromSave()
        {
            GameController.ClearStaticContainer();
            SaveInstance.FireEvent_OnLoad();
            _StartGameHandler();
        }

        //Hacks
        public static void ClearStaticContainer()
        {
            Coin.TotalPoint = 0;
            Photo.Unlocked_Photo_List.Clear();

            if (GlobalPhoto.instance) {
                GlobalPhoto.instance.HideAllPhoto();
                GlobalPhoto.instance.HideAllPart();
                GlobalPhoto.instance.HideAllPartParent();
            }
        }

        void _OnSaveDeleted()
        {
            GameController.ClearStaticContainer();
        }
    }
}
