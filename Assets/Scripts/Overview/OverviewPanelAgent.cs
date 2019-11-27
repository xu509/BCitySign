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
        public Image introImgF1;
        public Image introImgF2;

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

        public void choseF1(){

            introImgF1.gameObject.SetActive(true);
            introImgF2.gameObject.SetActive(false);
            return;
            introImgF2.transform.SetAsFirstSibling();
            introImgF2.DOFade(0, 0.5f).OnComplete(() => {
                
            });
        }

        public void choseF2(){

            introImgF1.gameObject.SetActive(false);
            introImgF2.gameObject.SetActive(true);
            return;
            introImgF1.gameObject.SetActive(false);
            introImgF1.transform.SetAsFirstSibling();
            introImgF1.DOFade(0, 0.5f).OnComplete(() => {
                
            });
        }

        public void DoReturn() {
            Close(()=> {
                _menuAgent.ShowTool();
            });
        }


    }
}