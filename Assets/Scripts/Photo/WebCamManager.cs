using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BCity {

    public class WebCamManager : MonoBehaviour
    {
        [SerializeField,Header("显示画布")] RawImage photoRaw;
        [SerializeField, Header("倒计时数字")] Text _timerText;
        [SerializeField, Header("捕捉图片")] Image _capturePhotoImage;

        WebCamTexture camTexture;
        BCManager _bcManager;

        Action<Texture2D> _onPhoto;
        Action _onCountDownFinished;    // 倒计时结束回调
        Action _onCountDownStart;    // 倒计时结束回调
        Action _onInitError;    // 倒计时结束回调


        private int timer;



        /// <summary>
        ///     初始化WebCam
        /// </summary>
        public void Init(Action<Texture2D> onPhoto,Action onCountDownFinished, Action onCountDownStart,Action onInitError) {
            Debug.Log("网络摄像头模块启动");

            _onPhoto = onPhoto;
            _onCountDownFinished = onCountDownFinished;
            _onCountDownStart = onCountDownStart;
            _onInitError = onInitError;

            _bcManager = GameObject.Find("MainBrain").GetComponent<BCManager>();

            // 调用摄像头
            StartCoroutine(CallCamera());
            //CallCamera();



        }


        IEnumerator CallCamera() {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.Log("网络摄像头已启动成功");

                WebCamDevice[] devices = WebCamTexture.devices;

                if (devices.Length > 0)
                {
                    var deviceName = devices[0].name;
                    Debug.Log(devices[0].name);
                    camTexture = new WebCamTexture(deviceName, 1920, 1080, 30);
                    photoRaw.texture = camTexture;

                    if (!camTexture.isPlaying) {
                        camTexture.Play();
                    }                    

                    // 开始倒数
                    timer = 3;
                    _timerText.text = timer.ToString();

                    StartCountDown();
                }
                else {
                    Debug.Log("当前摄像头连接失败！");
                    _onInitError.Invoke();
                }

            }
        }

        void StartCountDown() {
            _onCountDownStart.Invoke();
            Invoke("setTimer", 1.0f);
            Invoke("setTimer", 2.0f);
            Invoke("setTimer", 3.0f);
        }

        void setTimer()
        {
            Debug.Log("setTimer");
            timer = timer - 1;
            _timerText.text = timer.ToString();
            if (timer == 0)
            {
                _onCountDownFinished.Invoke();
                // 进行拍照
                StartPhoto();
            }
        }

        /// <summary>
        /// 进行拍摄
        /// </summary>
        void StartPhoto() {

            Texture2D t2d = new Texture2D(camTexture.width, camTexture.height, TextureFormat.ARGB32, true);
            //将WebCamTexture 的像素保存到texture2D中

            t2d.SetPixels(camTexture.GetPixels());
            t2d.Apply();

            //texture.filterMode = FilterMode.Point;
            Sprite sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
            _capturePhotoImage.sprite = sprite;

            _capturePhotoImage.gameObject.SetActive(true);

            Debug.Log(camTexture.width + " - " + camTexture.height);
            
            _onPhoto.Invoke(sprite.texture);

        }


        public void DoRePhoto() {
            // 开始倒数
            timer = 3;
            _timerText.text = timer.ToString();
            _capturePhotoImage.gameObject.SetActive(false);

            StartCountDown();
        }

        public void StopCamera() {
            if (camTexture.isPlaying) {
                camTexture.Stop();
            }
        }



    }

}
