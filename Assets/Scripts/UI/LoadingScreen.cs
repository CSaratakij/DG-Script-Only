using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField]
        Slider slider;


        float maxProgress;
        float sliderProgress;

        Canvas canvas;


        void Awake()
        {
            canvas = GetComponent<Canvas>();
            _Subscribe_Events();
        }

        void _OnLoadingScene()
        {
            _FadeIn();
        }

        void _OnLoadedScene()
        {
            _FadeOut();
        }

        void _FadeIn()
        {
            if (canvas) {
                StartCoroutine(_Show_CallBack());
            }
        }

        void _FadeOut()
        {
            if (canvas) {
                _Hide();
            }
        }

        IEnumerator _Show_CallBack()
        {
            canvas.enabled = true;

            while (sliderProgress < 0.9f) {

                maxProgress = GameController.loadingProgress + 0.1f;
                sliderProgress += Mathf.Lerp(sliderProgress, maxProgress, 0.2f) * Time.deltaTime;
                sliderProgress = Mathf.Clamp(sliderProgress, 0.0f, 0.9f);

                slider.value = sliderProgress;
                yield return null;
            }
        }

        void _Hide()
        {
            maxProgress = GameController.loadingProgress + 0.1f;
            sliderProgress = maxProgress;

            sliderProgress = Mathf.Clamp(sliderProgress, 0.0f, maxProgress);
            slider.value = sliderProgress;
        }

        void OnDestroy()
        {
            _Unsubscribe_Events();
        }

        void _Subscribe_Events()
        {
            GameController.OnLoadingScene += _OnLoadingScene;
            GameController.OnLoadedScene += _OnLoadedScene;
        }

        void _Unsubscribe_Events()
        {
            GameController.OnLoadingScene -= _OnLoadingScene;
            GameController.OnLoadedScene -= _OnLoadedScene;
        }
    }
}
