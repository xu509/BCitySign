using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity
{
    /// <summary>
    /// 书刊代理
    /// </summary>
    public class BookAgent : MonoBehaviour
    {
        [SerializeField, Header("Book Pro")] BookPro _bookPro;
        [SerializeField] AutoFlip _flipAgent;

        [SerializeField, Header("Scroll")] ScrollAreaAgent _scrollAreaAgent;
        private BCManager _manager;

        public void Init(FromSceneEnum fromSceneEnum,int page) {

            Debug.Log("初始化bookagent ： " + fromSceneEnum);
            _manager = GameObject.Find("MainBrain").GetComponent<BCManager>();


            // 获取数据
            IDaoService _daoManagerServ = GameObject.Find("Dao").GetComponent<DaoManager>().GetDaoService();

            int size = _manager.albumSize;
            int start = (page - 1) * size;
            int total = (int)_daoManagerServ.GetListTotal();

            List<PageRecord> list = _daoManagerServ.GetList(start, size);


            //PageRecord record = list[0];
            //Debug.Log("record GetListTotal " + (int)_daoManagerServ.GetListTotal());
            //Debug.Log("record PhotoAddress " + record.PhotoAddress);
            //Debug.Log("record SignAddress " + record.SignAddress);

            // 初始化book组件
            if (fromSceneEnum == FromSceneEnum.Menu)
            {
                _bookPro.Init(list, 0);
            }
            else if (fromSceneEnum == FromSceneEnum.Photo)
            {
                _bookPro.Init(list, list.Count);
            }
            else if (fromSceneEnum == FromSceneEnum.Sign) {
                Debug.Log("从签名页面打开！  -> " + list.Count);
                _bookPro.Init(list, list.Count);
            }

            // 初始化滚动组件
            _scrollAreaAgent.Init(OnRecognizeDirection);

        }


        public void OnRecognizeDirection(ScrollDirectionEnum scrollDirectionEnum) {
            if (scrollDirectionEnum == ScrollDirectionEnum.Left)
            {
                _flipAgent.FlipRightPage();


            }
            else if (scrollDirectionEnum == ScrollDirectionEnum.Right)
            {
                _flipAgent.FlipLeftPage();
            }
        }

        public void DoPreviousPage() {

            _flipAgent.FlipRightPage();
        }

        public void DoNextPage() {
            _flipAgent.FlipLeftPage();
        }

    }
}