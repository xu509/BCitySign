using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity
{
    /// <summary>
    ///     屏幕管理器
    /// </summary>
    public class ScreenProtectManager : MonoBehaviour
    {
        [SerializeField] LogoAgent _logoAgentPrefab;
        [SerializeField] Transform _logoContainer;
        public Transform logoContainer { get { return _logoContainer; } }

        /// <summary>
        ///   操作框panel
        /// </summary>
        [SerializeField,Header("操作Panel")] Transform _opContainer;
        public Transform opContainer { get { return _opContainer; } }

        [SerializeField] MenuAgent _menuAgentPrefab;
        public MenuAgent menuAgentPrefab { get { return _menuAgentPrefab; } }


        [SerializeField] GoLeftDisplayBehavior _goLeftDisplayBehavior;

        [SerializeField,Range(0f,20f)] float _moveSpeed;
        public float moveSpeed { get { return _moveSpeed; } }



        private int _row;
        private int _column;

        private ScreenProtectStatus _screenProtectStatus;
        private BCManager _manager;
        

        public void Init() {
            _screenProtectStatus = ScreenProtectStatus.Prepare;
            _row = 0;
            _column = 0;

            _goLeftDisplayBehavior.Init(this);
            _manager = GameObject.Find("MainBrain").GetComponent<BCManager>();

        }

        public void Run() {
            if (_screenProtectStatus == ScreenProtectStatus.Prepare) {
                // 创建卡片
                _screenProtectStatus = ScreenProtectStatus.Init;

                CreateAgents();
                // 进行创建卡片
                _screenProtectStatus = ScreenProtectStatus.InitComplete;                
            }

            if (_screenProtectStatus == ScreenProtectStatus.InitComplete) {
                _screenProtectStatus = ScreenProtectStatus.Running;
            }

            if (_screenProtectStatus == ScreenProtectStatus.Running) {
                // 进行移动
                _goLeftDisplayBehavior.Run();
            }        
        }


        private void CreateAgents (){
            // 创建并且加满
            float x = 0, y = 0;

            var logoContainerWidth = _logoContainer.GetComponent<RectTransform>().rect.width;
            var logoContainerHeight = _logoContainer.GetComponent<RectTransform>().rect.height;

            while (y < logoContainerHeight) {
                if (x < logoContainerWidth)
                {
                    // 生成 agent
                    var gen_x = _column;


                    // 更新x    
                    // 更新row
                }
                else { 
                    // 更新y
                    // 更新column

                    // 归零x
                }
            }

            


            LogoAgent agent = GameObject.Instantiate(_logoAgentPrefab, _logoContainer);
            agent.GetComponent<RectTransform>().anchoredPosition = new Vector2(960,540);
        }







    }
}