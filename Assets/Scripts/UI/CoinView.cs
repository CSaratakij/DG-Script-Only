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


        Timer timer;


        void Awake()
        {
            timer = GetComponent<Timer>();
            _Subscribe_Events();
        }

        void Start()
        {
            _Show(false);
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
            _Show(true);
        }

        void _OnTimerStop()
        {
            _Show(false);
        }

        void _Show(bool value)
        {
            gameObject.SetActive(value);
        }

        void _Update_Coin_UI(uint value)
        {
            txtCoin.text = string.Format(COIN_TEXT_FORMAT, value);
        }

        void _Subscribe_Events()
        {
            if (timer) {
                timer.OnTimerStart += _OnTimerStart;
                timer.OnTimerStop += _OnTimerStop;
            }

            GameController.OnLoadedScene += _OnLoadedScene;
            Coin.OnPointValueChanged += _OnPointValueChanged;
        }

        void _Unsubscribe_Events()
        {
            if (timer) {
                timer.OnTimerStart -= _OnTimerStart;
                timer.OnTimerStop -= _OnTimerStop;
            }

            GameController.OnLoadedScene -= _OnLoadedScene;
            Coin.OnPointValueChanged -= _OnPointValueChanged;
        }
    }
}
