using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class MarkTargetCutscene : CutsceneTrigger
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        float delay;


        Transform previousTarget;
        CameraFolllow cameraFollow;


#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, size);
            Gizmos.DrawLine(transform.position, target.position);
            Handles.Label(transform.position, "Trigger Area");
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            cameraFollow = Camera.main.GetComponent<CameraFolllow>();
        }

        IEnumerator _PlayCutscene_Callback()
        {
            PlayerController.isInCinematic = true;
            GameController.FireEvent_OnCinematic();

            previousTarget = cameraFollow.CurrentTarget;
            cameraFollow.CurrentTarget = target;

            yield return new WaitForSeconds(delay);

            cameraFollow.CurrentTarget = previousTarget;

            PlayerController.isInCinematic = false;
            GameController.FireEvent_OnCinematic();

            MarkPlayed(true);
        }

        public override void PlayCutscene()
        {
            if (isPlayed) { return; }
            StartCoroutine(_PlayCutscene_Callback());
        }
    }
}
