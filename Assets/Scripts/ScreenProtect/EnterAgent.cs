using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace BCity { 
    public class EnterAgent : MonoBehaviour
    {
        [SerializeField] RectTransform _bgContainer;


        [SerializeField, Header("放大系数"),Range(0.5f,2f)] float scaleFactor;
        [SerializeField, Header("放大动画时间"), Range(0.5f, 20)] float scaleDurTime;
        [SerializeField, Header("透明系数"),Range(0f, 1f)] float fadeFactor;
        [SerializeField, Header("透明动画时间"), Range(0.5f, 20)] float fadeDurTime;

        float _startTime;

        // Start is called before the first frame update
        void Start()
        {
            _startTime = Time.time;

            //GetComponent<RectTransform>().transform.DOScale(scaleFactor, scaleDurTime).SetLoops(-1, LoopType.Yoyo);
            //GetComponent<Image>().DOFade(fadeFactor, fadeDurTime).SetLoops(-1, LoopType.Yoyo);
        }    

        // Update is called once per frame
        void Update()
        {
            var runTime = Time.time - _startTime;

            // 更改放大系数

            float stime = runTime % scaleDurTime;

            if ((stime/ scaleDurTime) <= 0.5)
            {
                float scaleMat = Mathf.Lerp(1, 1 * scaleFactor, stime / scaleDurTime);
                _bgContainer.transform.localScale = new Vector3(scaleMat, scaleMat, scaleMat);
            }
            else {
                float scaleMat = Mathf.Lerp(1 * scaleFactor,1, stime / scaleDurTime);
                _bgContainer.transform.localScale = new Vector3(scaleMat, scaleMat, scaleMat);
            }



            float ftime = runTime % fadeDurTime;

            if ((ftime / fadeDurTime) <= 0.5)
            {
                float fadeMat = Mathf.Lerp(1, 1 * fadeFactor, ftime / scaleDurTime);
                _bgContainer.transform.localScale = new Vector3(fadeMat, fadeMat, fadeMat);
            }
            else
            {
                float fadeMat = Mathf.Lerp(1 * fadeFactor, 1, ftime / scaleDurTime);
                _bgContainer.transform.localScale = new Vector3(fadeMat, fadeMat, fadeMat);
            }


        }
    }
}