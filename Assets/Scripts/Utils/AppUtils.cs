using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BCity
{ 
    public class AppUtils : MonoBehaviour
    {
        public static float ConvertToFloat(string str)
        {
            float r = 0f;

            if (!float.TryParse(str, out r))
            {
                r = 0f;
            }
            return r;
        }

        public static bool CreateFileIfNotExist(string path, string filename)
        {
            bool isCreate = false;


            string p = path + filename;

            //Debug.Log("p : " + p);


            if (!File.Exists(@p))
            {
                Debug.Log("CreateFileIfNotExist : " + p);
                DirectoryInfo di = Directory.CreateDirectory(path);


                isCreate = true;

                FileStream fs = File.Create(p);
                fs.Close();


            }

            return isCreate;
        }

    }
}