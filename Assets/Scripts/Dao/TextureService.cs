using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BCity
{
    public class TextureService : MonoBehaviour
    {

        public Texture2D ScaleTexture(Texture target, int width, int height)
        {

            RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

            Graphics.Blit(target, rt);

            Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, false);

            //var result = TextureResource.Instance.GetTexture(TextureResource.Write_Pad_Texture_Big) as Texture2D;
            //Texture2D result = null;

            //// 涂白底部
            for (int x = 0; x < result.width; x++)
            {
                for (int y = 0; y < result.height; y++)
                {
                    result.SetPixel(x, y, Color.white);
                }
            }

            result.name = "ScaleTextureResult";
            RenderTexture.active = rt;

            int desx = (result.width - width) / 2;
            int desy = (result.height - height) / 2;

            result.ReadPixels(new Rect(0, 0, width, height), desx, desy);

            // 去除所有的透明像素
            for (int x = 0; x < result.width; x++)
            {
                for (int y = 0; y < result.height; y++)
                {

                    Color currentColor = result.GetPixel(x, y);

                    if (currentColor != Color.white && currentColor != Color.black)
                    {
                        result.SetPixel(x, y, Color.white);
                    }

                }
            }

            result.Apply();
            RenderTexture.active = null;
            GameObject.Destroy(rt);
            rt = null;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dir"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string SaveBytesToFile(byte[] bytes, string dir, string filename)
        {            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir); // 这步耗时

            // 判断当前目录下有多少个文件，如超过上限则进行清理
            //获取文件信息
            DirectoryInfo direction = new DirectoryInfo(dir);

            // 如果文件数量超过100，则清空文件夹
            //if (files.Length > 100)
            //{
            //    // 清理文件夹下所有的文件
            //    for (int i = 0; i < files.Length; i++)
            //    {
            //        string fp = dir + "/" + files[i].Name;
            //        File.Delete(fp);
            //    }

            //}

            string path = dir + filename + ".png";
            FileStream file = File.Open(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);

            Debug.Log("创建文件： " + path);

            file.Close();

            return filename + ".png";

        }






    }
}