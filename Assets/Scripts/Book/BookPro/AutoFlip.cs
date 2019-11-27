using UnityEngine;
using System.Collections;
using System;

namespace BCity { 

    [RequireComponent(typeof(BookPro))]
    public class AutoFlip : MonoBehaviour
    {
        public BookPro ControledBook;
        public FlipMode Mode;
        public float PageFlipTime = 1;
        public float DelayBeforeStart;
        public float TimeBetweenPages=5;
        public bool AutoStartFlip=true;
        bool flippingStarted = false;
        bool isPageFlipping = false;
        float elapsedTime = 0;
        float nextPageCountDown = 0;
        // Use this for initialization
        void Start () {
            if (!ControledBook)
                ControledBook = GetComponent<BookPro>();
            ControledBook.interactable = false;
            if (AutoStartFlip)
                StartFlipping();
        }
        public void FlipRightPage()
        {
            if (isPageFlipping) {
                Debug.Log("翻页中，未翻页成功");
                return; 
            }
            if (ControledBook.CurrentPaper >= ControledBook.papers.Count) {
                Debug.Log("未进入翻页逻辑");
                return;
            }
            isPageFlipping = true;
            PageFlipper.FlipPage(ControledBook, PageFlipTime, FlipMode.RightToLeft, ()=> { isPageFlipping = false; });
            Debug.Log("进行翻页");


        }
        public void FlipLeftPage()
        {
            Debug.Log("Flip Left Page");


            if (isPageFlipping) return;
            if (ControledBook.CurrentPaper <= 0) return;
            isPageFlipping = true;
            PageFlipper.FlipPage(ControledBook, PageFlipTime, FlipMode.LeftToRight, () => { isPageFlipping = false; });
            Debug.Log("Left :  PageFlipper.FlipPage");

        }
        public void StartFlipping()
        {
            flippingStarted = true;
            elapsedTime = 0;
            nextPageCountDown = 0;
        }
        void Update()
        {
            if (flippingStarted)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > DelayBeforeStart)
                {
                    if (nextPageCountDown < 0)
                    {
                        if ((ControledBook.CurrentPaper <= ControledBook.EndFlippingPaper &&
                            Mode == FlipMode.RightToLeft) ||
                            (ControledBook.CurrentPaper > ControledBook.StartFlippingPaper &&
                            Mode == FlipMode.LeftToRight))
                        {
                            isPageFlipping = true;
                            PageFlipper.FlipPage(ControledBook, PageFlipTime, Mode, ()=> { isPageFlipping = false; });
                        }
                        else
                        {
                            flippingStarted = false;
                            this.enabled = false;
                        }

                        nextPageCountDown = PageFlipTime + TimeBetweenPages+ Time.deltaTime;
                    }
                    nextPageCountDown -= Time.deltaTime;
                }
            }
        }
    }
}