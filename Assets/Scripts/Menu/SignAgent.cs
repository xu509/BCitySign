using System.Collections;
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

        private DateTime _signDateTime;


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
            Destroy(gameObject);
        }

       
        /// <summary>
        ///     点击拍照
        /// </summary>
        public void DoPhoto() {
            gameObject.SetActive(false);

            // 此时保存签名
            SaveSign();

            _menuAgent.OpenPhoto(_signDateTime);
        }

        public void DoFinish() {

            // 保存
            SaveSign();


            gameObject.SetActive(false);
            _menuAgent.OpenAlbum(0,true);
        }


        public void DoReturn() {
            _menuAgent.ShowTool();
            Close();
        }


        /// <summary>
        ///     保存签名
        /// </summary>
        private void SaveSign() {
            DateTime dateTime = DateTime.Now;
            _signDateTime = dateTime;

            PageRecord pageRecord = new PageRecord();
            pageRecord.Cdate = dateTime;

            // 保存签名
            var texture = _writePadAgent.GetTexture();
            var result = _manager.daoManager.SaveTexture(dateTime, texture, SaveTextureType.Sign);
            string signAddress = (string)result.GetData();
            pageRecord.SignAddress = signAddress;

            _manager.daoManager.GetDaoService().Add(pageRecord);
        }





    }
}