using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class ChestController : MonoBehaviour
    {
        [SerializeField]
        uint requireCoin;

        [SerializeField]
        Vector2 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask activatorMask;

        [SerializeField]
        GameObject uiObject;


        public bool IsUnlocked { get { return isUnlocked; } set { isUnlocked = value; } }


        int hitCount;
        bool isUnlocked;

        Collider2D[] hit;
        Animator anim;


#if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + new Vector3(offset.x, offset.y, 0.0f), size);
            Handles.Label(transform.position + new Vector3(offset.x, offset.y, 0.0f), "Trigger Area");
        }
#endif

        void Awake()
        {
            hit = new Collider2D[1];
            anim = GetComponent<Animator>();

            _ShowInteractUI(false);
        }

        void Update()
        {
            _InputHandler();
            _AnimationHandler();
            _UIHandler();
        }

        void FixedUpdate()
        {
            if (isUnlocked) {
                hitCount = (hitCount > 0) ? 0 : hitCount;
                return;
            }

            hitCount = Physics2D.OverlapBoxNonAlloc(transform.position + new Vector3(offset.x, offset.y, 0.0f), size, 0.0f, hit, activatorMask);
        }

        void _InputHandler()
        {
            if (isUnlocked) { return; }
            if (hitCount <= 0) { return; }
            if (Input.GetButtonDown("Interact")) {
                _OpenChest(Coin.TotalPoint);
            }
        }

        void _AnimationHandler()
        {
            anim.SetBool("isUnlocked", isUnlocked);
        }

        void _OpenChest(uint receivedCoin)
        {
            if (requireCoin > receivedCoin) { return; }
            Coin.Remove(requireCoin);
            isUnlocked = true;
        }

        void _UIHandler()
        {
            if (hitCount > 0) {
                _ShowCoinView();
            }

            _ShowInteractUI(hitCount > 0);
        }

        void _ShowInteractUI(bool value)
        {
            if (!uiObject) {
                return;
            }

            if (uiObject.activeSelf != value) {
                uiObject.SetActive(value);
            }
        }

        void _ShowCoinView()
        {
            if (!CoinView.instance) {
                return;
            }

            CoinView.instance.ShowTemponary();
        }
    }
}
