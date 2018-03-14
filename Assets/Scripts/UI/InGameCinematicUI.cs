using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class InGameCinematicUI : MonoBehaviour
    {
        Canvas canvas;


        void Awake()
        {
            canvas = GetComponent<Canvas>();
            _Subscribe_Events();
        }

        void OnDestroy()
        {
            _Unsubscribe_Events();
        }

        void _OnCinematic(bool isIn)
        {
            canvas.enabled = isIn;
        }

        void _Subscribe_Events()
        {
            GameController.OnCinematic += _OnCinematic;
        }

        void _Unsubscribe_Events()
        {
            GameController.OnCinematic -= _OnCinematic;
        }
    }
}
