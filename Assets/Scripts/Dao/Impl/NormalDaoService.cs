using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity {

    public class NormalDaoService : MonoBehaviour, IDaoService
    {
        DaoDataSource _dataSource;
        BCManager _manager;

        void Start()
        {
            _dataSource = GameObject.Find("Dao").GetComponent<DaoDataSource>();
            _manager = GameObject.Find("MainBrain").GetComponent<BCManager>();
        }


        public void Add(PageRecord pageRecord)
        {
            var datas = _dataSource.GetData();
            datas.pageRecords.Add(pageRecord);

            Debug.Log("Add Page Records : " + datas.pageRecords.Count);

            _dataSource.UpdateXMLData();           
        }

        public List<PageRecord> GetList(int start, int size)
        {
            //var total = GetListTotal();
            var datas = _dataSource.GetData();



            if (start > datas.pageRecords.Count - 1) {
                return null;
            }

            int si = size;


            if ((start + size) > datas.pageRecords.Count - 1) {
                si = datas.pageRecords.Count - start;
            }

            var result = datas.pageRecords.GetRange(start, si);

            Debug.Log("DAO : start - " + start + " size - " + si);


            return result;            
        }

        public long GetListTotal()
        {
            var datas = _dataSource.GetData();
            long total = datas.pageRecords.Count;
            return total;
        }

        public void SavePhotoInfomation(DateTime dateTime, string imageUrl)
        {
            //_dataSource.GetData

            var datas = _dataSource.GetData();

            for (int i = 0; i < datas.pageRecords.Count; i++) {
                if (datas.pageRecords[i].Cdate == dateTime) {
                    Debug.Log("更新数据");
                    datas.pageRecords[i].PhotoAddress = imageUrl;
                    break;
                }
            }

            _dataSource.UpdateXMLData();

        }
    }
}