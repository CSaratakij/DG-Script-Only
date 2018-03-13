using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class IntroStoryBoardUI : MonoBehaviour
    {
        [SerializeField]
        float showDelay;

        [SerializeField]
        float hideDelay;

        [SerializeField]
        RectTransform[] masks;


        void Start()
        {
            if (SaveInstance.IsSaveFileExists()) { 
                gameObject.SetActive(false);
                return; 
            }

            StartCoroutine(_ShowStoryBoard_Callback());
        }

        IEnumerator _ShowStoryBoard_Callback()
        {
            PlayerController.isInCinematic = true;

            foreach (RectTransform rect in masks) {
                yield return new WaitForSeconds(showDelay);
                rect.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(hideDelay);

            PlayerController.isInCinematic = false;
            gameObject.SetActive(false);
        }
    }
}
