using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen instance = null;


        [SerializeField]
        Slider slider;


        float maxProgress;
        float sliderProgress;

        Canvas canvas;


        void Awake()
        {
            canvas = GetComponent<Canvas>();
            _Subscribe_Events();

            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
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

            maxProgress = 0.0f;
            sliderProgress = 0.0f;

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

            StartCoroutine(_Hide_CallBack());
        }

        IEnumerator _Hide_CallBack()
        {
            yield return new WaitForSeconds(0.3f);
            canvas.enabled = false;
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
