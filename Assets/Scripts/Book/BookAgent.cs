﻿using System.Collections;
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

        public void Init(FromSceneEnum fromSceneEnum) {

            // 获取数据
            IDaoService _daoManagerServ = GameObject.Find("Dao").GetComponent<DaoManager>().GetDaoService();
            List<PageRecord> list = _daoManagerServ.GetList(0, (int)_daoManagerServ.GetListTotal());
            //PageRecord record = list[0];
            //Debug.Log("record GetListTotal " + (int)_daoManagerServ.GetListTotal());
            //Debug.Log("record PhotoAddress " + record.PhotoAddress);
            //Debug.Log("record SignAddress " + record.SignAddress);

            // 初始化book组件
            if (fromSceneEnum == FromSceneEnum.Menu)
            {
                _bookPro.Init(list, 0);
            }
            else {
                _bookPro.Init(list, 0);
            }

            // 初始化滚动组件
            _scrollAreaAgent.Init(OnRecognizeDirection);

        }


        public void OnRecognizeDirection(ScrollDirectionEnum scrollDirectionEnum) {
            Debug.Log("识别方向 ");

            if (scrollDirectionEnum == ScrollDirectionEnum.Left)
            {
                _flipAgent.FlipRightPage();
                Debug.Log("下一张！");


            }
            else if (scrollDirectionEnum == ScrollDirectionEnum.Right)
            {
                _flipAgent.FlipLeftPage();
                Debug.Log("上一张");
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