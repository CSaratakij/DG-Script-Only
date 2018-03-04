using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class PhotoView : MonoBehaviour
    {
        [SerializeField]
        Image imgPhoto;


        bool isShow;


        void Awake()
        {
            _Subscribe_Events();
        }

        void Start()
        {
            Show(false);
        }

        void Update()
        {
            //Hacks...
            if (!PlayerController.isInCinematic && isShow) {
                PlayerController.isInCinematic = true;
            }

            if (isShow) {
                _InputHandler();
            }
        }

        void OnDestroy()
        {
            _Unsubscribe_Events();
        }

        void _InputHandler()
        {
            if (GamepadWatcher.isGamepadConnected) {
                if (Input.GetButtonDown("Cancel")) {
                    isShow = false;
                    Show(isShow);
                }
            }
            else {
                if (Input.GetButtonDown("Jump")) {
                    isShow = false;
                    Show(isShow);
                }
            }
        }

        void _OnPhotoCollected(uint id, uint partID)
        {
            //Need to change display logic..
            //might be painful,
            //maybe 2 camera will help us in this case?
            /* imgPhoto.sprite = "Texture in camera??" */
            Show(true);
        }

        void _OnLoadedScene()
        {
            if (isShow) {
                Show(false);
            }
        }

        void _Subscribe_Events()
        {
            Photo.OnPhotoCollected += _OnPhotoCollected;
            GameController.OnLoadedScene += _OnLoadedScene;
        }

        void _Unsubscribe_Events()
        {
            Photo.OnPhotoCollected -= _OnPhotoCollected;
            GameController.OnLoadedScene -= _OnLoadedScene;
        }

        public void Show(bool value)
        {
            isShow = value;
            PlayerController.isInCinematic = value;
            gameObject.SetActive(isShow);
        }
    }
}
