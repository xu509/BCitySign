﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace BCity { 

    /// <summary>
    /// 签字版代理
    /// </summary>
    public class SignAgent : MonoBehaviour
    {
        [SerializeField] WritePadAgent _writePadAgent;

        private BCManager _manager;
        private MenuAgent _menuAgent;


        public void Init(MenuAgent menuAgent) {
            _menuAgent = menuAgent;
            _manager = GameObject.Find("MainBrain").GetComponent<BCManager>();
        }

        /// <summary>
        ///     打开
        /// </summary>
        public void Open() {
            GetComponent<Image>().DOFade(1f, 2f);
        }

        /// <summary>
        ///     关闭
        /// </summary>
        public void Close() { 
        
        }

       
        /// <summary>
        ///     点击拍照
        /// </summary>
        public void DoPhoto() {
            gameObject.SetActive(false);
            _menuAgent.OpenPhoto();
        }

        public void DoFinish() {

            // 保存
            DateTime dateTime = DateTime.Now;

            //_manager.daoManager.SaveTexture(dateTime,)

            // 保存签名
            var texture = _writePadAgent.GetTexture();
            _manager.daoManager.SaveTexture(dateTime, texture, SaveTextureType.Sign);

            gameObject.SetActive(false);
            _menuAgent.OpenAlbum(false);
        }

    }
}