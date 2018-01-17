using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }

        public void QuitGame()
        {
            //propmt to make sure.
            //if yes -> exit game..
            Application.Quit();
        }
    }
}
