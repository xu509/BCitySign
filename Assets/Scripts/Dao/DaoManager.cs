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

            Texture2D newPng = null;

            if (saveTextureType == SaveTextureType.Sign)
            {
                fileName = "sign";
                newPng = _textureService.ScaleTexture(texture, width, height);

            }
            else if (saveTextureType == SaveTextureType.Photo) {

                fileName = "photo";
                //newPng = _textureService.ScaleTexture(texture, texture.width, texture.height);
                
                //newPng = _textureService.ScaleTexture(texture, texture.width, texture.height);

            }
            if (newPng != null) {
                string newFileName = _textureService.SaveBytesToFile(newPng.EncodeToPNG(), fullFileDic, fileName);
                return new ReqResult(ResultMessage.OK, fileDic + newFileName);
            }
            return null;
        }

        public ReqResult SavePhotoTexture(DateTime dt, Texture2D texture)
        {
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


            fileName = "photo";
            string newFileName = _textureService.SaveBytesToFile(texture.EncodeToPNG(), fullFileDic, fileName);
            return new ReqResult(ResultMessage.OK, fileDic + newFileName);
        }


        public Sprite GetImageSprite(string path) {

            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(_fileDir + path))
            {
                fileData = File.ReadAllBytes(_fileDir + path);
                tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);
                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Clamp;

                bool t = tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            else
            {
                string str = "File is not found : " + (_fileDir + path);
            }

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            return sprite;
        }



    }


}
