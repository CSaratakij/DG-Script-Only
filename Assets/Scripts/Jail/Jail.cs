using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG
{
    public class Jail : MonoBehaviour
    {
        //Hacks
        [SerializeField]
        bool isUseForLoadLevel;

        [SerializeField]
        int finalSceneIndex;

        [SerializeField]
        Transform jailDoor;

        [SerializeField]
        float startDelay;

        [SerializeField]
        float moveDelay;

        [SerializeField]
        float loadLevelDelay;

        [SerializeField]
        float catMoveForce;

        [SerializeField]
        Animator catAnimator;

        [SerializeField]
        Rigidbody2D catRigid;


        bool isCanMove;
        bool isEntered;

        SwitchSignalReceiver signal;


        void Awake()
        {
            signal = GetComponent<SwitchSignalReceiver>();
        }

        void Update()
        {
            _ToggleJailDoor(!signal.IsTurnOn);
        }

        void FixedUpdate()
        {
            if (signal.IsTurnOn && !isEntered) {
                StartCoroutine(_MoveCallback());
                isEntered = true;
            }

            if (isEntered && isCanMove) {
                catRigid.velocity = (Vector2.right * catMoveForce) * Time.deltaTime;
            }
        }

        void _ToggleJailDoor(bool value)
        {
            if (jailDoor.gameObject.activeSelf != value) {
                jailDoor.gameObject.SetActive(value);
            }
        }

        IEnumerator _MoveCallback()
        {
            PlayerController.isInCinematic = true;
            yield return new WaitForSeconds(startDelay);
            catAnimator.SetBool("isRunAway", true);

            yield return new WaitForSeconds(moveDelay);

            isCanMove = true;

            yield return new WaitForSeconds(loadLevelDelay);
            PlayerController.isInCinematic = false;

            if (isUseForLoadLevel) {

                if (GameController.instance) {
                    GameController.instance.MoveToScene(finalSceneIndex, 2.0f, true);
                }
                else {
                    SceneManager.LoadScene(finalSceneIndex);
                }
            }
        }
    }
}
