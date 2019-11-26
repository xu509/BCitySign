﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BCity
{
    /// <summary>
    ///     相册代理
    /// </summary>
    public class AlbumAgent : MonoBehaviour
    {
        // - 父组件
        private MenuAgent _menuAgent;

        // - 子组件
        [SerializeField, Header("Book")] BookAgent _bookAgent;


        /// <summary>
        ///     打开
        /// </summary>
        public void Open(bool fromMenu)
        {
            GetComponent<Image>().DOFade(1f, 2f);
        }

        public void Init(MenuAgent menuAgent) {
            _menuAgent = menuAgent;

            _bookAgent.Init();

        }



        /// <summary>
        ///     关闭
        /// </summary>
        public void Close()
        {
            Destroy(gameObject);
        }



        public void DoLeft() {
            Debug.Log("上一页");
            _bookAgent.DoPreviousPage();
        }

        public void DoRight()
        {
            Debug.Log("下一页");
            _bookAgent.DoNextPage();
        }

        public void DoReturn()
        {
            Debug.Log("点击返回");

            Close();
            _menuAgent.Recover();
        }

    }
}