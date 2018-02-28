using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField]
    bool isDisableOnUsed = true;

    [SerializeField]
    float disableDelay;

    [SerializeField]
    AudioClip collectSound;


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

        if (audioSource && collectSound) {
            audioSource.PlayOneShot(collectSound);
        }

        if (isDisableOnUsed) {
            StartCoroutine(_Disabled_Callback());
        }
    }

    IEnumerator _Disabled_Callback()
    {
        yield return new WaitForSeconds(disableDelay);
        gameObject.SetActive(false);
    }
}
