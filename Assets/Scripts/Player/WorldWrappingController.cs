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
        bool isCanEditMode;

        [SerializeField]
        bool isCanMoveMode;

        [SerializeField]
        Transform mask;

        [SerializeField]
        Transform target;
        
        [SerializeField]
        Vector2 offsetArea;

        [SerializeField]
        float marginX;

        [SerializeField]
        float marginY;

        [SerializeField]
        float moveModeSpeed;

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

        [SerializeField]
        LineRenderer highlightLineRenderer;

        [SerializeField]
        Gradient normalModeLineColor;

        [SerializeField]
        Gradient editModeLineColor;

        [SerializeField]
        Gradient selectSideLineColor;

        [SerializeField]
        Gradient moveModeLineColor;

        [SerializeField]
        Transform triangleRight;

        [SerializeField]
        Transform triangleUp;


        public bool IsUseFocus { get { return isUseFocus; } }

        public bool IsInEditMode { get { return isInEditMode; } }
        public bool IsInMoveMode { get { return isInMoveMode; } }

        public bool IsCanFocus { get { return isEnableFocus; } set { isEnableFocus = value; } }
        public bool IsCanMoveMode { get { return isCanMoveMode; } set { isCanMoveMode = value; } }
        public bool IsCanEditMode { get { return isCanEditMode; } set { isCanEditMode = value; } }

        public Vector3[] CurrentFocusPoint { get { return currentLinePoints; } }


        bool isUseFocus;
        bool isInEditMode;
        bool isInMoveMode;

        bool isUseAxisX;
        bool isUseAxisY;

        RaycastHit2D ray_UpperToLower;
        RaycastHit2D ray_LowerToUpper;
        RaycastHit2D ray_LeftToRight;
        RaycastHit2D ray_RightToLeft;

        Collider2D boxLeft;
        Collider2D boxRight;

        Vector2 lastSelectedSide;

        Vector3[] currentLinePoints;
        Vector3[] currentHighlightLinePoints;

        Vector3 originWorldWrappingPoint;
        Vector3 triangleMarkPos;

        LineRenderer lineRenderer;


        void Awake()
        {
            _Initialize();
        }

        void Start()
        {
            originWorldWrappingPoint = target.position;

            currentLinePoints[0].x = originWorldWrappingPoint.x - offsetArea.x;
            currentLinePoints[0].y = originWorldWrappingPoint.y + offsetArea.y;

            currentLinePoints[1].x = originWorldWrappingPoint.x + offsetArea.x;
            currentLinePoints[1].y = originWorldWrappingPoint.y + offsetArea.y;

            currentLinePoints[2].x = originWorldWrappingPoint.x + offsetArea.x;
            currentLinePoints[2].y = originWorldWrappingPoint.y - offsetArea.y;

            currentLinePoints[3].x = originWorldWrappingPoint.x - offsetArea.x;
            currentLinePoints[3].y = originWorldWrappingPoint.y - offsetArea.y;

            lineRenderer.colorGradient = normalModeLineColor;
            highlightLineRenderer.colorGradient = selectSideLineColor;
        }

        void Update()
        {
            _InputHandler();

            if (isEnableFocus && isUseFocus) {
                _FocusHandler();
                _WorldWrappingHandler();
                _UpdateFocusColor();

                //Hacks
                if (FocusButtonIndicator.instance) {
                    if (FocusButtonIndicator.instance.IsShow_Focus) {
                        if (FocusButtonIndicator.instance.IsCan_EditMode != isCanEditMode) {
                            FocusButtonIndicator.instance.IsCan_EditMode = isCanEditMode;
                        }

                        if (FocusButtonIndicator.instance.IsCan_MoveMode != isCanMoveMode) {
                            FocusButtonIndicator.instance.IsCan_MoveMode = isCanMoveMode;
                        }
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (isEnableFocus) {

                var originUpper = new Vector2(target.position.x, currentLinePoints[0].y + 0.03f);
                var originLower = new Vector2(target.position.x, currentLinePoints[2].y - 0.03f);

                var originLeft = new Vector2(currentLinePoints[0].x, target.position.y);
                var originRight = new Vector2(currentLinePoints[1].x, target.position.y);

                ray_UpperToLower = Physics2D.Raycast(originUpper, Vector2.down, 1000.0f, boundMask);
                ray_LowerToUpper = Physics2D.Raycast(originLower, Vector2.up, 1000.0f, boundMask);

                ray_LeftToRight = Physics2D.Raycast(originLeft, Vector2.right, 1000.0f, boundMask);
                ray_RightToLeft = Physics2D.Raycast(originRight, Vector2.left, 1000.0f, boundMask);

                boxLeft = Physics2D.OverlapBox(originLeft, new Vector2(0.25f, 1.4f), 0.0f, boundMask);
                boxRight = Physics2D.OverlapBox(originRight, new Vector2(0.25f, 1.4f), 0.0f, boundMask);
            }
        }

        void _Initialize()
        {
            currentLinePoints = new Vector3[4];
            currentHighlightLinePoints = new Vector3[2];
            lineRenderer = GetComponent<LineRenderer>();
            lastSelectedSide = Vector2.right;
        }

        void _InputHandler()
        {
            if (Input.GetButtonUp("Resize")) {
                isUseAxisX = false;
            }

            if (Input.GetButtonUp("Resize")) {
                isUseAxisY = false;
            }
        }

        void _FocusHandler()
        {
            if (isCanMoveMode && isInMoveMode) {
                _MoveModeHandler();
                _MoveBound_Vertical();
            }
            else {

                if (isCanEditMode && isInEditMode) {
                    _EditModeHandler();
                }
                else {
                    _ClearEditMode();
                }

                _MoveBoundPosition();
            }
        }

        void _MoveModeHandler()
        {
            _MoveModeHandler_Horizontal();
            _MoveModeHandler_Vertical();
        }

        void _MoveModeHandler_Horizontal()
        {
            var axisX = Input.GetAxisRaw("Horizontal");

            var boundOffset_Right = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
            var boundOffset_Left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));

            if (axisX > 0.0f) {
                var isCanMove = (currentLinePoints[0].x + marginX < target.position.x) && (currentLinePoints[1].x < boundOffset_Right.x);

                if (isCanMove) {
                    originWorldWrappingPoint.x += (axisX * moveModeSpeed) * Time.deltaTime;

                    currentLinePoints[0].x += (axisX * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[1].x += (axisX * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[2].x += (axisX * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[3].x += (axisX * moveModeSpeed) * Time.deltaTime;

                    _RepositionWorldWrappingRect();

                    _SelectSideHandler();
                    _RedrawHighlightLine();
                    _RePositionTriangle();
                }
            }
            else if (axisX < 0.0f) {
                var isCanMove = (currentLinePoints[1].x - marginX > target.position.x) && (currentLinePoints[0].x > boundOffset_Left.x);

                if (isCanMove) {
                    originWorldWrappingPoint.x += (axisX * moveModeSpeed) * Time.deltaTime;

                    currentLinePoints[0].x += (axisX * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[1].x += (axisX * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[2].x += (axisX * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[3].x += (axisX * moveModeSpeed) * Time.deltaTime;

                    _RepositionWorldWrappingRect();

                    _SelectSideHandler();
                    _RedrawHighlightLine();
                    _RePositionTriangle();
                }
            }
        }

        void _MoveModeHandler_Vertical()
        {
            var axisY = Input.GetAxisRaw("Vertical");

            var boundOffset_Upper = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
            var boundOffset_Lower = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));

            if (axisY > 0.0f) {
                var isCanMove = (currentLinePoints[2].y + marginY < target.position.y) && (currentLinePoints[0].y < boundOffset_Upper.y);

                if (isCanMove) {
                    originWorldWrappingPoint.y += (axisY * moveModeSpeed) * Time.deltaTime;

                    currentLinePoints[0].y += (axisY * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[1].y += (axisY * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[2].y += (axisY * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[3].y += (axisY * moveModeSpeed) * Time.deltaTime;

                    _RepositionWorldWrappingRect();

                    _SelectSideHandler();
                    _RedrawHighlightLine();
                    _RePositionTriangle();
                }
            }
            else if (axisY < 0.0f) {
                var isCanMove = (currentLinePoints[0].y - marginY > target.position.y) && (currentLinePoints[2].y > boundOffset_Lower.y);

                if (isCanMove) {
                    originWorldWrappingPoint.y += (axisY * moveModeSpeed) * Time.deltaTime;

                    currentLinePoints[0].y += (axisY * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[1].y += (axisY * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[2].y += (axisY * moveModeSpeed) * Time.deltaTime;
                    currentLinePoints[3].y += (axisY * moveModeSpeed) * Time.deltaTime;

                    _RepositionWorldWrappingRect();

                    _SelectSideHandler();
                    _RedrawHighlightLine();
                    _RePositionTriangle();
                }
            }
        }

        void _EditModeHandler()
        {
            _SelectSideHandler();
            _RedrawHighlightLine();
            _RePositionTriangle();
            _EditModeHandler_Horizontal();
            _EditModeHandler_Vertical();
        }

        void _SelectSideHandler()
        {
            _SelectSideHandler_Horizontal();
            _SelectSideHandler_Vertical();
        }

        void _SelectSideHandler_Horizontal()
        {
            var axisX = Input.GetAxisRaw("Horizontal");

            if (axisX > 0.0f) {
                lastSelectedSide = Vector2.right;
            }
            else if (axisX < 0.0f) {
                lastSelectedSide = Vector2.left;
            }
        }

        void _SelectSideHandler_Vertical()
        {
            var axisY = Input.GetAxisRaw("Vertical");

            if (axisY > 0.0f) {
                lastSelectedSide = Vector2.up;
            }
            else if (axisY < 0.0f) {
                lastSelectedSide = Vector2.down;
            }
        }

        void _RedrawHighlightLine()
        {
            if (Vector2.up == lastSelectedSide) {
                currentHighlightLinePoints[0] = currentLinePoints[0];
                currentHighlightLinePoints[1] = currentLinePoints[1];

                highlightLineRenderer.positionCount = 2;
                highlightLineRenderer.SetPositions(currentHighlightLinePoints);
            }
            else if (Vector2.down == lastSelectedSide) {
                currentHighlightLinePoints[0] = currentLinePoints[3];
                currentHighlightLinePoints[1] = currentLinePoints[2];

                highlightLineRenderer.positionCount = 2;
                highlightLineRenderer.SetPositions(currentHighlightLinePoints);
            }
            else if (Vector2.left == lastSelectedSide) {
                currentHighlightLinePoints[0] = currentLinePoints[0];
                currentHighlightLinePoints[1] = currentLinePoints[3];

                highlightLineRenderer.positionCount = 2;
                highlightLineRenderer.SetPositions(currentHighlightLinePoints);
            }
            else if (Vector2.right == lastSelectedSide) {
                currentHighlightLinePoints[0] = currentLinePoints[1];
                currentHighlightLinePoints[1] = currentLinePoints[2];

                highlightLineRenderer.positionCount = 2;
                highlightLineRenderer.SetPositions(currentHighlightLinePoints);
            }
        }

        void _RePositionTriangle()
        {
            var vertTriangleOffsetX = (currentLinePoints[1].x - currentLinePoints[0].x) * 0.5f;
            var horzTriangleOffsetY = (currentLinePoints[1].y - currentLinePoints[2].y) * 0.5f;

            var offsetX = 0.5f;
            var offsetY = 0.5f;

            if (Vector2.up == lastSelectedSide) {
                triangleMarkPos.x = currentLinePoints[0].x + vertTriangleOffsetX;
                triangleMarkPos.y = currentLinePoints[0].y + offsetY;
                triangleUp.position = triangleMarkPos;
            }
            else if (Vector2.down == lastSelectedSide) {
                triangleMarkPos.x = currentLinePoints[0].x + vertTriangleOffsetX;
                triangleMarkPos.y = currentLinePoints[2].y - offsetY;
                triangleUp.position = triangleMarkPos;
            }
            else if (Vector2.left == lastSelectedSide) {
                triangleMarkPos.x = currentLinePoints[0].x - offsetX;
                triangleMarkPos.y = currentLinePoints[0].y - horzTriangleOffsetY;
                triangleRight.position = triangleMarkPos;
            }
            else if (Vector2.right == lastSelectedSide) {
                triangleMarkPos.x = currentLinePoints[1].x + offsetX;
                triangleMarkPos.y = currentLinePoints[1].y - horzTriangleOffsetY;
                triangleRight.position = triangleMarkPos;
            }

            var scaleVert = triangleUp.localScale;
            var scaleHorz = triangleRight.localScale;

            scaleVert.y = (lastSelectedSide == Vector2.up) ? 1.0f : -1.0f;
            scaleHorz.x = (lastSelectedSide == Vector2.right) ? 1.0f : -1.0f;

            triangleUp.localScale = scaleVert;
            triangleRight.localScale = scaleHorz;

            triangleUp.gameObject.SetActive(lastSelectedSide == Vector2.up || lastSelectedSide == Vector2.down);
            triangleRight.gameObject.SetActive(lastSelectedSide == Vector2.left || lastSelectedSide == Vector2.right);
        }

        void _EditModeHandler_Horizontal()
        {
            var axisX = Input.GetAxisRaw("Resize");

            if (lastSelectedSide == Vector2.right) {

                if (!isUseAxisX) {

                    if (axisX > 0.0f) {
                        Debug.Log("Increase side right...");

                        isUseAxisX = true;

                        var boundOffset = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

                        if (currentLinePoints[1].x + 2 < boundOffset.x) {
                            currentLinePoints[1].x += 2;
                            currentLinePoints[2].x += 2;
                        }
                        else {
                            currentLinePoints[1].x = boundOffset.x;
                            currentLinePoints[2].x = boundOffset.x;
                        }

                        _RecalculateWorldWrappingCenter();
                        _RepositionWorldWrappingRect();
                    }
                    else if (axisX < 0.0f) {
                        Debug.Log("Decrease side right...");

                        isUseAxisX = true;

                        var isInWorldWrappingRect = _IsInWorldWrappingRect_Horizontal(
                                currentLinePoints[0],
                                new Vector2(currentLinePoints[1].x - (marginX + 2), 0.0f),
                                target.position
                        );

                        if (isInWorldWrappingRect) {
                            currentLinePoints[1].x -= 2;
                            currentLinePoints[2].x -= 2;

                            _RecalculateWorldWrappingCenter();
                            _RepositionWorldWrappingRect();
                        }
                        else {
                            Debug.Log("Can't, you dumbass...");
                        }
                    }
                }
            }
            else if (lastSelectedSide == Vector2.left) {

                if (!isUseAxisX) {

                    if (axisX > 0.0f) {
                        Debug.Log("Increase side left...");

                        isUseAxisX = true;

                        var boundOffset = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane));

                        if (currentLinePoints[0].x - 2 > boundOffset.x) {
                            currentLinePoints[0].x -= 2;
                            currentLinePoints[3].x -= 2;
                        }
                        else {
                            currentLinePoints[0].x = boundOffset.x;
                            currentLinePoints[3].x = boundOffset.x;
                        }

                        _RecalculateWorldWrappingCenter();
                        _RepositionWorldWrappingRect();
                    }
                    else if (axisX < 0.0f) {
                        Debug.Log("Decrease side left...");
                        isUseAxisX = true;


                        var isInWorldWrappingRect = _IsInWorldWrappingRect_Horizontal(
                                new Vector2(currentLinePoints[0].x + (marginX + 2), 0.0f),
                                currentLinePoints[1],
                                target.position
                        );

                        if (isInWorldWrappingRect) {
                            currentLinePoints[0].x += 2;
                            currentLinePoints[3].x += 2;

                            _RecalculateWorldWrappingCenter();
                            _RepositionWorldWrappingRect();
                        }
                        else {
                            Debug.Log("Can't, you dumbass...");
                        }
                    }
                }
            }
        }

        void _EditModeHandler_Vertical()
        {
            var axisY = Input.GetAxisRaw("Resize");

            if (lastSelectedSide == Vector2.up) {

                if (!isUseAxisY) {

                    if (axisY > 0.0f) {
                        Debug.Log("Increase side up...");

                        isUseAxisY = true;

                        var boundOffset = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

                        if (currentLinePoints[0].y + 1 < boundOffset.y) {
                            currentLinePoints[0].y += 1;
                            currentLinePoints[1].y += 1;
                        }
                        else {
                            currentLinePoints[0].y = boundOffset.y;
                            currentLinePoints[1].y = boundOffset.y;
                        }

                        _RecalculateWorldWrappingCenter();
                        _RepositionWorldWrappingRect();
                    }
                    else if (axisY < 0.0f) {
                        Debug.Log("Decrease side up...");

                        isUseAxisY = true;

                        var isInWorldWrappingRect = _IsInWorldWrappingRect_Vertical(
                            new Vector2(0.0f, currentLinePoints[0].y - (marginY + 1)),
                            currentLinePoints[2],
                            target.position
                        );

                        if (isInWorldWrappingRect) {
                            currentLinePoints[0].y -= 1;
                            currentLinePoints[1].y -= 1;

                            _RecalculateWorldWrappingCenter();
                            _RepositionWorldWrappingRect();
                        }
                        else {
                            Debug.Log("Can't, you dumbass...");
                        }
                    }
                }
            }
            else if (lastSelectedSide == Vector2.down) {

                if (!isUseAxisY) {

                    if (axisY > 0.0f) {
                        Debug.Log("Increase side down...");

                        isUseAxisY = true;

                        var boundOffset = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

                        if (currentLinePoints[2].y - 1 > boundOffset.y) {
                            currentLinePoints[2].y -= 1;
                            currentLinePoints[3].y -= 1;
                        }
                        else {
                            currentLinePoints[2].y = boundOffset.y;
                            currentLinePoints[3].y = boundOffset.y;
                        }

                        _RecalculateWorldWrappingCenter();
                        _RepositionWorldWrappingRect();
                    }
                    else if (axisY < 0.0f) {
                        Debug.Log("Decrease side down...");

                        isUseAxisY = true;

                        var isInWorldWrappingRect = _IsInWorldWrappingRect_Vertical(
                            currentLinePoints[0],
                            new Vector2(0.0f, currentLinePoints[2].y + (marginY + 1)),
                            target.position
                        );

                        if (isInWorldWrappingRect) {
                            currentLinePoints[2].y += 1;
                            currentLinePoints[3].y += 1;

                            _RecalculateWorldWrappingCenter();
                            _RepositionWorldWrappingRect();
                        }
                        else {
                            Debug.Log("Can't, you dumbass...");
                        }
                    }
                }
            }
        }

        void _WorldWrappingHandler()
        {
            if (target) {

                if (target.position.x > currentLinePoints[1].x) {
                    var newPos = target.position;
                    newPos.x = currentLinePoints[0].x + marginX;
                    target.position = newPos;
                }

                else if (target.position.x < currentLinePoints[0].x) {
                    var newPos = target.position;
                    newPos.x = currentLinePoints[1].x - marginX;
                    target.position = newPos;
                }

                if (target.position.y > (currentLinePoints[1].y + marginY)) {
                    var newPos = target.position;

                    if (ray_LowerToUpper) {

                        if (ray_LowerToUpper.distance < 1.2f) {
                            newPos.y = originWorldWrappingPoint.y;
                        }
                        else {
                            newPos.y = currentLinePoints[2].y;
                        }
                    }
                    else {
                        newPos.y = currentLinePoints[2].y;
                    }

                    target.position = newPos;
                }

                if (target.position.y < (currentLinePoints[2].y - marginY)) {
                    var newPos = target.position;

                    if (ray_UpperToLower) {

                        if (ray_UpperToLower.distance < 1.2f) {
                            newPos.y = originWorldWrappingPoint.y;
                        }
                        else {
                            newPos.y = currentLinePoints[1].y;
                        }
                    }
                    else {
                        newPos.y = currentLinePoints[1].y;
                    }

                    target.position = newPos;
                }
            }
            else {
                Debug.Log("Can't find target...");
            }
        }

        void _MoveBoundPosition()
        {
            _MoveBound_Horizontal();
            _MoveBound_Vertical();
        }

        void _MoveBound_Horizontal()
        {
            var expectPosRight = originWorldWrappingPoint;
            var expectPosLeft = originWorldWrappingPoint;

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

        void _MoveBound_Vertical()
        {
            var expectPosUp = originWorldWrappingPoint;
            var expectPosDown = originWorldWrappingPoint;

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


        void _MaintainWorldWrappingOffset()
        {
            _MaintainWorldWrappingOffset_Horizontal();
            _MaintainWorldWrappingOffset_Vertical();
        }

        void _MaintainWorldWrappingOffset_Horizontal()
        {
            var offsetX = originWorldWrappingPoint.x - target.position.x;

            currentLinePoints[0].x -= offsetX;
            currentLinePoints[1].x -= offsetX;
            currentLinePoints[2].x -= offsetX;
            currentLinePoints[3].x -= offsetX;
        }

        void _MaintainWorldWrappingOffset_Vertical()
        {
            var offsetY = originWorldWrappingPoint.y - target.position.y;

            currentLinePoints[0].y -= offsetY;
            currentLinePoints[1].y -= offsetY;
            currentLinePoints[2].y -= offsetY;
            currentLinePoints[3].y -= offsetY;
        }

        void _RepositionWorldWrappingRect()
        {
            _RedrawWorldWrappingAreaLine();
            _ResizeSpriteMask();
            _RepositionBound();
        }

        void _RepositionBound()
        {
            _RepositionBound_Vertical();
            _RepositionBound_Horizontal();
        }

        void _RepositionBound_Vertical()
        {
            var newPos = originWorldWrappingPoint;
            newPos.y += boundfreeOffset.y;

            upperBound.position = newPos;
            upperBound.gameObject.SetActive(true);

            newPos = originWorldWrappingPoint;
            newPos.y -= boundfreeOffset.y;

            lowerBound.position = newPos;
            lowerBound.gameObject.SetActive(true);
        }

        void _RepositionBound_Horizontal()
        {
            var newPos = originWorldWrappingPoint;
            newPos.x += boundfreeOffset.x;

            rightBound.position = newPos;
            rightBound.gameObject.SetActive(true);

            newPos = originWorldWrappingPoint;
            newPos.x -= boundfreeOffset.x;

            leftBound.position = newPos;
            leftBound.gameObject.SetActive(true);
        }

        void _RedrawWorldWrappingAreaLine()
        {
            lineRenderer.positionCount = currentLinePoints.Length;
            lineRenderer.SetPositions(currentLinePoints);
        }

        void _ResizeSpriteMask()
        {
            if (mask) {

                var offsetX = currentLinePoints[1].x - currentLinePoints[0].x;
                var offsetY = currentLinePoints[0].y - currentLinePoints[2].y;

                var scale = new Vector3(offsetX, offsetY, mask.localScale.z);
                mask.localScale = scale;

                var newPos = new Vector3(currentLinePoints[0].x + (offsetX / 2), currentLinePoints[2].y + (offsetY / 2), 1.0f);
                mask.position = newPos;

                mask.gameObject.SetActive(true);
            }
            else {
                Debug.Log("Can't find sprite mask..");
            }
        }

        void _RecalculateWorldWrappingCenter()
        {
            var offsetX = currentLinePoints[1].x - currentLinePoints[0].x;
            var offsetY = currentLinePoints[0].y - currentLinePoints[2].y;

            var center = new Vector3(currentLinePoints[0].x + (offsetX / 2), currentLinePoints[0].y - (offsetY / 2), 1.0f);
            var newPosY = center.y;

            if (target.position.y < center.y) {
                newPosY = center.y - (center.y - target.position.y);
            }
            else if (target.position.y > center.y) {
                newPosY = center.y + (center.y - target.position.y);
            }

            originWorldWrappingPoint.x = center.x;
            originWorldWrappingPoint.y = newPosY;
        }

        void _ClearWorldWrapping()
        {
            _ClearEditMode();

            lineRenderer.positionCount = 0;
            mask.gameObject.SetActive(false);
            upperBound.gameObject.SetActive(false);
            lowerBound.gameObject.SetActive(false);
            rightBound.gameObject.SetActive(false);
            leftBound.gameObject.SetActive(false);
        }

        void _ClearEditMode()
        {
            highlightLineRenderer.positionCount = 0;
            triangleUp.gameObject.SetActive(false);
            triangleRight.gameObject.SetActive(false);
        }

        void _UpdateFocusColor()
        {
            if (isInMoveMode) {
                lineRenderer.colorGradient = moveModeLineColor;
            }
            else {
                if (isInEditMode) {
                    lineRenderer.colorGradient = editModeLineColor;
                }
                else {
                    lineRenderer.colorGradient = normalModeLineColor;
                }
            }
        }

        bool _IsInWorldWrappingRect_Horizontal(Vector2 refLeftPos, Vector2 refRightPos, Vector2 pos) {
            return (pos.x > refLeftPos.x && pos.x < refRightPos.x);
        }
        
        bool _IsInWorldWrappingRect_Vertical(Vector2 refUpperPos, Vector2 refLowerPos, Vector2 pos) {
            return (pos.y < refUpperPos.y && pos.y > refLowerPos.y);
        }

        public void AllowFocus(bool value)
        {
            isEnableFocus = value;
        }

        public void UseFocus(bool value)
        {
            if (value) {
                _MaintainWorldWrappingOffset();
                originWorldWrappingPoint = target.position;
                _RepositionWorldWrappingRect();

                if (FocusButtonIndicator.instance) {
                    FocusButtonIndicator.instance.Show();
                }
            }
            else {
                _ClearWorldWrapping();

                if (FocusButtonIndicator.instance) {
                    FocusButtonIndicator.instance.Hide();
                }
            }

            isUseFocus = value;

            if (FocusButtonIndicator.instance) {
                FocusButtonIndicator.instance.IsShow_Focus = value;
                FocusButtonIndicator.instance.IsCan_EditMode = isCanEditMode;
                FocusButtonIndicator.instance.IsCan_MoveMode = isCanMoveMode;
            }
        }

        public void UseEditMode(bool value)
        {
            isInEditMode = value;

            if (FocusButtonIndicator.instance) {
                FocusButtonIndicator.instance.IsShow_EditMode = value;
            }
        }

        public void UseMoveMode(bool value)
        {
            isInMoveMode = value;

            if (FocusButtonIndicator.instance) {
                FocusButtonIndicator.instance.IsShow_MoveMode = value;
            }
        }
    }
}
