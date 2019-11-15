using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;



namespace BCity
{ 
    public class DaoDataSource : MonoBehaviour
    {
        private PageRecordDataBase _likeDataBase;
        public PageRecordDataBase GetData() {
            return _likeDataBase;
        }


        public void Init() {
            try
            {
                // 查找XML，如果不存在就新建
                string path = Application.dataPath + "/BCityAsset/";
                string filename = "data.xml";
                string p = path + filename;

                bool isCreate = AppUtils.CreateFileIfNotExist(path, filename);
                if (isCreate)
                {
                    _likeDataBase = new PageRecordDataBase();
                    CreateXMLData();
                }
                else {

                }
                LoadDataFromXml();
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                Debug.LogError(e.Message);
                //throw new Exception("服务器连接失败：" + e.Message.ToString());
            }
        }


        public void CreateXMLData()
        {
            Debug.Log("创建XML");

            XmlSerializer serializer = new XmlSerializer(typeof(PageRecordDataBase));
            string path = Application.dataPath + "/BCityAsset/data.xml";
            _likeDataBase = new PageRecordDataBase();

            FileStream stream = new FileStream(path, FileMode.Create);
            serializer.Serialize(stream, _likeDataBase);
            stream.Close();
        }

        /// <summary>
        /// 更新xml数据
        /// </summary>
        public void UpdateXMLData() {
            XmlSerializer serializer = new XmlSerializer(typeof(PageRecordDataBase));
            string path = Application.dataPath + "/BCityAsset/data.xml";
            FileStream stream = new FileStream(path, FileMode.Create);
            serializer.Serialize(stream, _likeDataBase);
            stream.Close();
        }



        private void LoadDataFromXml() {
            XmlSerializer serializer = new XmlSerializer(typeof(PageRecordDataBase));
            FileStream stream = new FileStream(Application.dataPath + "/BCityAsset/data.xml", FileMode.Open);
            _likeDataBase = serializer.Deserialize(stream) as PageRecordDataBase;
            stream.Close();
        }



        [System.Serializable]
        public class PageRecordDataBase {

            [XmlArray("records")]
            public List<PageRecord> _pageRecords = new List<PageRecord>();

            public List<PageRecord> pageRecords { get { return _pageRecords; } }

        }



    }
}