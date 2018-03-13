using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class PlatformController : MonoBehaviour
    {
        [SerializeField]
        bool isPauseMoving;

        [SerializeField]
        bool isUseWrap;

        [SerializeField]
        float moveSpeed;

        [SerializeField]
        float changeDirDelay;

        [SerializeField]
        Vector2 offset;

        [SerializeField]
        Transform[] points;


        public bool IsPauseMoving { get { return isPauseMoving; } }
        public Vector3 MoveDirection { get { return currentDirection; } }


        enum MoveState {
            Forward,
            Backward
        }

        int currentPointIndex;

        bool isInitChangeDir;
        bool isReach;

        Vector3 currentDirection;
        Vector3 velocity;

        Transform nextTargetPoint;
        Rigidbody2D rigid;

        MoveState currentMoveState;


        public PlatformController()
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

                if (points[startIndex] && points[endIndex]) {
                    Handles.DrawDottedLine(
                        points[startIndex].position,
                        points[endIndex].position,
                        4.0f
                    );
                }
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
            isReach = (nextTargetPoint.position - transform.position).magnitude <= 0.3f;
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
            if (MoveState.Forward == currentMoveState) {

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
                        currentMoveState = MoveState.Backward;

                        isInitChangeDir = true;
                        StartCoroutine(_ChangeDirection_CallBack());
                    }
                }
            }
            else if (MoveState.Backward == currentMoveState) {

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
                        currentMoveState = MoveState.Forward;

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
