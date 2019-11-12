using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace BCity
{ 
    /// <summary>
    ///     拍照代理
    /// </summary>
    public class PhotoAgent : MonoBehaviour
    {
        MenuAgent _menuAgent;

        public void Init(MenuAgent menuAgent) {
            _menuAgent = menuAgent;
        }

        public void Open() {
            GetComponent<Image>().DOFade(1, 2f);
        }

        public void Close() {
            Destroy(gameObject);
        }


        public void DoFinish() {
            Close();
            _menuAgent.OpenAlbum(false);
        }

        public void DoRephoto()
        {
            Debug.Log("重拍");

        }

    }
}