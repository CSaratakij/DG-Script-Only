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

            //test load last save..
            //need remove
            GameController.GameInit(true);
        }

        void Start()
        {
            SaveInstance.FireEvent_OnLoad();
        }

        void Update()
        {
            //Test..
            if (Input.GetKeyDown(KeyCode.S)) {
                SaveInstance.FireEvent_OnSave();
            }
            else if (Input.GetKeyDown(KeyCode.L)) {
                SaveInstance.FireEvent_OnLoad();
            }

            if (Input.GetKeyDown(KeyCode.N)) {
                MoveToScene("test");
            }
            else if (Input.GetKeyDown(KeyCode.M)) {
                MoveToScene("test2");
            }

            //test
            if (GameController.isGameInit && !GameController.isGameStarted && gameSaveAgent) {

                var targetSceneName = gameSaveAgent.LastActiveScene;
                if (targetSceneName != null) {
                    SceneManager.LoadScene(gameSaveAgent.LastActiveScene);
                }

                GameController.isGameStarted = true;
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
