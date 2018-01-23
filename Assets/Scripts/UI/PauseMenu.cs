using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DG
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        Button btnResume;

        [SerializeField]
        EventSystem eventObj;


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

            eventObj.firstSelectedGameObject = btnResume.gameObject;
        }
        
        void Update()
        {
            _InputHandler();
        }

        void _InputHandler()
        {
            if (Input.GetButtonDown("PauseMenu")) {
                ToggleMenu();
            }
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
            PlayerController.isInCinematic = false;
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
            PlayerController.isInCinematic = false;
        }

        public void ToggleMenu()
        {
            isShow = !isShow;
            canvas.enabled = isShow;
            PlayerController.isInCinematic = isShow;

            if (isShow && btnResume && eventObj) {
                eventObj.SetSelectedGameObject(btnResume.gameObject, new BaseEventData(eventObj));
            }
        }
    }
}
