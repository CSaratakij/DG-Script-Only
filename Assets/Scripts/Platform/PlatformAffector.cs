//make child checking and setting its parent to this transform..
//not works ^
//sadly, this need reworks..
//hold on
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
        Vector2 areaSize;

        [SerializeField]
        Transform[] points;

        [SerializeField]
        LayerMask effectorMask;


        public bool IsPauseMoving { get { return isPauseMoving; } }


        int currentPointIndex;

        bool isInitChangeDir;
        bool isPauseMoving;

        Vector3 currentDirection;
        Vector3 velocity;

        Transform nextTargetPoint;
        Rigidbody2D rigid;


        public PlatformAffector()
        {
            points = new Transform[2];
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Handles.color = Color.red;

            for (int i = 0; i < (points.Length - 1); i++) {

                var startIndex = i;
                var endIndex = i + 1; 

                Handles.DrawDottedLine(
                    points[startIndex].position,
                    points[endIndex].position,
                    4.0f
                );
            }

            Handles.DrawWireCube(transform.position, new Vector3(offset.x, offset.y, 0.0f)); 
        }
#endif

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            nextTargetPoint = points[1];
            currentDirection = (nextTargetPoint.position - transform.position).normalized;
            rigid.position = points[0].position;
        }

        void Update()
        {
            _MoveThroughPoint();
        }

        void FixedUpdate()
        {
            _MoveHandler();
        }

        void _MoveHandler()
        {
            if (isInitChangeDir || isPauseMoving) {
                rigid.velocity = Vector2.zero;
            }
            else {
                velocity = (moveSpeed * currentDirection) * Time.deltaTime;
                rigid.velocity = velocity;
            }
        }

        void _MoveThroughPoint()
        {
            if (currentDirection.x > 0.0f) {

                var isReach = (transform.position.x + (offset.x * 0.5f) > nextTargetPoint.position.x);

                if (isReach) {

                    if (currentPointIndex < points.Length - 1) {
                        currentPointIndex += 1;

                        nextTargetPoint = points[currentPointIndex];
                        currentDirection = (nextTargetPoint.position - transform.position).normalized;

                        isInitChangeDir = true;
                        StartCoroutine(_ChangeDirection_CallBack());
                    }
                    else {
                        currentPointIndex -= 1;

                        nextTargetPoint = points[currentPointIndex];
                        currentDirection = (nextTargetPoint.position - transform.position).normalized;

                        isInitChangeDir = true;
                        StartCoroutine(_ChangeDirection_CallBack());
                    }
                }
            }
            else if (currentDirection.x < 0.0f) {

                var isReach =  (transform.position.x - (offset.x * 0.5f) < nextTargetPoint.position.x);

                if (isReach) {

                    if (currentPointIndex > 0) {
                        currentPointIndex -= 1;

                        nextTargetPoint = points[currentPointIndex];
                        currentDirection = (nextTargetPoint.position - transform.position).normalized;

                        isInitChangeDir = true;
                        StartCoroutine(_ChangeDirection_CallBack());
                    }
                    else {
                        currentPointIndex += 1;

                        nextTargetPoint = points[currentPointIndex];
                        currentDirection = (nextTargetPoint.position - transform.position).normalized;

                        isInitChangeDir = true;
                        StartCoroutine(_ChangeDirection_CallBack());
                    }
                }
            }
        }

        IEnumerator _ChangeDirection_CallBack()
        {
            yield return new WaitForSeconds(changeDirDelay);
            isInitChangeDir = false;
        }

        public void PauseMoving(bool value)
        {
            isPauseMoving = value;
        }
    }
}
