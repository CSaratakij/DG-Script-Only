using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class CoinView : MonoBehaviour
    {
        const string COIN_TEXT_FORMAT = "x{0}";

        [SerializeField]
        Text txtCoin;


        public static CoinView instance;


        Timer timer;


        void Awake()
        {
            if (instance == null) {
                instance = this;
            }
            else {
                Destroy(this.gameObject);
            }

            timer = GetComponent<Timer>();
            _Subscribe_Events();
        }

        void Start()
        {
            Show(false);
        }

        void OnDestroy()
        {
            _Unsubscribe_Events();
        }

        void _OnLoadedScene()
        {
            _Update_Coin_UI(Coin.TotalPoint);
        }

        void _OnPointValueChanged(uint value)
        {
            _Update_Coin_UI(value);
            timer.CountDown();
        }

        void _OnTimerStart()
        {
            Show(true);
        }

        void _OnTimerStop()
        {
            Show(false);
        }

        void _Update_Coin_UI(uint value)
        {
            txtCoin.text = string.Format(COIN_TEXT_FORMAT, value);
        }

        void _OnPauseStateChanged(bool isShow, bool isInSubMenu)
        {
            if (isInSubMenu) {
                ShowPermenent(false);
            }
            else {
                ShowPermenent(isShow);
            }
        }

        void _Subscribe_Events()
        {
            if (timer) {
                timer.OnTimerStart += _OnTimerStart;
                timer.OnTimerStop += _OnTimerStop;
            }

            GameController.OnLoadedScene += _OnLoadedScene;
            Coin.OnPointValueChanged += _OnPointValueChanged;
            PauseMenu.OnPauseStateChanged += _OnPauseStateChanged;
        }

        void _Unsubscribe_Events()
        {
            if (timer) {
                timer.OnTimerStart -= _OnTimerStart;
                timer.OnTimerStop -= _OnTimerStop;
            }

            GameController.OnLoadedScene -= _OnLoadedScene;
            Coin.OnPointValueChanged -= _OnPointValueChanged;
            PauseMenu.OnPauseStateChanged -= _OnPauseStateChanged;
        }

        public void Show(bool value)
        {
            if (gameObject.activeSelf == value) { return; }
            gameObject.SetActive(value);
        }

        public void ShowTemponary()
        {
            timer.CountDown();
        }

        public void ShowPermenent(bool value)
        {
            timer.Stop();
            Show(value);
        }
    }
}
