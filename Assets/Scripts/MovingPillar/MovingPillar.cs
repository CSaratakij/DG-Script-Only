using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    [RequireComponent(typeof(SwitchSignalReceiver))]
    public class MovingPillar : MonoBehaviour
    {
        [SerializeField]
        float activateDelay;

        [SerializeField]
        float moveSpeed;

        [SerializeField]
        MovingPillarState activateState;

        [SerializeField]
        MovingPillarState disableState;

        [SerializeField]
        Transform upTarget;

        [SerializeField]
        Transform downTarget;

        [SerializeField]
        Vector3 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask boxLayerMask;


        public bool IsAllowOpen { get { return signalReceiver.IsTurnOn; } }
        public bool IsActivate { get { return currentState == activateState; } }


        enum MovingPillarState
        {
            None,
            Up,
            Down
        }


        int hitCount;
        bool isInitHit = false;

        Collider2D[] hit;

        SwitchSignalReceiver signalReceiver;
        MovingPillarState currentState;


#if UNITY_EDITOR
         void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + offset, size);
         }
#endif

        void Awake()
        {
            signalReceiver = GetComponent<SwitchSignalReceiver>();
            currentState = disableState;
            hit = new Collider2D[1];
        }

        void Update()
        {
            _MovePillarHandler();
        }

        void FixedUpdate()
        {
            hitCount = Physics2D.OverlapBoxNonAlloc(transform.position + offset, size, 0.0f,  hit, boxLayerMask);

            if (hitCount > 0 && !isInitHit) {
                _ResetBox(hit[0].transform.gameObject);
                isInitHit = true;
            }
        }

        void _ResetBox(GameObject obj)
        {
            StartCoroutine(_ResetBox_Callback(obj));
        }

        IEnumerator _ResetBox_Callback(GameObject obj)
        {
            obj.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.0f);

            obj.transform.position = obj.GetComponent<BoxController>().OriginalPosition;
            obj.gameObject.SetActive(true);

            isInitHit = false;
        }

        void _MovePillarHandler()
        {
            if (IsAllowOpen) {
                if (currentState != activateState) {
                    _MovePillar(activateState);
                }
            }
            else {
                if (currentState != disableState) {
                    _MovePillar(disableState);
                }
            }
        }

        void _MovePillar(MovingPillarState state)
        {
            switch (state) {
                case MovingPillarState.Up:
                {
                    var direction = (upTarget.position - transform.position).normalized;
                    transform.Translate((direction * moveSpeed) * Time.deltaTime);

                    if (transform.position.y >= upTarget.position.y) {
                        currentState = state;
                    }

                    break;
                }

                case MovingPillarState.Down:
                {
                    var direction = (downTarget.position - transform.position).normalized;
                    transform.Translate((direction * moveSpeed) * Time.deltaTime);

                    if (transform.position.y <= downTarget.position.y) {
                        currentState = state;
                    }

                    break;
                }

                default:
                    break;
            }
        }
    }
}
