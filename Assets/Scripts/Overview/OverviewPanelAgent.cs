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
            GetComponent<Image>().DOFade(0, Time.deltaTime).OnComplete(() => {
                Destroy(gameObject);
                onCloseCompleted.Invoke();
            });
        }

        public void choseF1(){
            //introImgF1.
            /*introImgF2.gameObject.SetActive(false);
            introImgF1.gameObject.SetActive(true);
            return;*/

            introImgF1.GetComponent<CanvasGroup>().alpha = 0;
            introImgF1.gameObject.SetActive(true);
            introImgF2.gameObject.SetActive(false);
            introImgF1.GetComponent<CanvasGroup>().DOFade(1, 1f);
        }

        public void choseF2(){

            /*introImgF1.gameObject.SetActive(false);
            introImgF2.gameObject.SetActive(true);
            return;*/

            introImgF2.GetComponent<CanvasGroup>().alpha = 0;
            introImgF2.gameObject.SetActive(true);
            introImgF1.gameObject.SetActive(false);
            introImgF2.GetComponent<CanvasGroup>().DOFade(1, 1f);
            /*
            introImgF1.GetComponent<CanvasGroup>().alpha = 1;
            introImgF2.GetComponent<CanvasGroup>().alpha = 1;
            introImgF1.transform.SetAsFirstSibling();
            introImgF1.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => {
                introImgF2.transform.SetAsFirstSibling();
                introImgF1.GetComponent<CanvasGroup>().alpha = 1;

            });*/
        }

        public void DoReturn() {
            Close(()=> {
                _menuAgent.ShowTool();
            });
        }


    }
}