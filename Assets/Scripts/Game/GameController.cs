using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG
{
    public class GameController : MonoBehaviour
    {
        public static bool isGameInit = false;
        public static bool isGameStarted = false;


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
        }

        void _StartGameHandler()
        {
            var targetSceneName = gameSaveAgent.LastActiveScene;

            if (targetSceneName != null) {
                MoveToScene(targetSceneName, 2.0f, false);
            }
            else {
                MoveToScene(1, 2.0f, false);
            }
        }

        IEnumerator _LoadSceneAsync(string target, float delay)
        {
            longOperation = SceneManager.LoadSceneAsync(target);
            longOperation.allowSceneActivation = false;

            while (!longOperation.isDone) {
                Debug.Log("Progress : " + longOperation.progress);
                if (longOperation.progress >= 0.9f) {
                    break;
                }
                yield return null;
            }

            yield return new WaitForSeconds(delay);
            longOperation.allowSceneActivation = true;
        }

        IEnumerator _LoadSceneAsync(int target, float delay)
        {
            longOperation = SceneManager.LoadSceneAsync(target);
            longOperation.allowSceneActivation = false;

            while (!longOperation.isDone) {
                Debug.Log("Progress : " + longOperation.progress);
                if (longOperation.progress >= 0.9f) {
                    break;
                }
                yield return null;
            }

            yield return new WaitForSeconds(delay);
            longOperation.allowSceneActivation = true;
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

            StartCoroutine(_LoadSceneAsync(sceneName, delay));
        }

        public void MoveToScene(int id, float delay, bool needSaveCurrent = true)
        {
            if (needSaveCurrent) {
                SaveInstance.FireEvent_OnSave();
            }

            StartCoroutine(_LoadSceneAsync(id, delay));
        }
    }
}
