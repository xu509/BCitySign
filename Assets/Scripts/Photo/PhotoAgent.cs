using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace BCity
{ 
    /// <summary>
    ///     拍照代理
    /// </summary>
    public class PhotoAgent : MonoBehaviour
    {
        MenuAgent _menuAgent;

        [SerializeField] WebCamManager _webCamManager;

        DateTime _dateTime;
        BCManager _bcManager;

        Texture2D _photo; //拍摄的图片
        bool _isCountDowning;
        bool _hasPhotoed;

        public void Init(MenuAgent menuAgent,DateTime dateTime) {
            _menuAgent = menuAgent;
            _dateTime = dateTime;
            _isCountDowning = false;

            _bcManager = GameObject.Find("MainBrain").GetComponent<BCManager>();

            Debug.Log("拍摄模块启动");


        }

        public void Open() {
            GetComponent<Image>().DOFade(1, 2f)
                .OnComplete(()=> {
                    try
                    {
                        _webCamManager.Init(OnWebCameraPhoto, OnCountDownFinished, OnCountDownStart, OnInitError);
                    }
                    catch (Exception ex) { 
                    //ex.Message
                        }
                    
                });
        }


        public void Close() {
            Destroy(gameObject);
        }


        public void DoFinish() {
            _webCamManager.StopCamera();

            // 保存图片
            var result = _bcManager.daoManager.SavePhotoTexture(_dateTime, _photo);

            //更新数据
            string photoUrl = (string)result.GetData();
            _bcManager.daoManager.GetDaoService().SavePhotoInfomation(_dateTime, photoUrl);

            Close();
            _menuAgent.OpenAlbum(FromSceneEnum.Photo);
        }

        public void DoRephoto()
        {
            _webCamManager.DoRePhoto();

        }


        public void OnWebCameraPhoto(Texture2D texture) {
            _photo = texture;
            _hasPhotoed = true;
        }

        public void OnCountDownFinished()
        {
            _isCountDowning = false;
        }

        public void OnCountDownStart()
        {                        
            _isCountDowning = true;
        }

        public void OnInitError() { 
        
        }


    }
}