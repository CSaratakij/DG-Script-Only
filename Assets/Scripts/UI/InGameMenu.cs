using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class InGameMenu : MonoBehaviour
    {
        //Need to change this to actual coin ui's prefabs
        [SerializeField]
        RectTransform panelCoin;


        public static InGameMenu instance;
        Canvas canvasInGame;


        void Awake()
        {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }

            canvasInGame = GetComponent<Canvas>();
            _Subscribe_Events();
        }

        void Destroy()
        {
            _Unsubscribe_Events();
        }

        void _OnPickedItem(GameObject obj)
        {
            _UpdateUI(obj.tag);
        }

        void _Hide()
        {
            canvasInGame.enabled = false;
        }

        void _Show()
        {
            canvasInGame.enabled = true;
        }

        void _UpdateUI(string tag) {
            switch (tag) {
                case "coin":
                    break;

                case "photo":
                    break;

                default:
                    break;
            }
        }

        void _Subscribe_Events()
        {
            Item.OnPickedItem += _OnPickedItem;
        }

        void _Unsubscribe_Events()
        {
            Item.OnPickedItem -= _OnPickedItem;
        }
    }
}
