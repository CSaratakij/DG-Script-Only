using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class ScrollTexture : MonoBehaviour
    {
        [SerializeField]
        Vector2 direction;

        [SerializeField]
        float speed;


        Vector2 offset;
        Renderer render;


        void Awake()
        {
            render = GetComponent<Renderer>();
        }

        void Update()
        {
            offset = (direction * speed) * Time.time;
            render.material.mainTextureOffset = offset;
        }
    }
}
