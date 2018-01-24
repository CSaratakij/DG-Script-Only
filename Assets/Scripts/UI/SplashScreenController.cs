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
            SceneManager.LoadScene(buildIndex);
        }
    }
}
