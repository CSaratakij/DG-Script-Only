using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG
{
    //need sigleton pattern
    public class GameController : MonoBehaviour
    {
        public static bool isGameInit = false;
        public static bool isGameStarted = false;
        public static float loadingProgress = 0.0f;

        public delegate void LoadingSceneFunc();

        public static event LoadingSceneFunc OnLoadingScene;
        public static event LoadingSceneFunc OnLoadedScene;


        AsyncOperation longOperation;
        GameSaveAgent gameSaveAgent;


        void Awake()
        {
            gameSaveAgent = GetComponent<GameSaveAgent>();
        }

        void Start()
        {
            SaveInstance.FireEvent_OnLoad();
        }

        void Update()
        {
            if (GameController.isGameInit) {
                if (!GameController.isGameStarted && gameSaveAgent) {
                    _StartGameHandler();
                    GameController.GameStart(true);
                }
            }

            //test
            if (GameController.isGameInit && GameController.isGameStarted) {
                if (Input.GetKeyDown(KeyCode.N)) {
                    MoveToScene(1, 3.0f);
                }
                else if (Input.GetKeyDown(KeyCode.M)) {
                    MoveToScene(2, 3.0f);
                }
            }
        }

        void _StartGameHandler()
        {
            var targetSceneName = gameSaveAgent.LastActiveScene;

            if (targetSceneName != null) {
                MoveToScene(targetSceneName, 3.0f, false);
            }
            else {
                MoveToScene(1, 3.0f, false);
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
    }
}
