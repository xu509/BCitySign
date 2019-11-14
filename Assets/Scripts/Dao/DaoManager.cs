using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity
{
    /// <summary>
    ///     数据模块的使用： https://www.yuque.com/docs/share/f0dac93c-98be-4b29-9491-0f5f30fab7b8#
    /// </summary>
    public class DaoManager : MonoBehaviour
    {
        [SerializeField] DaoSourceEnum _daoSourceEnum;
        [SerializeField] NormalDaoService _normalDaoService;
        [SerializeField] DaoDataSource _daoDataSource;


        public void Init() {
            _daoDataSource.Init();

            //GameObject.Find("Dao").GetComponent<DaoManager>().GetDaoService();

        }

        public void Run() { }


        public IDaoService GetDaoService() {
            if (_daoSourceEnum == DaoSourceEnum.Normal)
            {
                return _normalDaoService;
            }
            else {
                return null;
            }
        }

    }


}
