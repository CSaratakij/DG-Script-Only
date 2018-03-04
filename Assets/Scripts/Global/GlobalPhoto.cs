using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class GlobalPhoto : MonoBehaviour
    {
        GameObject[] allPhotos;


        public static GlobalPhoto instance;


        void Awake()
        {
            //Singleton
            //here..
            //
            //get childs
            //here..
        }

        public void Show(uint id, uint[] unlockedPartID)
        {
            //Todo
        }
    }
}
