using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG
{
    public class SplashScreenController : MonoBehaviour
    {
        [SerializeField]
        int buildIndex;

        [SerializeField]
        float delay;


        void Start()
        {
            StartCoroutine(_LoadScene_Callback());
        }

        IEnumerator _LoadScene_Callback()
        {
            yield return new WaitForSeconds(delay);

            //Hacks..
            //after last scene..
            GameController.GameInit(false);
            GameController.GameStart(false);

            if (GameController.instance) {
                GameController.instance.ClearSpawnOnGameStartObject();
                GameController.instance.MoveToScene(buildIndex, 0.0f, false);
            }

            SceneManager.LoadScene(buildIndex);
        }
    }
}
