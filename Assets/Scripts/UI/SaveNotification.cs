using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class SaveNotification : MonoBehaviour
    {
        const float HIDE_SAVE_NOTIFICATION_DELAY = 2.5f;
        public static SaveNotification instance = null;


        Canvas canvas;


        void Awake()
        {
            canvas = GetComponent<Canvas>();

            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
        }

        void Start()
        {
            Hide();
        }


        public void ShowNotification()
        {
            canvas.enabled = true;
            StartCoroutine(_Show_Notification_Callback());
        }

        public void Hide()
        {
            canvas.enabled = false;
        }

        IEnumerator _Show_Notification_Callback()
        {
            yield return new WaitForSeconds(HIDE_SAVE_NOTIFICATION_DELAY);
            Hide();
        }
    }
}
