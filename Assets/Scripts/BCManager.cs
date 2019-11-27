using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity {
    public class BCManager : MonoBehaviour
    {
        /// <summary>
        ///     屏保
        /// </summary>
        [SerializeField] ScreenProtectManager _screenProtectManager;
        [SerializeField] DaoManager _daoManager;


        /// <summary>
        /// 相册大小
        /// </summary>
        [SerializeField] int _albumSize;
        public int albumSize { get { return _albumSize; } }



        public ScreenProtectManager screenProtectManager { get { return _screenProtectManager; } }
        public DaoManager daoManager { get { return _daoManager; } }


        private MenuAgent _menuAgent;
        public MenuAgent menuAgent { set { _menuAgent = value; } get { return _menuAgent; } }


        // Start is called before the first frame update
        void Start()
        {
            _daoManager.Init();
            _screenProtectManager.Init();
            
        }

        // Update is called once per frame
        void Update()
        {            
            _screenProtectManager.Run();

        }
    }

}
