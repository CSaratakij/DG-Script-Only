using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace DG
{
    public class UILogic : MonoBehaviour
    {
        public void ContinueFromSave()
        {
            GameController.GameInit(true);
        }

        public void NewGame()
        {
            SaveInstance.FireEvent_OnSaveDeleted();
            GameController.GameInit(true);
        }

        public void GameOver()
        {
            GameController.GameInit(false);
            GameController.GameStart(false);
        }

        public void BackToMainMenu()
        {
            GameOver();

            if (GameController.instance != null) {
                GameController.instance.ClearSpawnOnGameStartObject();
                GameController.instance.MoveToScene(1, 3.0f, false);
            }
            else {
                Debug.Log("Can't find game controller instance..");
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
