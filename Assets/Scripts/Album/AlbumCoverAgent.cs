using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity {

    public class AlbumCoverAgent : MonoBehaviour
    {

        private int _page;

        private BCManager _manager;
        private AlbumSetsAgent _albumSetsAgent;


        public void Init(DateTime start,DateTime end,int page, AlbumSetsAgent albumSetsAgent) {
            _manager = GameObject.Find("MainBrain").GetComponent<BCManager>();

            _page = page;

            _albumSetsAgent = albumSetsAgent;

            // 设置文字
        }



        public void OnClickAgent() {
            Debug.Log("OnClickAgent");
            _albumSetsAgent.OpenAlbum(_page);
        }


        
    }


}
