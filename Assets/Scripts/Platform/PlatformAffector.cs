using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class PlatformAffector : MonoBehaviour
    {
        [SerializeField]
        bool isUseWrap;

        [SerializeField]
        float moveSpeed;

        [SerializeField]
        float changeDirDelay;

        [SerializeField]
        Vector2 offset;

        [SerializeField]
        Transform startPoint;

        [SerializeField]
        Transform endPoint;


        bool isInitChangeDir;

        Vector3 currentDirection;
        Vector3 velocity;

        Rigidbody2D rigid;


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Handles.color = Color.red;

            if (!startPoint) {
                return;
            }

            Handles.DrawDottedLine(startPoint.position, transform.position, 4.0f); 

            if (!endPoint) {
                return;
            }
           
            Handles.DrawDottedLine(transform.position, endPoint.position, 4.0f); 
            Handles.DrawWireCube(transform.position, new Vector3(offset.x, offset.y, 0.0f)); 
        }
#endif

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            currentDirection = endPoint.position - startPoint.position;
            currentDirection = currentDirection.normalized;

            rigid.position = startPoint.position;
        }

        void Update()
        {
            _ChangeDirectionHandler();
        }

        void FixedUpdate()
        {
            _MoveHandler();
        }

        void _ChangeDirectionHandler()
        {
            if ((transform.position.x + (offset.x * 0.5f)) > endPoint.position.x) {
                if (currentDirection.x > 0.0f) {
                    currentDirection = (startPoint.position - transform.position).normalized;
                    isInitChangeDir = true;
                    StartCoroutine(_ChangeDirection_CallBack());
                }
            }
            else if ((transform.position.x - (offset.x * 0.5f)) < startPoint.position.x) {
                if (currentDirection.x < 0.0f) {
                    currentDirection = (endPoint.position - transform.position).normalized;
                    isInitChangeDir = true;
                    StartCoroutine(_ChangeDirection_CallBack());
                }
            }
        }

        void _MoveHandler()
        {
            if (isInitChangeDir) {
                rigid.velocity = Vector2.zero;
            }
            else {
                velocity = moveSpeed * currentDirection * Time.deltaTime;
                rigid.velocity = velocity;
            }
        }

        IEnumerator _ChangeDirection_CallBack()
        {
            yield return new WaitForSeconds(changeDirDelay);
            isInitChangeDir = false;
        }
    }
}
