using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        [SerializeField, Header("园区概览")] OverviewPanelAgent _overviewPanelPrefab;
        [SerializeField] Transform _overviewContainer;

        bool _showMenuTool = false;
        bool _showAlbum = false;
        bool _showSign = false;
        bool _showPhoto = false;
        bool _showOverview = false;

        AlbumAgent _albumAgent;
        SignAgent _signAgent;
        PhotoAgent _photoAgent;
        OverviewPanelAgent _overviewAgent;




        public void Recover() {
            _albumAgent = null;
            _signAgent = null;
            _showAlbum = false;
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
            if (!_showAlbum)
            {
                if (_albumAgent == null)
                {
                    _albumAgent = Instantiate(_albumAgentPrefab, _albumAgentContainer);
                    _albumAgent.Init(this);
                    Debug.Log("_albumAgent init");

                }
                _albumAgent.Open(true);
                _showAlbum = true; ;
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


        /// <summary>
        ///     打开概览
        /// </summary>
        public void OpenOverview()
        {
            Debug.Log("打开概览");

            if (!_showOverview)
            {
                if (_overviewAgent == null)
                {
                    _overviewAgent = Instantiate(_overviewPanelPrefab, _overviewContainer);
                    _overviewAgent.Init(this);
                }

                _overviewAgent.Open(()=> { 
                    
                    // 关闭其他打开的面板
                
                });
                _showOverview = true;
                _menuContainer.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// 隐藏menu
        /// </summary>
        public void Hide() {
            _menuContainer.GetComponent<CanvasGroup>()
                .DOFade(0, 2F).OnComplete(()=> {
                    _menuContainer.gameObject.SetActive(false);
                });
        }

        /// <summary>
        /// 显示menu
        /// </summary>
        public void ShowTool()
        {
            if (!_menuContainer.gameObject.activeSelf) {
                _menuContainer.gameObject.SetActive(true);
            }

            // 清理显示标志位
            _showAlbum = false;
            _showSign = false;
            _showPhoto = false;
            _showOverview = false;


            _menuContainer.GetComponent<CanvasGroup>()
               .DOFade(1, 2F).OnComplete(() => {
                   //_menuContainer.gameObject.SetActive(false);
               });
        }


        #region 按钮事件

        /// <summary>
        /// 签名
        /// </summary>
        public void DoSign()
        {
            Debug.Log("点击签名");
            _menuContainer.gameObject.SetActive(false);

            if (!_showSign)
            {
                if (_signAgent == null)
                {
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
        public void DoCheckAlbum()
        {
            Debug.Log("点击查看相册");
            OpenAlbum(true);
        }

        /// <summary>
        ///     点击园区概览
        /// </summary>
        public void DoOverview() {
            Debug.Log("打开园区概览");
            OpenOverview();
        }
        #endregion



    }

}
