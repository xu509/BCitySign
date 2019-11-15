using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity
{

    public class PageRecord
    {
        int id;
        DateTime cdate;     // 签字日期时间
        string signAddress;     //  签名图地址
        string photoAddress;    //  拍照图地址

        public int Id { get => id; set => id = value; }
        public DateTime Cdate { get => cdate; set => cdate = value; }
        public string SignAddress { get => signAddress; set => signAddress = value; }
        public string PhotoAddress { get => photoAddress; set => photoAddress = value; }
    }

}