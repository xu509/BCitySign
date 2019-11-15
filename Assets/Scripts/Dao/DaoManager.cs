using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        [SerializeField] TextureService _textureService;


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
        public ReqResult SaveTexture(DateTime dt, Texture texture, SaveTextureType saveTextureType) {
            int width = 600;
            int height = 600;

            string dataTime = dt.ToString("yyyy-MM-dd-HH-mm-ss");
                     
            string fileName = null;


            // 创建文件夹
            string fileDic = dataTime + "/";
            string fullFileDic = _fileDir + fileDic;

            if (!File.Exists(fullFileDic))
            {
                DirectoryInfo di = Directory.CreateDirectory(fullFileDic);
                Debug.Log("创建文件夹： " + fullFileDic);
            }

            Texture2D newPng;
            newPng = _textureService.ScaleTexture(texture, width, height);

            if (saveTextureType == SaveTextureType.Sign)
            {
                fileName = "sign";
            }
            else if (saveTextureType == SaveTextureType.Photo) {

                fileName = "photo";
            }

            string newFileName = _textureService.SaveBytesToFile(newPng.EncodeToPNG(), fullFileDic, fileName);

            return new ReqResult(ResultMessage.OK, fileDic + newFileName);
        }

    }


}
