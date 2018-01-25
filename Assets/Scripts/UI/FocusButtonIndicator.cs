using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class FocusButtonIndicator : MonoBehaviour
    {
        public static FocusButtonIndicator instance = null;

        public bool IsCan_EditMode { get; set; }
        public bool IsCan_MoveMode { get; set; }

        public bool IsShow_Focus { get; set; }
        public bool IsShow_EditMode { get; set; }
        public bool IsShow_MoveMode { get; set; }


        [SerializeField]
        RectTransform focusPanel;

        [SerializeField]
        RectTransform editModePanel;

        [SerializeField]
        RectTransform moveModePanel;

        [SerializeField]
        RectTransform[] indicatorEditMode;

        [SerializeField]
        RectTransform[] indicatorMoveMode;


        Canvas canvas;


        void Awake()
        {
            canvas = GetComponent<Canvas>();

            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
        }

        void Start()
        {
            Hide();
        }

        void Update()
        {
            if (canvas.enabled) {
                _ShowButtonHandler();
            }
        }

        void _ShowButtonHandler()
        {
            moveModePanel.gameObject.SetActive(IsShow_Focus && IsShow_MoveMode);
            editModePanel.gameObject.SetActive(IsShow_Focus && IsShow_EditMode && !IsShow_MoveMode);
            focusPanel.gameObject.SetActive(IsShow_Focus && !IsShow_EditMode && !IsShow_MoveMode);

            //Hacks
            if (focusPanel.gameObject.activeSelf) {

                foreach (var obj in indicatorEditMode) {
                    obj.gameObject.SetActive(IsCan_EditMode);
                }

                foreach (var obj in indicatorMoveMode) {
                    obj.gameObject.SetActive(IsCan_MoveMode);
                }
            }
        }

        public void Show()
        {
            canvas.enabled = true;
        }

        public void Hide()
        {
            canvas.enabled = false;
        }
    }
}
