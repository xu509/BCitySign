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

    public enum ScrollDirectionEnum
    {
        Left, Right, Top, Bottom
    }

    public class AlbumAgent : MonoBehaviour
    {
        private MenuAgent _menuAgent;

        [SerializeField] ScrollAreaAgent _scrollAreaAgent;
        [SerializeField] BookPro _bookPro;

        [SerializeField] AutoFlip _flipAgent;

        /// <summary>
        ///     打开
        /// </summary>
        public void Open(bool fromMenu)
        {
            GetComponent<Image>().DOFade(1f, 2f);
        }

        public void Init(MenuAgent menuAgent) {
            _menuAgent = menuAgent;
            _scrollAreaAgent.Init(OnRecognizeDirection);

            initData(); 
        }

        private void initData() {
            IDaoService _daoManagerServ = GameObject.Find("Dao").GetComponent<DaoManager>().GetDaoService();
            List<PageRecord> list = _daoManagerServ.GetList(0, (int)_daoManagerServ.GetListTotal());
            PageRecord record = list[0];
            Debug.Log("record GetListTotal " + (int)_daoManagerServ.GetListTotal());
            Debug.Log("record PhotoAddress " + record.PhotoAddress);
            Debug.Log("record SignAddress " + record.SignAddress);

            _bookPro.InitData(list);
        }


        void OnRecognizeDirection(ScrollDirectionEnum scrollDirectionEnum) {
            if (scrollDirectionEnum == ScrollDirectionEnum.Left){
                _flipAgent.FlipRightPage();
            }
            else if(scrollDirectionEnum == ScrollDirectionEnum.Right){
                _flipAgent.FlipLeftPage();

            }
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