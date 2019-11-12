using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity {

    /// <summary>
    ///     菜单代理类  
    /// </summary>
    public class MenuAgent : MonoBehaviour
    {
        [SerializeField,Header("Menu 容器")] RectTransform _menuContainer;
        [SerializeField,Header("相册")] AlbumAgent _albumAgentPrefab;
        [SerializeField] Transform _albumAgentContainer;
        [SerializeField,Header("签字板")] SignAgent _signAgentPrefab;
        [SerializeField] Transform _signAgentContainer;
        [SerializeField, Header("拍照")] PhotoAgent _photoAgentPrefab;
        [SerializeField] Transform _photoAgentContainer;

        bool _showalbum = false;
        bool _showSign = false;
        bool _showPhoto = false;

        AlbumAgent _albumAgent;
        SignAgent _signAgent;
        PhotoAgent _photoAgent;


        /// <summary>
        /// 签名
        /// </summary>
        public void DoSign() {
            Debug.Log("点击签名");
            _menuContainer.gameObject.SetActive(false);

            if (!_showSign) {
                if (_signAgent == null) {
                    _signAgent = Instantiate(_signAgentPrefab, _signAgentContainer);
                    _signAgent.Init(this);
                }

                _signAgent.Open();
                _showSign = true; 
            }
        }


        /// <summary>
        /// 查看相册
        /// </summary>
        public void DoCheckAlbum() {
            Debug.Log("点击查看相册");
            OpenAlbum(true);
        }

        public void Recover() {
            _albumAgent = null;
            _signAgent = null;
            _showalbum = false;
            _showSign = false;
            _menuContainer.gameObject.SetActive(true);
        }


        /// <summary>
        ///     打开相册
        /// </summary>
        public void OpenAlbum(bool fromMenu) {
            if (_showPhoto) {
                _showPhoto = false;
                _photoAgent = null;
            }

            if (_showSign)
            {
                _showSign = false;
                _signAgent = null;
            }


            _menuContainer.gameObject.SetActive(false);
            if (!_showalbum)
            {
                if (_albumAgent == null)
                {
                    _albumAgent = Instantiate(_albumAgentPrefab, _albumAgentContainer);
                    _albumAgent.Init(this);
                    Debug.Log("_albumAgent init");

                }
                _albumAgent.Open(true);
                _showalbum = true; ;
            }
        }

        public void OpenPhoto() {
            if (!_showPhoto) {
                if (_photoAgent == null)
                {
                    _photoAgent = Instantiate(_photoAgentPrefab, _photoAgentContainer);
                    _photoAgent.Init(this);
                }

                _photoAgent.Open();
                _showPhoto = true;
            }

        
        
        }

    }

}
