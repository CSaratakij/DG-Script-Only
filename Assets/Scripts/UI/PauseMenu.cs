using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu instance = null;


        bool isShow;
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
        
        void Update()
        {
            _InputHandler();
        }

        void _InputHandler()
        {
            if (Input.GetButtonDown("PauseMenu")) {
                _ToggleMenu();
            }
        }

        void _ToggleMenu()
        {
            isShow = !isShow;
            canvas.enabled = isShow;
        }

        void _OnLoadingScene()
        {
            isShow = false;
            canvas.enabled = false;
            gameObject.SetActive(false);
        }

        void _OnLoadedScene()
        {
            gameObject.SetActive(true);
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

        public void Hide()
        {
            isShow = false;
            canvas.enabled = false;
        }
    }
}
