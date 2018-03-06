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
        const float BURST_OUT_FORCE = 370;
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

        Timer timer;
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
            timer = GetComponent<Timer>();

            foreach (Transform obj in itemParent) {
                obj.gameObject.SetActive(false);
                obj.position = transform.position;
                
                var item = obj.gameObject.GetComponent<Item>();

                if (item) {
                    item.IsInteractable = false;
                }
            }

            var seed = Random.value;
            Random.InitState((int)seed);

            _ShowInteractUI(false);
            _Subscribe_Events();
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

        void OnDestroy()
        {
            _Unsubscribe_Events();
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
            timer.CountDown();
            isUnlocked = true;
        }

        void _OnTimerStop()
        {
            _BurstOutItems();
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

        void _BurstOutItems()
        {
            if (!itemParent) {
                return;
            }

            if (itemParent.childCount <= 0) {
                return;
            }

            var offset = 0.2f;
            var vectorFromHitToChest = (transform.position - hit[0].transform.position);
            var origin = Vector3.zero;

            if (vectorFromHitToChest.x > 0) {
                origin = new Vector3(transform.position.x + offset, transform.position.y, 0.0f);
            }
            else {
                origin = new Vector3(transform.position.x - offset, transform.position.y, 0.0f);
            }

            foreach (Transform obj in itemParent) {

                var direction = Vector2.zero;

                if (vectorFromHitToChest.x > 0) {
                    direction = new Vector2(0.2f, 1.0f).normalized;
                }
                else {
                    direction = new Vector2(-0.2f, 1.0f).normalized;
                }

                var force = Random.Range(BURST_OUT_FORCE - 50, BURST_OUT_FORCE + 50);
                var velocity = (direction * force) * Time.deltaTime;
                var collider = obj.gameObject.GetComponent<Collider2D>();

                if (collider && collider.isTrigger) {
                    collider.isTrigger = false;
                }

                var rigid = obj.gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
                rigid.freezeRotation = true;

                obj.position = origin;
                obj.gameObject.SetActive(true);

                var item = obj.gameObject.GetComponent<Item>();

                if (item) {
                    item.MakeInteractable(1.0f);
                }

                rigid.AddForce(velocity, ForceMode2D.Impulse);
            }
        }

        void _Subscribe_Events()
        {
            if (timer) {
                timer.OnTimerStop += _OnTimerStop;
            }
        }

        void _Unsubscribe_Events()
        {
            if (timer) {
                timer.OnTimerStop -= _OnTimerStop;
            }
        }
    }
}
