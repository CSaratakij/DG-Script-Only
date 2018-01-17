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
            _GameStartHandler();
        }

        void _GameStartHandler()
        {
            if (GameController.isGameInit && !GameController.isGameStarted && gameSaveAgent) {

                var targetSceneName = gameSaveAgent.LastActiveScene;
                if (targetSceneName != null) {
                    //load scene async here...?
                    SceneManager.LoadScene(gameSaveAgent.LastActiveScene);
                }
                else {
                    //load scene async here...?
                    SceneManager.LoadScene(1);
                }

                GameController.GameStart(true);
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

        public static void MoveToScene(string sceneName)
        {
            SaveInstance.FireEvent_OnSave();

            //load another scene (load async?)
            SceneManager.LoadScene(sceneName);
        }
    }
}
