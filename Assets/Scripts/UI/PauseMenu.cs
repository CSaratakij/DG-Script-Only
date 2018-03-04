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
        Button btnJournal;

        [SerializeField]
        RectTransform pauseControl;

        [SerializeField]
        RectTransform panelControl;

        [SerializeField]
        RectTransform panelJournal;

        [SerializeField]
        ScrollRect scrollRect;

        [SerializeField]
        RectTransform contentKeyboard;

        [SerializeField]
        RectTransform contentGamepad;

        [SerializeField]
        Dropdown[] controlmapViews;

        [SerializeField]
        RectTransform[] dialogBoxes;

        [SerializeField]
        EventSystem eventObj;


        public delegate void Func(bool isShow, bool isInSubMenu);
        public static event Func OnPauseStateChanged;


        public static PauseMenu instance = null;


        bool isShow;
        bool isUsingSubmenu;

        Canvas canvas;


        void Awake()
        {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }

            canvas = GetComponent<Canvas>();
            _Subscribe_Events();

            eventObj.SetSelectedGameObject(null, new BaseEventData(eventObj));
        }
        
        void Update()
        {
            isUsingSubmenu = panelControl.gameObject.activeSelf || panelJournal.gameObject.activeSelf;

            if (isUsingSubmenu) {
                _SubMenuHandler();
            }
            else {
                _InputHandler();
            }
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

                    pauseControl.gameObject.SetActive(true);

                    panelControl.gameObject.SetActive(false);
                    panelJournal.gameObject.SetActive(false);

                    eventObj.SetSelectedGameObject(btnControl.gameObject, new BaseEventData(eventObj));
                    _FireEvent_OnPauseStateChanged(isShow, false);
                }
                else {
                    _FireEvent_OnPauseStateChanged(false, true);
                }

                if (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.JoystickButton5)) {
                    foreach (var obj in controlmapViews) {
                        NextControlMapView(obj);
                    }
                }
            }
            else if (panelJournal.gameObject.activeSelf) {

                if (Input.GetButtonDown("Cancel")) {

                    pauseControl.gameObject.SetActive(true);

                    panelControl.gameObject.SetActive(false);
                    panelJournal.gameObject.SetActive(false);

                    eventObj.SetSelectedGameObject(btnJournal.gameObject, new BaseEventData(eventObj));
                    _FireEvent_OnPauseStateChanged(isShow, false);
                }
                else {
                    _FireEvent_OnPauseStateChanged(false, true);
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


        void _FireEvent_OnPauseStateChanged(bool isShow, bool isInSubMenu)
        {
            if (OnPauseStateChanged != null) {
                OnPauseStateChanged(isShow, isInSubMenu);
            }
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
            eventObj.SetSelectedGameObject(null, new BaseEventData(eventObj));

            foreach (var obj in dialogBoxes) {
                obj.gameObject.SetActive(false);
            }

            _FireEvent_OnPauseStateChanged(isShow, false);
        }

        public void ToggleMenu()
        {
            isShow = !isShow;
            canvas.enabled = isShow;
            PlayerController.isInCinematic = isShow;

            if (isShow && btnResume && eventObj) {
                eventObj.SetSelectedGameObject(btnResume.gameObject, new BaseEventData(eventObj));
            }
            else {
                eventObj.SetSelectedGameObject(null, new BaseEventData(eventObj));
                foreach (var obj in dialogBoxes) {
                    obj.gameObject.SetActive(false);
                }
            }

            _FireEvent_OnPauseStateChanged(isShow, false);
        }

        public void NextControlMapView(Dropdown target) {
            var newValue = target.value;
            newValue = (newValue == 0) ? 1 : 0;
            target.value = newValue;

            contentKeyboard.gameObject.SetActive(newValue == 0);
            contentGamepad.gameObject.SetActive(newValue == 1);

            scrollRect.content = (newValue == 0) ? contentKeyboard : contentGamepad;
        }

        public void RestartGameFromSave()
        {
            if (GameController.instance) {
                Hide();
                GameController.instance.RestartGameFromSave();
            }
            else {
                Debug.Log("Can't find 'GameController'");
            }
        }

        public void UnSelectAnything()
        {
            eventObj.SetSelectedGameObject(null, new BaseEventData(eventObj));
        }
    }
}
