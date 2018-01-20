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
            //propmt to make sure.
            //if yes -> delete save file ..
            SaveInstance.DeleteSave();
            GameController.GameInit(true);
        }

        public void GameOver()
        {
            GameController.GameInit(false);
            GameController.GameStart(false);
        }

        public void BackToMainMenu()
        {
            //propmt to make sure.. -> all save that isn't write is loss..
            //if yes -> back to mainmenu..
            GameOver();

            if (GameController.instance != null) {
                //make game controller destroy intances in 
                //'need destroy on back to mainmenu list'
                GameController.instance.ClearSpawnOnGameStartObject();
                //
                //then
                GameController.instance.MoveToScene(0, 3.0f, false);
            }
            else {
                Debug.Log("Can't find game controller instance..");
                //normal load level here..
            }
        }

        public void QuitGame()
        {
            //propmt to make sure.
            //if yes -> exit game..
            Application.Quit();
        }
    }
}
