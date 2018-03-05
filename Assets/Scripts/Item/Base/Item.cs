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


        bool isUsed;

        AudioSource audioSource;
        SpriteRenderer spriteRenderer;


        public bool IsUsed { get { return isUsed; } set { isUsed = value; } }


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
    }
}
