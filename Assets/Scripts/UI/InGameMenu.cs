using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class InGameMenu : MonoBehaviour
    {
        Canvas canvasInGame;


        void Awake()
        {
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
