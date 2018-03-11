using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class CutsceneTrigger : MonoBehaviour
    {
        [SerializeField]
        protected Vector2 size;

        [SerializeField]
        LayerMask playerMask;


        public bool IsPlayed { get { return isPlayed; } set { isPlayed = value; } }
        public bool IsReady { get { return isReady; } }


        protected int hitPlayerCount;

        protected bool isPlayed;
        protected bool isReady;

        protected Collider2D[] hit;


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, size);
            Handles.Label(transform.position, "Trigger Area");
        }
#endif

        protected virtual void Awake()
        {
            hit = new Collider2D[1];
        }

        protected virtual void Update()
        {
            if (isPlayed) { return; }
            if (hitPlayerCount > 0 && !isReady) {
                isReady = true;
                PlayCutscene();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (isPlayed) { return; }
            hitPlayerCount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0.0f,  hit, playerMask);
        }

        public virtual void PlayCutscene()
        {
        }

        public void MarkPlayed(bool value)
        {
            isPlayed = value;
        }
    }
}
