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
        const string REQUIRE_COIN_TEXT_FORMAT = "x {0}";

        [SerializeField]
        uint requireCoin;

        [SerializeField]
        Transform itemParent;

        [SerializeField]
        Vector2 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask activatorMask;

        [SerializeField]
        GameObject uiObject;

        [SerializeField]
        TextMesh textMesh;


        public bool IsUnlocked { get { return isUnlocked; } set { isUnlocked = value; } }


        int hitCount;
        bool isUnlocked;

        Collider2D[] hit;
        Animator anim;


#if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + new Vector3(offset.x, offset.y, 0.0f), size);
            Handles.Label(transform.position, "Require Coin : " + requireCoin);
        }
#endif

        void Awake()
        {
            hit = new Collider2D[1];
            anim = GetComponent<Animator>();

            foreach (Transform item in itemParent) {
                item.gameObject.SetActive(false);
                item.position = transform.position;
            }

            var seed = (int)Random.Range(1, 10);
            Random.InitState(seed);

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
            _SplitOutItems();
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
                if (textMesh) {
                    textMesh.text = string.Format(REQUIRE_COIN_TEXT_FORMAT, requireCoin);
                }
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

        void _SplitOutItems()
        {
            if (hitCount <= 0 || !itemParent) {
                return;
            }

            if (itemParent.childCount <= 0) {
                return;
            }

            var offset = 1.0f;
            var vectorFromHitToChest = (transform.position - hit[0].transform.position);
            var origin = Vector3.zero;

            if (vectorFromHitToChest.x > 0) {
                origin = new Vector3(transform.position.x + offset, transform.position.y, 0.0f);
            }
            else {
                origin = new Vector3(transform.position.x - offset, transform.position.y, 0.0f);
            }

            foreach (Transform item in itemParent) {

                var force = Random.Range(200, 300);
                var direction = Vector2.zero;

                if (vectorFromHitToChest.x > 0) {
                    direction = new Vector2(0.4f, 1.0f).normalized;
                }
                else {
                    direction = new Vector2(-0.4f, 1.0f).normalized;
                }

                var velocity = direction * force * Time.deltaTime;
                var collider = item.gameObject.GetComponent<Collider2D>();

                if (collider && collider.isTrigger) {
                    collider.isTrigger = false;
                }

                var rigid = item.gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
                rigid.freezeRotation = true;

                item.position = origin;
                item.gameObject.SetActive(true);

                rigid.AddForce(velocity, ForceMode2D.Impulse);
            }
        }
    }
}
