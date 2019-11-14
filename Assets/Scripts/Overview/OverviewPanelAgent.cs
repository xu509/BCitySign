using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace BCity { 
    public class OverviewPanelAgent : MonoBehaviour
    {
        MenuAgent _menuAgent;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }


        public void Init(MenuAgent menuAgent) {
            _menuAgent = menuAgent;
        }

        public void Open(Action onOpenCompleted) {
            gameObject.SetActive(true);

            GetComponent<Image>().DOFade(1, 0.5f).OnComplete(()=> {
                onOpenCompleted.Invoke();
            });

        }

        public void Close(Action onCloseCompleted) {
            GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() => {
                Destroy(gameObject);
                onCloseCompleted.Invoke();
            });
        }

        public void DoReturn() {
            Close(()=> {
                _menuAgent.ShowTool();
            });
        }


    }
}