using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity
{
    public class GoLeftDisplayBehavior : MonoBehaviour
    {
        ScreenProtectManager _screenProtectManager;

        public void Init(ScreenProtectManager screenProtectManager) {
            _screenProtectManager = screenProtectManager;
        }

        public void Run() {
            _screenProtectManager.logoContainer.Translate(new Vector3(0 - _screenProtectManager.moveSpeed * Time.deltaTime, 0, 0));
        }

        
    }

}
