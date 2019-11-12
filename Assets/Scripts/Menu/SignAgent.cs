using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BCity { 

    /// <summary>
    /// 签字版代理
    /// </summary>
    public class SignAgent : MonoBehaviour
    {
        private MenuAgent _menuAgent;

        public void Init(MenuAgent menuAgent) {
            _menuAgent = menuAgent;
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
        
        
        }

        public void DoFinish() {
            gameObject.SetActive(false);
            _menuAgent.OpenAlbum(false);
        }



    }
}