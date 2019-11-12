using System.Collections;
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
        private MenuAgent _menuAgent;


        /// <summary>
        ///     打开
        /// </summary>
        public void Open(bool fromMenu)
        {
            GetComponent<Image>().DOFade(1f, 2f);
        }

        public void Init(MenuAgent menuAgent) {
            _menuAgent = menuAgent;
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
        }

        public void DoRight()
        {
            Debug.Log("下一页");
        }

        public void DoReturn()
        {
            Debug.Log("点击返回");

            Close();
            _menuAgent.Recover();
        }

    }
}