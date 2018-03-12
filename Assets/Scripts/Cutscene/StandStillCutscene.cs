using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class StandStillCutscene : CutsceneTrigger
    {
        [SerializeField]
        float delay;

        [SerializeField]
        Transform[] disableObjects;


        protected override void Awake()
        {
            base.Awake();
            _Subscribe_Events();
        }

        void OnDestroy()
        {
            _Unsubscribe_Events();
        }

        void _OnFinishLoad()
        {
            if (IsPlayed) {
                _DisableObject();
            }
        }

        void _DisableObject()
        {
            foreach (var obj in disableObjects) {
                obj.gameObject.SetActive(false);
            }
        }

        void _Subscribe_Events()
        {
            SaveInstance.OnFinishLoad += _OnFinishLoad;
        }

        void _Unsubscribe_Events()
        {
            SaveInstance.OnFinishLoad -= _OnFinishLoad;
        }

        IEnumerator _PlayCutscene_Callback()
        {
            PlayerController.isInCinematic = true;
            GameController.FireEvent_OnCinematic();

            yield return new WaitForSeconds(delay);

            PlayerController.isInCinematic = false;
            GameController.FireEvent_OnCinematic();

            MarkPlayed(true);
            _DisableObject();
        }

        public override void PlayCutscene()
        {
            if (isPlayed) { return; }
            StartCoroutine(_PlayCutscene_Callback());
        }
    }
}
