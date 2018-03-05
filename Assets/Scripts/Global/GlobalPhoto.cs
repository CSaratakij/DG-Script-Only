using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class GlobalPhoto : MonoBehaviour
    {
        [SerializeField]
        Transform photoGroup;

        [SerializeField]
        Transform maskGroup;


        uint currentPhotoInPhotoViewID;

        Transform[] allPhotos;
        Transform[] allMasks;


        public static GlobalPhoto instance;

        public int TotalPhoto { get { return allPhotos.Length; } }
        public uint CurrentPhotoInPhotoViewID { get { return currentPhotoInPhotoViewID; } }


        void Awake()
        {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }

            _Initialize();
            _Subscribe_Events();
        }

        void OnDestroy()
        {
            _Unsubscribe_Events();
        }

        void _Initialize()
        {
            allPhotos = new Transform[photoGroup.childCount];
            allMasks = new Transform[maskGroup.childCount];

            for (int i = 0; i < allPhotos.Length; i++) {
                allPhotos[i] = photoGroup.GetChild(i);
            }

            for (int i = 0; i < allMasks.Length; i++) {
                allMasks[i] = maskGroup.GetChild(i);
            }
        }

        void _OnPhotoCollected(uint photoID, uint partID)
        {
            currentPhotoInPhotoViewID = photoID;
            ShowPhoto(photoID, partID, true);
        }

        void _Subscribe_Events()
        {
            Photo.OnPhotoCollected += _OnPhotoCollected;
        }

        void _Unsubscribe_Events()
        {
            Photo.OnPhotoCollected -= _OnPhotoCollected;
        }

        public void HideAllPhoto()
        {
            foreach (var child in allPhotos) {
                child.gameObject.SetActive(false);
            }
        }

        public void HideAllPartParent()
        {
            foreach (var child in allMasks) {
                child.gameObject.SetActive(false);
            }
        }

        public void HideAllPart()
        {
            foreach (Transform parent in allMasks) {
                foreach (Transform child in parent) {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public void ShowPhoto(uint photoID, bool isShow)
        {
            HideAllPhoto();

            if (photoID < allPhotos.Length) {
                allPhotos[photoID].gameObject.SetActive(isShow);
            }
            else {
                var log = string.Format("Can't find photo with id : {0}", photoID);
                Debug.Log(log);
            }
        }

        public void ShowParentOfPhotoPart(uint photoID, bool isShow)
        {
            HideAllPartParent();

            if (photoID < allMasks.Length) {
                allMasks[photoID].gameObject.SetActive(isShow);
            }
            else {
                var log = string.Format("Can't find parent part id : {0}", photoID);
                Debug.Log(log);
            }
        }

        public void ShowPhotoPart(uint photoID, uint partID, bool isShow)
        {
            ShowParentOfPhotoPart(photoID, isShow);

            try {
                var partObj = allMasks[photoID].GetChild((int)partID);
                partObj.gameObject.SetActive(isShow);
            }
            catch (Exception exception) {
                var log = string.Format("Part ID : {0} not exist..", partID);
                Debug.Log(log);
            }
        }

        public void ShowPhoto(uint photoID, uint partID, bool isShow)
        {
            ShowPhoto(photoID, isShow);
            ShowPhotoPart(photoID, partID, isShow);
        }

        public void ShowPhoto(uint photoID, uint[] partID, bool isShow)
        {
            ShowPhoto(photoID, isShow);
            for (int i = 0; i < partID.Length; i++) {
                ShowPhotoPart(photoID, partID[i], isShow);
            }
        }

        //Hacks..
        public void RestoreToCurrentPhoto()
        {
            ShowPhoto(currentPhotoInPhotoViewID, true);
            ShowParentOfPhotoPart(currentPhotoInPhotoViewID, true);
        }
    }
}
