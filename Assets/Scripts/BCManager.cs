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

        public ScreenProtectManager screenProtectManager { get { return _screenProtectManager; } }

        // Start is called before the first frame update
        void Start()
        {
            _screenProtectManager.Init();
        }

        // Update is called once per frame
        void Update()
        {
            _screenProtectManager.Run();

        }
    }

}
