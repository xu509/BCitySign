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

            var result = datas.pageRecords.GetRange(start, size);
            return result;            
        }

        public long GetListTotal()
        {
            var datas = _dataSource.GetData();
            long total = datas.pageRecords.Count;
            return total;
        }
    }
}