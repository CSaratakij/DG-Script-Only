using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class JournalView : MonoBehaviour
    {
        int currentPhoto = 0;


        public int CurrentPhoto { get { return currentPhoto; } }


        void Awake()
        {
            _Subscribe_Events();
        }

        void OnDestroy()
        {
            _Unsubscribe_Events();
        }

        void _OnPhotoCollected(uint photoID, uint partID)
        {
            if (!GlobalPhoto.instance) {
                return;
            }

            currentPhoto = (int)GlobalPhoto.instance.CurrentPhotoInPhotoViewID;
        }

        void _Show(uint id)
        {
            GlobalPhoto.instance.ShowPhoto(id, true);
            GlobalPhoto.instance.ShowParentOfPhotoPart(id, true);
        }

        void _Subscribe_Events()
        {
            Photo.OnPhotoCollected += _OnPhotoCollected;
        }

        void _Unsubscribe_Events()
        {
            Photo.OnPhotoCollected -= _OnPhotoCollected;
        }

        public void NextPhoto()
        {
            if (!GlobalPhoto.instance) {
                return;
            }

            currentPhoto = (currentPhoto + 1 > (GlobalPhoto.instance.TotalPhoto - 1)) ? 0 : currentPhoto + 1 ;
            _Show((uint)currentPhoto);
        }

        public void PreviousPhoto()
        {
            if (!GlobalPhoto.instance) {
                return;
            }

            currentPhoto = (currentPhoto - 1 < 0) ? (GlobalPhoto.instance.TotalPhoto - 1) : currentPhoto - 1;
            _Show((uint)currentPhoto);
        }

        //Hacks
        public void RestoreToCurrentPhoto()
        {
            if (!GlobalPhoto.instance) {
                return;
            }

            currentPhoto = (int)GlobalPhoto.instance.CurrentPhotoInPhotoViewID;
        }
    }
}
