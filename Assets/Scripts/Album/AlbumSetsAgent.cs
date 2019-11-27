using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BCity
{
    /// <summary>
    ///     相册集合
    /// </summary>
    public class AlbumSetsAgent : MonoBehaviour
    {
        [SerializeField] AlbumCoverAgent _albumCoverAgentPrefab;
        [SerializeField] Transform _albumCoverContainer;

        private int gap = 730;

        // - 父组件
        private MenuAgent _menuAgent;

        private BCManager _manager;

        private long _recordsTotal;


        /// <summary>
        ///     打开
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);

            CreateComponents();

            //GetComponent<Image>().DOFade(1f, 2f).OnComplete(() => {
            //    CreateComponents();
            //});

        }

        /// <summary>
        ///  init data
        /// </summary>
        /// <param name="menuAgent"></param>
        public void Init(MenuAgent menuAgent)
        {
            _menuAgent = menuAgent;
            _manager = GameObject.Find("MainBrain").GetComponent<BCManager>();

            _recordsTotal = _manager.daoManager.GetDaoService().GetListTotal();
        }


        private void CreateComponents()
        {
            Debug.Log("创建组件！");

            int number = (int)_recordsTotal / _manager.albumSize;
            if (_recordsTotal % _manager.albumSize > 0)
            {
                number++;
            }

            Debug.Log("相册数量： " + number);

            for (int i = 0; i < number; i++)
            {
                var albumCoverAgent = Instantiate<AlbumCoverAgent>(_albumCoverAgentPrefab, _albumCoverContainer);
                Debug.Log("创建一个");
                Debug.Log(albumCoverAgent.GetComponent<RectTransform>().anchoredPosition);

                var position = albumCoverAgent.GetComponent<RectTransform>();

                albumCoverAgent.GetComponent<RectTransform>().anchoredPosition = position.anchoredPosition + new Vector2(gap * i, 0);
            }
        }


        /// <summary>
        ///     关闭
        /// </summary>
        public void Close()
        {
            Destroy(gameObject);
        }



        public void DoReturn()
        {
            Debug.Log("点击返回");

            Close();
            _menuAgent.Recover();
        }

    }
}