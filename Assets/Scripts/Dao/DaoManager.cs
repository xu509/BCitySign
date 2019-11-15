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


        private string _fileDir;
        public string fileDir { get { return _fileDir; } }


        public void Init() {
            _daoDataSource.Init();

            _fileDir = Application.dataPath + "/BCityAsset/";
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

        /// <summary>
        ///     持久化 texture 文件
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public ReqResult SaveTexture(Texture texture) {
            float width = 600f;
            float height = 600f;





            return null;
        }

    }


}
