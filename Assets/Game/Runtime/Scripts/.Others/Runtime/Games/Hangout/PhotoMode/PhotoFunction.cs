using System;
using System.Collections;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Tracking;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Hangout
{
    public class PhotoFunction : MonoBehaviour
    {
        [SerializeField] private Image frameImage;
        [SerializeField] private RawImage photoHolder;
        [SerializeField] private float previewRatio;
        [SerializeField] private string baseFilename = "UbePhoto";
        [SerializeField] private GameObject permissionNotif;
        [SerializeField] private GameObject photoModeUI;
        [SerializeField] private Vector2 landscapeAspectRatio;

        public enum AspectRatio
        {
            Portrait,
            Landscape,
            Square
        }
        
        private int resWidth;
        private int resHeight;
        private Canvas canvas;
        private Slot<string> eventSlot;
        private Texture2D photoInMemory;
        private AspectRatio currentAspectRatio;

        public Vector2 LandscapeAspectRatio => landscapeAspectRatio;

        public void SetAspectRatio(AspectRatio newSelection)
        {
            currentAspectRatio = newSelection;
        }

        public void TakePhoto()
        {
            if (canvas == null)
            {
                canvas = transform.GetComponentInParent<Canvas>();
            }

            resWidth = Screen.width;
            resHeight = Screen.height;
            
            switch (currentAspectRatio)
            {
                case AspectRatio.Landscape:
                    resHeight = Mathf.RoundToInt(resWidth * landscapeAspectRatio.y / landscapeAspectRatio.x);
                    break;
                
                case AspectRatio.Square:
                    resHeight = resWidth;
                    break;
            }

            //hide UI, show watermark
            PhotoModeManager.Instance.WatermarkObject.SetActive(true);
            
            frameImage.gameObject.SetActive(false);
            photoModeUI.SetActive(false);
            FPSCounter.IsVisible = false;
            VersionDisplay.IsVisible = false;

            //take photo
            Destroy(photoInMemory);
            StartCoroutine(CaptureScreenFromCamera());
            GlobalNotifier.Instance.Trigger(PhotoModeEvent.TAKE_ID);
        }

        public void DisposePhoto()
        {
            frameImage.gameObject.SetActive(false);
            Destroy(photoInMemory);
        }

        private IEnumerator CaptureScreenFromCamera()
        {
            yield return new WaitForEndOfFrame();

            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            
            //crop texture
            var finalWidth = texture.width * 1f;
            var finalHeight = texture.height * 1f;

            switch (currentAspectRatio)
            {
                case AspectRatio.Landscape:
                    finalHeight = finalWidth * landscapeAspectRatio.y / landscapeAspectRatio.x;
                    break;
                
                case AspectRatio.Square:
                    finalHeight = finalWidth;
                    break;
            }

            var yOffset = Mathf.RoundToInt((texture.height - finalHeight) / landscapeAspectRatio.y);

            var pixels = texture.GetPixels(0, yOffset, 
                Mathf.RoundToInt(finalWidth), Mathf.RoundToInt(finalHeight));
            
            var finalTexture = new Texture2D(Mathf.RoundToInt(finalWidth), Mathf.RoundToInt(finalHeight));
            finalTexture.SetPixels(pixels);
            finalTexture.Apply();

            //return UI elements
            photoModeUI.SetActive(true);
            FPSCounter.IsVisible = true;
            VersionDisplay.IsVisible = true;
            PhotoModeManager.Instance.WatermarkObject.SetActive(false);

            var resizedDims = new Vector2(resWidth / previewRatio / canvas.scaleFactor, resHeight / previewRatio / canvas.scaleFactor);

            frameImage.gameObject.SetActive(true);

            frameImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, resizedDims.x + 20);
            frameImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, resizedDims.y + 20);

            photoHolder.texture = finalTexture;
            photoInMemory = finalTexture;
        }

        public void OpenSettings()
        {
            NativeGallery.OpenSettings();
        }

        public void SavePhoto()
        {
            var permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write);

            switch (permission)
            {
                case NativeGallery.Permission.Denied:
                    permissionNotif.SetActive(true);
                    return;
            }

            var filename = baseFilename + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            NativeGallery.SaveImageToGallery(photoInMemory, "Ube", filename, OnSavePhoto);
        }

        private void OnSavePhoto(bool success, string path)
        {
            if (success)
            {
                frameImage.gameObject.SetActive(false);
                Destroy(photoInMemory);
            }
        }

        private void StartedPhotoMode(IEvent<string> callback)
        {
            frameImage.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(OnPhotoModeStart.EVENT_NAME, StartedPhotoMode);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }
    }
}
