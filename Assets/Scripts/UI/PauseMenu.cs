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
        Button btnControl;

        [SerializeField]
        RectTransform panelControl;

        [SerializeField]
        ScrollRect scrollRect;

        [SerializeField]
        RectTransform contentKeyboard;

        [SerializeField]
        RectTransform contentGamepad;

        [SerializeField]
        Dropdown[] controlmapViews;

        [SerializeField]
        EventSystem eventObj;


        public static PauseMenu instance = null;


        bool isShow;
        bool isUsingSubmenu;

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
            isUsingSubmenu = panelControl.gameObject.activeSelf;

            if (!isUsingSubmenu) {
                _InputHandler();
            }

            _SubMenuHandler();
        }

        void _InputHandler()
        {
            if (Input.GetButtonDown("PauseMenu")) {
                ToggleMenu();
            }
        }

        void _SubMenuHandler()
        {
            if (panelControl.gameObject.activeSelf) {

                if (Input.GetButtonDown("Cancel")) {
                    panelControl.gameObject.SetActive(false);
                    eventObj.SetSelectedGameObject(btnControl.gameObject, new BaseEventData(eventObj));
                }

                if (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.JoystickButton5)) {
                    foreach (var obj in controlmapViews) {
                        NextControlMapView(obj);
                    }
                }
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

        public void NextControlMapView(Dropdown target) {
            var newValue = target.value;
            newValue = (newValue == 0) ? 1 : 0;
            target.value = newValue;

            contentKeyboard.gameObject.SetActive(newValue == 0);
            contentGamepad.gameObject.SetActive(newValue == 1);

            scrollRect.content = (newValue == 0) ? contentKeyboard : contentGamepad;
        }
    }
}
