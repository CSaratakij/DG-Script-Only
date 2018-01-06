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

        [SerializeField]
        Vector2 boundfreeOffset;

        [SerializeField]
        LayerMask boundMask;

        [SerializeField]
        Transform upperBound;
        
        [SerializeField]
        Transform lowerBound;

        [SerializeField]
        Transform rightBound;

        [SerializeField]
        Transform leftBound;


        public bool IsUseFocus { get { return isUseFocus; } }


        bool isUseFocus;

        RaycastHit2D ray_UpperToLower;
        RaycastHit2D ray_LowerToUpper;
        RaycastHit2D ray_LeftToRight;
        RaycastHit2D ray_RightToLeft;

        Collider2D boxLeft;
        Collider2D boxRight;

        Vector3[] currentLinePoints;
        LineRenderer lineRenderer;


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
        }

        void FixedUpdate()
        {
            if (isEnableFocus) {

                var originUpper = new Vector2(target.position.x, currentLinePoints[0].y + 0.05f);
                var originLower = new Vector2(target.position.x, currentLinePoints[2].y - 0.05f);
                var originLeft = new Vector2(currentLinePoints[0].x, target.position.y);
                var originRight = new Vector2(currentLinePoints[1].x, target.position.y);

                var originCircleLeft = new Vector2(currentLinePoints[0].x, target.position.y);
                var originCircleRight = new Vector2(currentLinePoints[1].x, target.position.y);

                ray_UpperToLower = Physics2D.Raycast(originUpper, Vector2.down, 1000.0f, boundMask);
                ray_LowerToUpper = Physics2D.Raycast(originLower, Vector2.up, 1000.0f, boundMask);
                ray_LeftToRight = Physics2D.Raycast(originLeft, Vector2.right, 1000.0f, boundMask);
                ray_RightToLeft = Physics2D.Raycast(originRight, Vector2.left, 1000.0f, boundMask);

                boxLeft = Physics2D.OverlapBox(originLeft, new Vector2(0.25f, 1.0f), 0.0f, boundMask);
                boxRight = Physics2D.OverlapBox(originRight, new Vector2(0.25f, 1.0f), 0.0f, boundMask);
            }
        }

        void _Initialize()
        {
            currentLinePoints = new Vector3[4];
            lineRenderer = GetComponent<LineRenderer>();
        }

        void _InputHandler()
        {
            //handle player's input for expand world wrapping area..
        }

        void _FocusHandler()
        {
            var offsetX = 0.25f;
            var offsetY = 0.55f;

            if (target) {

                _MoveBoundPosition();

                //Conflict at the moment..

                //horizontal
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

                //right detect
                //swapp main instead of change position??
                //if (target.position.x > (currentLinePoints[1].x + offsetX)) {
                if (target.position.x > currentLinePoints[1].x) {
                    var newPos = target.position;
                    newPos.x = currentLinePoints[0].x + offsetX;
                    target.position = newPos;
                }

                //left detect
                //swapp main instead of change position??
                //------------------------------
                //possible solution to fix a bug abous world wrapping from horizontal
                //(if about to go to the left but target position is less than edge (right) y axis raycast (upper to lower ray) -> set axis y of target to the result of raycast)
                //------------------------------
                //else if (target.position.x < (currentLinePoints[0].x - offsetX)) {
                else if (target.position.x < currentLinePoints[0].x) {
                    var newPos = target.position;
                    newPos.x = currentLinePoints[1].x - offsetX;
                    target.position = newPos;
                }

                //vertical
                //upper
                if (target.position.y > (currentLinePoints[1].y + offsetY)) {
                    var newPos = target.position;
                    newPos.y = currentLinePoints[2].y;
                    target.position = newPos;
                }

                //lower
                if (target.position.y < (currentLinePoints[2].y - offsetY)) {
                    var newPos = target.position;
                    newPos.y = currentLinePoints[1].y;
                    target.position = newPos;
                }
            }
            else {
                Debug.Log("Can't find target...");
            }
        }

        void _MoveBoundPosition()
        {
            _MoveBoundVertical();
            _MoveBoundHorizontal();
        }

        void _MoveBoundVertical()
        {
            var expectPosUp = target.position;
            var expectPosDown = target.position;

            if (ray_LowerToUpper) {
                expectPosUp.y = currentLinePoints[0].y + ray_LowerToUpper.distance;
            }
            else {
                expectPosUp.y = currentLinePoints[0].y + boundfreeOffset.y;
            }

            upperBound.position = expectPosUp;

            if (ray_UpperToLower) {
                expectPosDown.y = currentLinePoints[2].y - ray_UpperToLower.distance;
            }
            else {
                expectPosDown.y = currentLinePoints[2].y - boundfreeOffset.y;
            }

            lowerBound.position = expectPosDown;
        }

        void _MoveBoundHorizontal()
        {
            var expectPosRight = target.position;
            var expectPosLeft = target.position;

            if (boxLeft) {
                expectPosRight.x = currentLinePoints[1].x;
            }
            else {
                if (ray_LeftToRight) {
                    expectPosRight.x = currentLinePoints[1].x + ray_LeftToRight.distance;
                }
                else {
                    expectPosRight.x = currentLinePoints[1].x + boundfreeOffset.x;
                }
            }
            
            rightBound.position = expectPosRight;

            if (boxRight) {
                expectPosLeft.x = currentLinePoints[0].x;
            }
            else {
                if (ray_RightToLeft) {
                    expectPosLeft.x = currentLinePoints[0].x - ray_RightToLeft.distance;
                }
                else {
                    expectPosLeft.x = currentLinePoints[0].x - boundfreeOffset.x;
                }
            }

            leftBound.position = expectPosLeft;
        }

        void _RepositionWorldWrappingRect()
        {
            _RedrawWorldWrappingAreaLine();
            _ResizeSpriteMask();
            _RepositionBound();
        }

        void _RepositionBound()
        {
            _RepositionBoundVertical();
            _RepositionBoundHorizontal();
        }

        void _RepositionBoundVertical()
        {
            var newPos = target.position;
            newPos.y += boundfreeOffset.y;

            upperBound.position = newPos;
            upperBound.gameObject.SetActive(true);

            newPos = target.position;
            newPos.y -= boundfreeOffset.y;

            lowerBound.position = newPos;
            lowerBound.gameObject.SetActive(true);
        }

        void _RepositionBoundHorizontal()
        {
            var newPos = target.position;
            newPos.x += boundfreeOffset.x;

            rightBound.position = newPos;
            rightBound.gameObject.SetActive(true);

            newPos = target.position;
            newPos.x -= boundfreeOffset.x;

            leftBound.position = newPos;
            leftBound.gameObject.SetActive(true);
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

        void _ClearWorldWrapping()
        {
            lineRenderer.positionCount = 0;
            mask.gameObject.SetActive(false);
            upperBound.gameObject.SetActive(false);
            lowerBound.gameObject.SetActive(false);
            rightBound.gameObject.SetActive(false);
            leftBound.gameObject.SetActive(false);
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
