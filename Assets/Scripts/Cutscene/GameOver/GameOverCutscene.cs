using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG
{
    [RequireComponent(typeof(SwitchSignalReceiver))]
    public class GameOverCutscene : CutsceneTrigger
    {
        [SerializeField]
        Transform targetCat;

        [SerializeField]
        int targetSceneInBuildIndex;


        bool isEntered;
        SwitchSignalReceiver signal;


        protected override void Awake()
        {
            base.Awake();
            signal = GetComponent<SwitchSignalReceiver>();
        }

        protected override void Update()
        {
            base.Update();
            if (!isEntered && signal.IsTurnOn && !IsPlayed) {
                PlayCutscene();
                isEntered = true;
            }
        }

        public override void PlayCutscene()
        {
        }

        IEnumerator PlayCutscene_Callback()
        {
            /* yield return new WaitForSeconds( */
            yield return new WaitForSeconds(1.0f);
            //After done
            MarkPlayed(true);
        }
    }
}
