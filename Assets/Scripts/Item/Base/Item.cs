using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField]
        bool isDisableOnUsed = true;

        [SerializeField]
        float disableDelay;

        [SerializeField]
        AudioClip collectSound;


        public delegate void Func(GameObject obj);
        public static Func OnPickedItem;

        public bool IsUsed { get { return isUsed; } set { isUsed = value; } }
        public bool IsInteractable { get { return isInteractable; } set { isInteractable = value; } }


        bool isUsed;
        bool isInteractable = true;

        AudioSource audioSource;
        SpriteRenderer spriteRenderer;


        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Collect()
        {
            if (isUsed) {
                return;
            }

            isUsed = true;
            spriteRenderer.enabled = false;

            _FireEvent_Picked_Item(this.gameObject);

            if (audioSource && collectSound) {
                audioSource.PlayOneShot(collectSound);
            }

            if (isDisableOnUsed) {
                StartCoroutine(_Disabled_Callback());
            }
        }

        void _OnTimerStop()
        {
            isInteractable = true;
        }

        void _FireEvent_Picked_Item(GameObject obj)
        {
            if (OnPickedItem != null) {
                OnPickedItem(obj);
            }
        }

        IEnumerator _Disabled_Callback()
        {
            yield return new WaitForSeconds(disableDelay);
            gameObject.SetActive(false);
        }

        IEnumerator _MakeInteractable_Callback(float delay)
        {
            yield return new WaitForSeconds(delay);
            isInteractable = true;
        }

        public void MakeInteractable(float delay)
        {
            StartCoroutine(_MakeInteractable_Callback(delay));
        }
    }
}
