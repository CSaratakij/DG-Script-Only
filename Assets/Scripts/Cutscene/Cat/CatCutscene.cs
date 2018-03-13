using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class CatCutscene : MonoBehaviour
    {
        [SerializeField]
        float startDelay;

        [SerializeField]
        float runAwayDelay;

        [SerializeField]
        float runAwaySpeed;


        bool isEnteredTrigger;
        bool isBeginMove;

        Animator anim; 
        SwitchSignalReceiver signal;


        void Awake()
        {
            signal = GetComponent<SwitchSignalReceiver>();
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (!signal.IsTurnOn) { return; } 
            if (isEnteredTrigger) {
                if (isBeginMove) {
                    _MovementHandler();
                }
            }
            else {
                StartCoroutine(_Activate_Callback());
                isEnteredTrigger = true;
            }
        }

        IEnumerator _Activate_Callback()
        {
            yield return new WaitForSeconds(startDelay);
            anim.SetBool("isRunAway", true);

            yield return new WaitForSeconds(runAwayDelay);
            isBeginMove = true;
        }

        void _MovementHandler()
        {
            transform.Translate(Vector3.right * runAwaySpeed * Time.deltaTime);
        }
    }
}
