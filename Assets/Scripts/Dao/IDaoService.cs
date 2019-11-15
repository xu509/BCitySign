using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BCity
{

    public interface IDaoService 
    {
        void Add(PageRecord pageRecord);

        List<PageRecord> GetList(int start, int size);

        long GetListTotal();

    }

}