using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class FootStepAudioPlayer : MonoBehaviour
    {
        [SerializeField]
        AudioClip[] footstepClips;

        [SerializeField]
        AudioClip[] impactClips;


        AudioSource footstepAudioSource;
        AudioSource impactAudioSource;


        void Awake()
        {
            var audioSouce = GetComponents<AudioSource>();
            footstepAudioSource = audioSouce[0];
            impactAudioSource = audioSouce[1];
        }

        public void Play(string key)
        {
            if (!footstepAudioSource.isPlaying) {
                switch (key) {
                    case "grass":
                        footstepAudioSource.PlayOneShot(footstepClips[0]);
                    break;

                    case "stone":
                        footstepAudioSource.PlayOneShot(footstepClips[1]);
                    break;

                    default:
                        Debug.Log("Cannot find key : " + key);
                    break;
                }
            }
        }

        public void PlayImpact(string key)
        {
            if (!impactAudioSource.isPlaying) {
                switch (key) {
                    case "grass":
                        impactAudioSource.PlayOneShot(impactClips[0]);
                    break;

                    case "stone":
                        impactAudioSource.PlayOneShot(impactClips[1]);
                    break;

                    default:
                        Debug.Log("Cannot find key : " + key);
                    break;
                }
            }
        }
    }
}
