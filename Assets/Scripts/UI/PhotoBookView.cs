using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    public class PhotoBookView : MonoBehaviour
    {
        const uint MAX_PHOTO = 3;
        const string TOTAL_PHOTO_FORMAT = "{0} / {1}";


        [SerializeField]
        Text txtTotalPhoto;


        public static PhotoBookView instance;
        public int CurrentPhoto { get { return currentPhoto; } }


        int currentPhoto = 0;


        void Awake()
        {
            if (instance == null) {
                instance = this;
            }
            else {
                Destroy(this.gameObject);
            }

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
            _Update_UI((uint)currentPhoto);
        }

        void _Show(uint id)
        {
            GlobalPhoto.instance.ShowPhoto(id, true);
            GlobalPhoto.instance.ShowParentOfPhotoPart(id, true);
            _Update_UI(id);
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
            _Show((uint)currentPhoto);
        }

        void _Update_UI(uint id)
        {
            if (!txtTotalPhoto) { return; }
            txtTotalPhoto.text = string.Format(TOTAL_PHOTO_FORMAT, (id + 1), MAX_PHOTO);
        }
    }
}
