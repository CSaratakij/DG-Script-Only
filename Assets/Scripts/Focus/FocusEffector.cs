using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class FocusEffector : MonoBehaviour
    {
        [SerializeField]
        bool isActive;

        [SerializeField]
        Transform target;

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


        public bool IsActive { get { return isActive; } }


        SpriteRenderer render;
        WorldWrappingController worldWrappingControl;

        RaycastHit2D ray_UpperToLower;
        RaycastHit2D ray_LowerToUpper;
        RaycastHit2D ray_RightToLeft;
        RaycastHit2D ray_LeftToRight;


        void Awake()
        {
            _Initialilze();
        }

        void Update()
        {
            if (isActive && worldWrappingControl) {

                if (worldWrappingControl.IsUseFocus) {
                    _WorldWrappingHandler();
                    _MoveBoundHandler();

                    if (worldWrappingControl.IsInEditMode) {
                        render.maskInteraction = SpriteMaskInteraction.None;
                    }
                    else {
                        render.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    }
                }
                else {
                    render.maskInteraction = SpriteMaskInteraction.None;
                }

                upperBound.gameObject.SetActive(worldWrappingControl.IsUseFocus);
                lowerBound.gameObject.SetActive(worldWrappingControl.IsUseFocus);
                leftBound.gameObject.SetActive(worldWrappingControl.IsUseFocus);
                rightBound.gameObject.SetActive(worldWrappingControl.IsUseFocus);
            }
            else {

                upperBound.gameObject.SetActive(false);
                lowerBound.gameObject.SetActive(false);
                leftBound.gameObject.SetActive(false);
                rightBound.gameObject.SetActive(false);

                render.maskInteraction = SpriteMaskInteraction.None;
            }
        }

        void FixedUpdate()
        {
            if (isActive && worldWrappingControl) {

                var currentLinePoints = worldWrappingControl.CurrentFocusPoint;

                var originUpper = new Vector2(target.position.x, currentLinePoints[0].y + 0.03f);
                var originLower = new Vector2(target.position.x, currentLinePoints[2].y - 0.03f);

                var originLeft = new Vector2(currentLinePoints[0].x, target.position.y);
                var originRight = new Vector2(currentLinePoints[1].x, target.position.y);

                ray_UpperToLower = Physics2D.Raycast(originUpper, Vector2.down, 100.0f, boundMask);
                ray_LowerToUpper = Physics2D.Raycast(originLower, Vector2.up, 100.0f, boundMask);

                ray_LeftToRight = Physics2D.Raycast(originLeft, Vector2.right, 100.0f, boundMask);
                ray_RightToLeft = Physics2D.Raycast(originRight, Vector2.left, 100.0f, boundMask);
            }
        }

        void _Initialilze()
        {
            render = GetComponent<SpriteRenderer>();

            //instantiate bound from prefabs here..
            //for dev purpose -> normal setup would be fine..
        }

        void _WorldWrappingHandler()
        {
            _WorldWrapping_Horizontal();
            _WorldWrapping_Vertical();
        }

        void _WorldWrapping_Horizontal()
        {
            var offsetX = 0.3f;

            //wrap to right..
            if (target.position.x < worldWrappingControl.CurrentFocusPoint[0].x) {
                var newPos = worldWrappingControl.CurrentFocusPoint[1];

                newPos.x = worldWrappingControl.CurrentFocusPoint[1].x - offsetX;
                newPos.y = target.position.y;

                target.position = newPos;
            }
            //wrap to left..
            else if (target.position.x > worldWrappingControl.CurrentFocusPoint[1].x) {
                var newPos = worldWrappingControl.CurrentFocusPoint[0];

                newPos.x = worldWrappingControl.CurrentFocusPoint[0].x + offsetX;
                newPos.y = target.position.y;

                target.position = newPos;
            }
        }

        void _WorldWrapping_Vertical()
        {
            var offsetY = 0.8f;

            //wrap to up..
            if (target.position.y < worldWrappingControl.CurrentFocusPoint[2].y - offsetY) {
                var newPos = worldWrappingControl.CurrentFocusPoint[1];

                newPos.x = target.position.x;
                newPos.y = worldWrappingControl.CurrentFocusPoint[1].y;

                target.position = newPos;
            }
            //wrap to down..
            else if (target.position.y > worldWrappingControl.CurrentFocusPoint[1].y + offsetY) {
                var newPos = worldWrappingControl.CurrentFocusPoint[2];

                newPos.x = target.position.x;
                newPos.y = worldWrappingControl.CurrentFocusPoint[2].y;

                target.position = newPos;
            }
        }

        void _MoveBoundHandler()
        {
            _MoveBound_Horizontal();
            _MoveBound_Vertical();
        }

        void _MoveBound_Horizontal()
        {
            var newPos = worldWrappingControl.CurrentFocusPoint[0];

            if (ray_RightToLeft) {
                newPos.x = worldWrappingControl.CurrentFocusPoint[0].x - ray_RightToLeft.distance;
            }
            else {
                newPos.x = worldWrappingControl.CurrentFocusPoint[0].x;
            }

            newPos.y = target.position.y;
            leftBound.position = newPos;


            newPos = worldWrappingControl.CurrentFocusPoint[1];

            if (ray_LeftToRight) {
                newPos.x = worldWrappingControl.CurrentFocusPoint[1].x + ray_LeftToRight.distance;
            }
            else {
                newPos.x = worldWrappingControl.CurrentFocusPoint[1].x;
            }

            newPos.y = target.position.y;
            rightBound.position = newPos;
        }

        void _MoveBound_Vertical()
        {
            var newPos = worldWrappingControl.CurrentFocusPoint[0];

            if (ray_LowerToUpper) {
                newPos.y = worldWrappingControl.CurrentFocusPoint[0].y + ray_LowerToUpper.distance;
            }
            else {
                newPos.y = worldWrappingControl.CurrentFocusPoint[0].y;
            }

            newPos.x = target.position.x;
            upperBound.position = newPos;


            newPos = worldWrappingControl.CurrentFocusPoint[2];

            if (ray_UpperToLower) {
                newPos.y = worldWrappingControl.CurrentFocusPoint[2].y - ray_UpperToLower.distance;
            }
            else {
                newPos.y = worldWrappingControl.CurrentFocusPoint[2].y;
            }

            newPos.x = target.position.x;
            lowerBound.position = newPos;
        }

        public void UseEffector(bool value)
        {
            isActive = value;
        }

        public void SetAffector(PlayerController player)
        {
            if (player) {
                worldWrappingControl = player.GetComponent<WorldWrappingController>();
            }
            else {
                worldWrappingControl = null;
            }
        }
    }
}
