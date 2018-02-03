using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DG
{
    public class FootStepAudioPlayer : MonoBehaviour
    {
        [SerializeField]
        AudioClip[] footstepClips;

        [SerializeField]
        AudioClip[] impactClips;

        [SerializeField]
        string[] tagAsGrass;

        [SerializeField]
        string[] tagAsStones;


        AudioSource footstepAudioSource;
        AudioSource impactAudioSource;

        string previousMaterial;
        string currentMaterial;


        void Awake()
        {
            var audioSouce = GetComponents<AudioSource>();
            footstepAudioSource = audioSouce[0];
            impactAudioSource = audioSouce[1];
        }

        public void Play(string key)
        {
            previousMaterial = currentMaterial;
            currentMaterial = key;

            if (currentMaterial != previousMaterial) {
                footstepAudioSource.Stop();
            }

            if (!footstepAudioSource.isPlaying) {
                switch (key) {
                    case "grass":
                        footstepAudioSource.PlayOneShot(footstepClips[0]);
                    break;

                    case "stone":
                        footstepAudioSource.PlayOneShot(footstepClips[1]);
                    break;

                    default:
                        if (tagAsGrass.Contains(key)) {
                            footstepAudioSource.PlayOneShot(footstepClips[0]);
                        }
                        else if (tagAsStones.Contains(key)) {
                            footstepAudioSource.PlayOneShot(footstepClips[1]);
                        }
                        else {
                            Debug.Log("Cannot find key : " + key);
                        }
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
                        if (tagAsGrass.Contains(key)) {
                            impactAudioSource.PlayOneShot(impactClips[0]);
                        }
                        else if (tagAsStones.Contains(key)) {
                            impactAudioSource.PlayOneShot(impactClips[1]);
                        }
                        else {
                            Debug.Log("Cannot find key : " + key);
                        }
                    break;
                }
            }
        }

        public void PlayImpactForce(string key)
        {
            impactAudioSource.Stop();
            PlayImpact(key);
        }

        public void StopFootStep()
        {
            previousMaterial = "";
            currentMaterial = "";
            footstepAudioSource.Stop();
        }

        public void StopImpact()
        {
            impactAudioSource.Stop();
        }
    }
}
