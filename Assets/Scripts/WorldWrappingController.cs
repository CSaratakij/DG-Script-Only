using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class WorldWrappingController : MonoBehaviour
    {
        [SerializeField]
        bool isEnableFocus;

        [SerializeField]
        Transform mask;

        [SerializeField]
        Transform target;
        
        [SerializeField]
        Vector2 offsetArea;


        public bool IsUseFocus { get { return isUseFocus; } }


        Vector3[] currentLinePoints;
        LineRenderer lineRenderer;


        bool isUseFocus;


        void Awake()
        {
            _Initialize();
        }

        void Update()
        {
            _InputHandler();

            if (isEnableFocus && isUseFocus) {
                _FocusHandler();
            }
            else {
                Debug.Log("Disabled..");
            }
        }

        void _Initialize()
        {
            lineRenderer = GetComponent<LineRenderer>();
            currentLinePoints = new Vector3[4];
        }

        void _InputHandler()
        {
            //handle player's input for expand world wrapping area..
        }

        void _FocusHandler()
        {
            var offsetX = 0.3;

            if (target) {

                //Conflict at the moment..

                //Condition..
                //- If about to be the edge -> display second player sprite
                //- If about to be the edge + offset -> change main..

                //left..
                /*
                //swapp main instead of change position??
                if (target.position.x < currentLinePoints[0].x) {

                    var newPos = target.position;
                    newPos.x = currentLinePoints[1].x;
                    target.position = newPos;
                }
                */

                //right
                //swapp main instead of change position??
                if (target.position.x > (currentLinePoints[1].x + offsetX)) {
                    var newPos = target.position;
                    newPos.x = currentLinePoints[0].x;
                    target.position = newPos;
                }
                else if (target.position.x < (currentLinePoints[0].x - offsetX)) {
                    var newPos = target.position;
                    newPos.x = currentLinePoints[1].x;
                    target.position = newPos;
                }
            }
            else {
                Debug.Log("Can't find target...");
            }
        }

        void _RepositionWorldWrappingRect()
        {
            _RedrawWorldWrappingAreaLine();
            _ResizeSpriteMask();
        }

        void _ResizeSpriteMask()
        {
            if (mask) {
                var scale = new Vector3(offsetArea.x * 2, offsetArea.y * 2, mask.localScale.z);
                mask.localScale = scale;

                mask.position = transform.position;
                mask.gameObject.SetActive(true);
            }
            else {
                Debug.Log("Can't find sprite mask..");
            }
        }

        void _RedrawWorldWrappingAreaLine()
        {
            currentLinePoints[0].x = transform.position.x - offsetArea.x;
            currentLinePoints[0].y = transform.position.y + offsetArea.y;

            currentLinePoints[1].x = transform.position.x + offsetArea.x;
            currentLinePoints[1].y = transform.position.y + offsetArea.y;

            currentLinePoints[2].x = transform.position.x + offsetArea.x;
            currentLinePoints[2].y = transform.position.y - offsetArea.y;

            currentLinePoints[3].x = transform.position.x - offsetArea.x;
            currentLinePoints[3].y = transform.position.y - offsetArea.y;

            lineRenderer.positionCount = currentLinePoints.Length;
            lineRenderer.SetPositions(currentLinePoints);
        }

        void _ClearWorldWrapping()
        {
            lineRenderer.positionCount = 0;
            mask.gameObject.SetActive(false);
        }

        public void AllowFocus(bool value)
        {
            isEnableFocus = value;
        }

        public void UseFocus(bool value)
        {
            if (value) {
                _RepositionWorldWrappingRect();
            }
            else {
                _ClearWorldWrapping();
            }

            isUseFocus = value;
        }
    }
}
