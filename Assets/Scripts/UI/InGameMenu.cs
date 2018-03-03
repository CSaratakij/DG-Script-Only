using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class InGameMenu : MonoBehaviour
    {
        [SerializeField]
        CoinView coinView;


        Canvas canvasInGame;


        void Awake()
        {
            canvasInGame = GetComponent<Canvas>();
        }

        public void Show(bool value)
        {
            canvasInGame.enabled = value;
        }
    }
}
