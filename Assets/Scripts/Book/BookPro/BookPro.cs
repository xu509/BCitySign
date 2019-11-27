using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;


namespace BCity {

    public enum FlipMode1
    {
        RightToLeft,
        LeftToRight
    }

    public class BookPro : MonoBehaviour
    {
        [SerializeField, Header("Prefab - Left")] BookPageAgent _bookPageAgentLeftPrefab;
        [SerializeField, Header("Prefab - Right")] BookPageAgent _bookPageAgentRightPrefab;

        [SerializeField] int _size;


        private int _turnToPage;
        List<PageRecord> _pageRecords;
        private bool _isPaged = false;


        Canvas canvas;
        [SerializeField,Header("In Plugin")]
        RectTransform BookPanel;
        public Image ClippingPlane;
        public Image Shadow;
        public Image LeftPageShadow;
        public Image RightPageShadow;
        public Image ShadowLTR;
        public RectTransform LeftPageTransform;
        public RectTransform RightPageTransform;
        public bool interactable = true;
        public bool enableShadowEffect = true;
        [Tooltip("Uncheck this if the book does not contain transparent pages to improve the overall performance")]
        public bool hasTransparentPages = true;
        [HideInInspector]
        public int currentPaper = 0;
        [HideInInspector]
        public List<Paper> papers;
        /// <summary>
        /// OnFlip invocation list, called when any page flipped
        /// </summary>
        public UnityEvent OnFlip;


        public Button btnPrev;
        public Button btnNext;

        /// <summary>
        /// The Current Shown paper (the paper its front shown in right part)
        /// </summary>
        public int CurrentPaper
        {
            get { return currentPaper; }
            set
            {
                if (value != currentPaper)
                {
                    if (value < StartFlippingPaper)
                        currentPaper = StartFlippingPaper;
                    else if (value > EndFlippingPaper + 1)
                        currentPaper = EndFlippingPaper + 1;
                    else {
                        Debug.Log("更新索引至 -》 " + value);

                        currentPaper = value;

                    }
                    
                    UpdatePages();
                }
            }
        }
        [HideInInspector]
        public int StartFlippingPaper = 0;
        [HideInInspector]
        public int EndFlippingPaper = 1;

        public Vector3 EndBottomLeft
        {
            get { return ebl; }
        }
        public Vector3 EndBottomRight
        {
            get { return ebr; }
        }
        public float Height
        {
            get
            {
                return BookPanel.rect.height;
            }
        }

        Image Left;
        Image Right;

        //current flip mode
        FlipMode mode;

        /// <summary>
        /// this value should e true while the user darg the page
        /// </summary>
        bool pageDragging = false;

        /// <summary>
        /// should be true when the page tween forward or backward after release
        /// </summary>
        bool tweening = false;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageRecords"></param>
        /// <param name="toPage">目标页码</param>
        /// <param name="page">第几本书</param>
        public void Init(List<PageRecord> pageRecords,int toPage) {
            _pageRecords = pageRecords;
            _turnToPage = toPage;

            if (pageRecords == null)
            {
                Debug.Log("当前书本插件未加载到数据");
            }
            else {
                Debug.Log("当前书本插件加载到 ： " + pageRecords.Count + " 条数据");
                InitComponents();
            }
        }


        // Use this for initialization
        private void InitComponents()
        {            
            Canvas[] c = GetComponentsInParent<Canvas>();
            if (c.Length > 0)
                canvas = c[c.Length - 1];
            else
                Debug.LogError("Book Must be a child to canvas diectly or indirectly");


            InitPages();

            UpdatePages();

            CalcCurlCriticalPoints();


            float pageWidth = BookPanel.rect.width / 2.0f;
            float pageHeight = BookPanel.rect.height;
        

            ClippingPlane.rectTransform.sizeDelta = new Vector2(pageWidth * 2 + pageHeight, pageHeight + pageHeight * 2);

            //hypotenous (diagonal) page length
            float hyp = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
            float shadowPageHeight = pageWidth / 2 + hyp;

            Shadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            Shadow.rectTransform.pivot = new Vector2(1, (pageWidth / 2) / shadowPageHeight);

            ShadowLTR.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            ShadowLTR.rectTransform.pivot = new Vector2(0, (pageWidth / 2) / shadowPageHeight);

            RightPageShadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            RightPageShadow.rectTransform.pivot = new Vector2(0, (pageWidth / 2) / shadowPageHeight);

            LeftPageShadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            LeftPageShadow.rectTransform.pivot = new Vector2(1, (pageWidth / 2) / shadowPageHeight);

            Debug.Log("currentPaper : " + currentPaper);
            Debug.Log("papers length : " + papers.Count);


            Debug.Log("加载组件完成");

            TurnToPages(_turnToPage);

        }

        /// <summary>
        /// transform point from global (world-space) to local space
        /// </summary>
        /// <param name="global">poit iin world space</param>
        /// <returns></returns>
        public Vector3 transformPoint(Vector3 global)
        {
            Vector2 localPos = BookPanel.InverseTransformPoint(global);
            return localPos;
        }
        /// <summary>
        /// transform mouse position to local space
        /// </summary>
        /// <param name="mouseScreenPos"></param>
        /// <returns></returns>
        public Vector3 transformPointMousePosition(Vector3 mouseScreenPos)
        {
            if(canvas.renderMode== RenderMode.ScreenSpaceCamera )
            {
                Vector3 mouseWorldPos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, canvas.planeDistance));
                Vector2 localPos = BookPanel.InverseTransformPoint(mouseWorldPos);

                return localPos;
            }
            else if ( canvas.renderMode == RenderMode.WorldSpace)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 globalEBR = transform.TransformPoint(ebr);
                Vector3 globalEBL = transform.TransformPoint(ebl);
                Vector3 globalSt = transform.TransformPoint(st);
                Plane p = new Plane(globalEBR, globalEBL, globalSt);
                float distance;
                p.Raycast(ray, out distance);
                Vector2 localPos = BookPanel.InverseTransformPoint(ray.GetPoint(distance));
                return localPos;
            }
            else
            {
                //Screen Space Overlay
                Vector2 localPos = BookPanel.InverseTransformPoint(mouseScreenPos);
                return localPos;
            }
        
        }


        void updateButton(){
            var recordsCount = _pageRecords.Count;
            if (currentPaper == 0)
            {
                btnPrev.interactable = false;
            } else {
                btnPrev.interactable = true;

            }
            if (currentPaper == _pageRecords.Count + 1)
            {
                btnNext.interactable = false;
            }
            else {
                btnNext.interactable = true;

            }

        }
        /// <summary>
        /// Update page orders
        /// This function should be called whenever the current page changed, the dragging of the page started or the page has been flipped
        /// </summary>
        public void UpdatePages()
        {
            updateButton();
            int previousPaper = pageDragging ? currentPaper - 2 : currentPaper - 1;

            //Hide all pages
            for (int i = 0; i < papers.Count; i++)
            {
                BookUtility.HidePage(papers[i].Front);
                papers[i].Front.transform.SetParent(BookPanel.transform);
                BookUtility.HidePage(papers[i].Back);
                papers[i].Back.transform.SetParent(BookPanel.transform);
            }

            if (hasTransparentPages)
            {
                //Show the back page of all previous papers
                for (int i = 0; i <= previousPaper; i++)
                {
                    BookUtility.ShowPage(papers[i].Back);
                    papers[i].Back.transform.SetParent(BookPanel.transform);
                    papers[i].Back.transform.SetSiblingIndex(i);
                    BookUtility.CopyTransform(LeftPageTransform.transform, papers[i].Back.transform);
                }

                //Show the front page of all next papers
                for (int i = papers.Count - 1; i >= currentPaper; i--)
                {
                    BookUtility.ShowPage(papers[i].Front);
                    papers[i].Front.transform.SetSiblingIndex(papers.Count - i + previousPaper);
                    BookUtility.CopyTransform(RightPageTransform.transform, papers[i].Front.transform);
                }

            }
            else
            {
                //show back of previous page only
                if (previousPaper >= 0)
                {
                    BookUtility.ShowPage(papers[previousPaper].Back);
                    //papers[previousPaper].Back.transform.SetParent(BookPanel.transform);
                    //papers[previousPaper].Back.transform.SetSiblingIndex(previousPaper);
                    BookUtility.CopyTransform(LeftPageTransform.transform, papers[previousPaper].Back.transform);
                }
                //show front of current page only
                if (currentPaper <= papers.Count - 1)
                {
                    BookUtility.ShowPage(papers[currentPaper].Front);
                    papers[currentPaper].Front.transform.SetSiblingIndex(papers.Count - currentPaper + previousPaper);
                    BookUtility.CopyTransform(RightPageTransform.transform, papers[currentPaper].Front.transform);

                }
            }
            #region Shadow Effect
            if (enableShadowEffect)
            {
                //the shadow effect enabled
                if (previousPaper >= 0)
                {
                    //has at least one previous page, then left shadow should be active
                    LeftPageShadow.gameObject.SetActive(true);
                    LeftPageShadow.transform.SetParent(papers[previousPaper].Back.transform, true);
                    LeftPageShadow.rectTransform.anchoredPosition = new Vector3();
                    LeftPageShadow.rectTransform.localRotation = Quaternion.identity;
                }
                else
                {
                    //if no previous pages, the leftShaow should be disabled
                    LeftPageShadow.gameObject.SetActive(false);
                    LeftPageShadow.transform.SetParent(BookPanel, true);
                }

                if (currentPaper < papers.Count)
                {
                    //has at least one next page, the right shadow should be active
                    RightPageShadow.gameObject.SetActive(true);
                    RightPageShadow.transform.SetParent(papers[currentPaper].Front.transform, true);
                    RightPageShadow.rectTransform.anchoredPosition = new Vector3();
                    RightPageShadow.rectTransform.localRotation = Quaternion.identity;
                }
                else
                {
                    //no next page, the right shadow should be diabled
                    RightPageShadow.gameObject.SetActive(false);
                    RightPageShadow.transform.SetParent(BookPanel, true);
                }
            }
            else
            {
                //Enable Shadow Effect is Unchecked, all shadow effects should be disabled
                LeftPageShadow.gameObject.SetActive(false);
                LeftPageShadow.transform.SetParent(BookPanel, true);

                RightPageShadow.gameObject.SetActive(false);
                RightPageShadow.transform.SetParent(BookPanel, true);
            
            }
            #endregion
        }

  
        //mouse interaction events call back
        public void OnMouseDragRightPage()
        {
            if (interactable && !tweening)
            {
          
                DragRightPageToPoint(transformPointMousePosition(Input.mousePosition));
            }

        }
        public void DragRightPageToPoint(Vector3 point)
        {
            if (currentPaper > EndFlippingPaper) {
                return;
            }
            
            pageDragging = true;
            mode = FlipMode.RightToLeft;
            f = point;

            ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
            currentPaper += 1;
            Debug.Log("更新当前页码索引");

            UpdatePages();

            Left = papers[currentPaper - 1].Front.GetComponent<Image>();
            BookUtility.ShowPage(Left.gameObject);
            Left.rectTransform.pivot = new Vector2(0, 0);
            Left.transform.position = RightPageTransform.transform.position;
            Left.transform.localEulerAngles = new Vector3(0, 0, 0);

            Right = papers[currentPaper - 1].Back.GetComponent<Image>();
            BookUtility.ShowPage(Right.gameObject);
            Right.transform.position = RightPageTransform.transform.position;
            Right.transform.localEulerAngles = new Vector3(0, 0, 0);
        
            if (enableShadowEffect) Shadow.gameObject.SetActive(true);
            ClippingPlane.gameObject.SetActive(true);

            UpdateBookRTLToPoint(f);
        }
        public void OnMouseDragLeftPage()
        {
            if (interactable && !tweening)
            {
                DragLeftPageToPoint(transformPointMousePosition(Input.mousePosition));

            }

        }
        public void DragLeftPageToPoint(Vector3 point)
        {
            if (currentPaper <= StartFlippingPaper) return;
            pageDragging = true;
            mode = FlipMode.LeftToRight;
            f = point;

            UpdatePages();

            ClippingPlane.rectTransform.pivot = new Vector2(0, 0.35f);

            Right = papers[currentPaper - 1].Back.GetComponent<Image>();
            BookUtility.ShowPage(Right.gameObject);
            Right.transform.position = LeftPageTransform.transform.position;
            Right.transform.localEulerAngles = new Vector3(0, 0, 0);
            Right.transform.SetAsFirstSibling();

            Left = papers[currentPaper - 1].Front.GetComponent<Image>();
            BookUtility.ShowPage(Left.gameObject);
            Left.gameObject.SetActive(true);
            Left.rectTransform.pivot = new Vector2(1, 0);
            Left.transform.position = LeftPageTransform.transform.position;
            Left.transform.localEulerAngles = new Vector3(0, 0, 0);


            if (enableShadowEffect) ShadowLTR.gameObject.SetActive(true);
            ClippingPlane.gameObject.SetActive(true);
            UpdateBookLTRToPoint(f);
        }
        public void OnMouseRelease()
        {
            if (interactable )
                ReleasePage();
        }
        public void ReleasePage()
        {
            if (pageDragging)
            {
                pageDragging = false;
                float distanceToLeft = Vector2.Distance(c, ebl);
                float distanceToRight = Vector2.Distance(c, ebr);
                if (distanceToRight < distanceToLeft && mode == FlipMode.RightToLeft)
                    TweenBack();
                else if (distanceToRight > distanceToLeft && mode == FlipMode.LeftToRight)
                    TweenBack();
                else
                    TweenForward();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("currentPaper : " + currentPaper + " | pages size : " + papers.Count);

            if (pageDragging && interactable)
            {
                UpdateBook();                
            }
        }
        public void UpdateBook()
        {
            f = Vector3.Lerp(f, transformPointMousePosition(Input.mousePosition), Time.deltaTime * 10);
            if (mode == FlipMode.RightToLeft)
                UpdateBookRTLToPoint(f);
            else
                UpdateBookLTRToPoint(f);
        }
    
        /// <summary>
        /// This function called when the page dragging point reached its distenation after releasing the mouse
        /// This function will call the OnFlip invocation list
        /// if you need to call any fnction after the page flipped just add it to the OnFlip invocation list
        /// </summary>
        public void Flip()
        {
            pageDragging = false;

            if (mode == FlipMode.LeftToRight)
                currentPaper -= 1;
            //Debug.Log(currentPaper);
            Left.transform.SetParent(BookPanel.transform, true);
            Left.rectTransform.pivot = new Vector2(0, 0);
            Right.transform.SetParent(BookPanel.transform, true);
            UpdatePages();
            Shadow.gameObject.SetActive(false);
            ShadowLTR.gameObject.SetActive(false);
            ClippingPlane.gameObject.SetActive(false);
            if (OnFlip != null)
                OnFlip.Invoke();

            Debug.Log("翻页效果结束 currentPaper ： " + currentPaper);

            if (_isPaged) {

                //RefreshPagePool();

                //int to;

                //if (mode == FlipMode.LeftToRight)
                //{
                //    to = currentPaper + 1;
                //}
                //else
                //{
                //    to = currentPaper - 1;
                //}

                //Debug.Log("翻页至 currentPaper ： " + to);

                //TurnToPages(currentPaper);
            }


        }

        public void TweenForward()
        {
            if (mode == FlipMode.RightToLeft)
            {
                tweening = true;
                Tween.ValueTo(gameObject, f, ebl * 0.98f, 0.3f, TweenUpdate, ()=> {
                    Flip();
                    tweening = false;
                });
            }
            else
            {
                tweening = true;
                Tween.ValueTo(gameObject, f, ebr * 0.98f, 0.3f, TweenUpdate, () => {
                    Flip();
                    tweening = false;
                });
            }
        }
        void TweenUpdate(Vector3 follow)
        {
            if (mode == FlipMode.RightToLeft)
                UpdateBookRTLToPoint(follow);
            else
                UpdateBookLTRToPoint(follow);
        }

        public void TweenBack()
        {
            if (mode == FlipMode.RightToLeft)
            {
                tweening = true;
                Tween.ValueTo(gameObject, f, ebr * 0.98f, 0.3f, TweenUpdate, () =>
                {
                    currentPaper -= 1;
                    Right.transform.SetParent(BookPanel.transform);
                    Left.transform.SetParent(BookPanel.transform);
                    //pageDragging = false;
                    tweening = false;
                    Shadow.gameObject.SetActive(false);
                    ShadowLTR.gameObject.SetActive(false);
                    UpdatePages();
                });
            }
            else
            {
                tweening = true;
                Tween.ValueTo(gameObject, f, ebl * 0.98f, 0.3f, TweenUpdate, () =>
                {
                    Left.transform.SetParent(BookPanel.transform);
                    Right.transform.SetParent(BookPanel.transform);
                    //pageDragging = false;
                    tweening = false;
                    Shadow.gameObject.SetActive(false);
                    ShadowLTR.gameObject.SetActive(false);
                    UpdatePages();
                });
            }
        }

        /// <summary>
        ///     初始化pages
        /// </summary>
        private void InitPages()
        {
            var recordsCount = _pageRecords.Count;

            // 当没有内容时
            if (recordsCount == 0)
            {
                // 创建第一页
                var coverPaper = CreateCover();
                coverPaper.GetComponent<RectTransform>().SetSiblingIndex(0);

                var pageLeft = Instantiate<BookPageAgent>(_bookPageAgentLeftPrefab, transform);
                pageLeft.Init(null, false, true);
                pageLeft.GetComponent<RectTransform>().SetSiblingIndex(1);

                AddPage(coverPaper.gameObject, pageLeft.gameObject);

                // 创建第二页
                var pageRight = Instantiate<BookPageAgent>(_bookPageAgentRightPrefab, transform);
                pageRight.Init(null, false, true);
                pageRight.GetComponent<RectTransform>().SetSiblingIndex(2);

                var coverBackPage = CreateCoverBack();
                coverBackPage.GetComponent<RectTransform>().SetSiblingIndex(3);

                AddPage(pageRight.gameObject, coverBackPage.gameObject);
            }
            else if (recordsCount > 0) {
                    
                int size;
                bool hasOverSize = false;
                _isPaged = false;

                if (recordsCount > _size)
                {
                    size = _size;
                    hasOverSize = true;
                }
                else {
                    size = recordsCount;
                }

                for (int i = 0; i < size + 1; i++) {
                    Paper paper = new Paper();
                    papers.Add(paper);                    
                }


                EndFlippingPaper = size;

                if (hasOverSize)
                {
                    _isPaged = true;
                    Debug.Log("进入分页逻辑");
                    RefreshPagePool();


                    Debug.Log("papers size : " + papers.Count);
                    Debug.Log("_pageRecords size : " + _pageRecords.Count);

                }
                // 未超出容量
                else
                {
                    _isPaged = false;
                    Debug.Log("未分页逻辑");
                    Debug.Log("papers size : " + papers.Count);
                    Debug.Log("_pageRecords size : " + _pageRecords.Count);

                    // 创建第一页
                    var coverPaper = CreateCover();
                    coverPaper.GetComponent<RectTransform>().SetSiblingIndex(0);
                    papers[0].Front = coverPaper;
                    coverPaper.gameObject.name = "paper-cover";


                    for (int i = 0; i < _pageRecords.Count; i++) {
                        var data = _pageRecords[i];

                        var pageLeft = Instantiate<BookPageAgent>(_bookPageAgentLeftPrefab, transform);
                        pageLeft.Init(data, false, false);
                        pageLeft.GetComponent<RectTransform>().SetSiblingIndex(2 * i + 1);
                        papers[i].Back = pageLeft.gameObject;
                        pageLeft.gameObject.name = "paper-" + (2 * i + 1);
                        Debug.Log("Paper - " + i + " => back is => ##" + pageLeft.gameObject.name);


                        var pageRight = Instantiate<BookPageAgent>(_bookPageAgentRightPrefab, transform);
                        pageRight.Init(data, false, false);
                        pageRight.GetComponent<RectTransform>().SetSiblingIndex(2 * i + 2);
                        papers[i + 1].Front = pageRight.gameObject;
                        pageRight.gameObject.name = "paper-" + (2 * i + 2);

                        Debug.Log("Paper - " + (i+1) + " => Front is => ##" + pageRight.gameObject.name);
                    }


                    var coverBack = CreateCoverBack();
                    coverBack.GetComponent<RectTransform>().SetSiblingIndex(papers.Count * 2 + 1);
                    coverBack.gameObject.name = "paper-back";

                    papers[papers.Count - 1].Back = coverBack.gameObject;
                    Debug.Log("Paper - " + (papers.Count - 1) + " back is 封底");
                }

            }
        }

        /// <summary>
        ///     翻页至page
        /// </summary>
        /// <param name="page"></param>
        private void TurnToPages(int page) {

            currentPaper = page;
            UpdatePages();

            //if (page != 0) {

            //}           
        }


        /// <summary>
        ///     创建封面
        /// </summary>
        private GameObject CreateCover() {
            var cover = Instantiate<BookPageAgent>(_bookPageAgentRightPrefab,transform);
            cover.Init(null,true,false);
            cover.GetComponent<RectTransform>().SetSiblingIndex(0);
            
            return cover.gameObject;
        }

        /// <summary>
        ///     创建封底
        /// </summary>
        private GameObject CreateCoverBack()
        {
            var cover = Instantiate<BookPageAgent>(_bookPageAgentLeftPrefab, transform);
            cover.Init(null, true, false);

            //papers
            return cover.gameObject;
        }


        private void AddPage(GameObject front,GameObject back) {
            Paper paper = new Paper();
            paper.Front = front;
            paper.Back = back;
            papers.Add(paper);
        }


        private void RefreshPagePool() {
            // 当前页码 ： _turnToPage currentpage
            // 大小 ： _size

            Debug.Log("currentPaper In Refresh Page Pool : " + currentPaper);


            // 获取创建的起始坐标
            int startIndex = currentPaper - Mathf.CeilToInt(_size / 2f);
            if (startIndex < 0) {
                startIndex = 0;
            }

            if ((currentPaper + Mathf.CeilToInt(_size / 2f)) >= _pageRecords.Count) {
                startIndex = _pageRecords.Count - _size;
            }

            Debug.Log("起始坐标 ： " + startIndex);

            // 判断是否要生成封面
            //if (startIndex == 0) {
            var coverPaper = CreateCover();
            coverPaper.GetComponent<RectTransform>().SetSiblingIndex(0);
            papers[0].Front = coverPaper;
            coverPaper.gameObject.name = "paper-cover";
            //}

            for (int i = 0; i < _size; i++)
            {
                int dataIndex = i + startIndex;

                var data = _pageRecords[dataIndex];

                var pageLeft = Instantiate<BookPageAgent>(_bookPageAgentLeftPrefab, transform);
                pageLeft.Init(data, false, false);
                pageLeft.GetComponent<RectTransform>().SetSiblingIndex(2 * i + 1);
                papers[i].Back = pageLeft.gameObject;
                pageLeft.gameObject.name = "paper-" + (2 * i + 1);
                Debug.Log("Paper - " + i + " => back is => ##" + pageLeft.gameObject.name);

                var pageRight = Instantiate<BookPageAgent>(_bookPageAgentRightPrefab, transform);
                pageRight.Init(data, false, false);
                pageRight.GetComponent<RectTransform>().SetSiblingIndex(2 * i + 2);
                papers[i + 1].Front = pageRight.gameObject;
                pageRight.gameObject.name = "paper-" + (2 * i + 2);
                
            }

            // 创建封底
            var coverBack = CreateCoverBack();
            coverBack.GetComponent<RectTransform>().SetSiblingIndex(papers.Count * 2 + 1);
            coverBack.gameObject.name = "paper-back";
            papers[papers.Count - 1].Back = coverBack.gameObject;


            //Debug.Log("_turnToPage : " + _turnToPage);
            //TurnToPages(_turnToPage);


        }






        #region Page Curl Internal Calculations
        //for more info about this part please check this link : http://rbarraza.com/html5-canvas-pageflip/

        float radius1, radius2;
        //Spine Bottom
        Vector3 sb;
        //Spine Top
        Vector3 st;
        //corner of the page
        Vector3 c;
        //Edge Bottom Right
        Vector3 ebr;
        //Edge Bottom Left
        Vector3 ebl;
        //follow point 
        Vector3 f;
    
        private void CalcCurlCriticalPoints()
        {
            sb = new Vector3(0, -BookPanel.rect.height / 2);
            ebr = new Vector3(BookPanel.rect.width / 2, -BookPanel.rect.height / 2);
            ebl = new Vector3(-BookPanel.rect.width / 2, -BookPanel.rect.height / 2);
            st = new Vector3(0, BookPanel.rect.height / 2);
            radius1 = Vector2.Distance(sb, ebr);
            float pageWidth = BookPanel.rect.width / 2.0f;
            float pageHeight = BookPanel.rect.height;
            radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
        }
        public void UpdateBookRTLToPoint(Vector3 followLocation)
        {
            mode = FlipMode.RightToLeft;
            f = followLocation;
            if (enableShadowEffect)
            {
                Shadow.transform.SetParent(ClippingPlane.transform, true);
                Shadow.transform.localPosition = new Vector3(0, 0, 0);
                Shadow.transform.localEulerAngles = new Vector3(0, 0, 0);

                ShadowLTR.transform.SetParent(Left.transform);
                ShadowLTR.rectTransform.anchoredPosition = new Vector3();
                ShadowLTR.transform.localEulerAngles = Vector3.zero;
                ShadowLTR.gameObject.SetActive(true);
            }
            Right.transform.SetParent(ClippingPlane.transform, true);

            Left.transform.SetParent(BookPanel.transform, true);
            c = Calc_C_Position(followLocation);
            Vector3 t1;
            float T0_T1_Angle = Calc_T0_T1_Angle(c, ebr, out t1);
            if (T0_T1_Angle >= -90) T0_T1_Angle -= 180;

            ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
            ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
            ClippingPlane.transform.position = BookPanel.TransformPoint(t1);


            RightPageShadow.transform.localEulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
            RightPageShadow.transform.position = BookPanel.TransformPoint(t1);

            //page position and angle
            Right.transform.position = BookPanel.TransformPoint(c);
            float C_T1_dy = t1.y - c.y;
            float C_T1_dx = t1.x - c.x;
            float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
            Right.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - (T0_T1_Angle + 90));

            Left.transform.SetParent(ClippingPlane.transform, true);
            Left.transform.SetAsFirstSibling();

            Shadow.rectTransform.SetParent(Right.rectTransform, true);
        }
        public void UpdateBookLTRToPoint(Vector3 followLocation)
        {
            mode = FlipMode.LeftToRight;
            f = followLocation;
            if (enableShadowEffect)
            {
                ShadowLTR.transform.SetParent(ClippingPlane.transform, true);
                ShadowLTR.transform.localPosition = new Vector3(0, 0, 0);
                ShadowLTR.transform.localEulerAngles = new Vector3(0, 0, 0);

                Shadow.transform.SetParent(Right.transform);
                Shadow.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
                Shadow.transform.localEulerAngles = Vector3.zero;
                Shadow.gameObject.SetActive(true);
            }
            Left.transform.SetParent(ClippingPlane.transform, true);
            Right.transform.SetParent(BookPanel.transform, true);

            c = Calc_C_Position(followLocation);
            Vector3 t1;
            float T0_T1_Angle = Calc_T0_T1_Angle(c, ebl, out t1);
            if (T0_T1_Angle < 0) T0_T1_Angle += 180;

            ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
            ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

            LeftPageShadow.transform.localEulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
            LeftPageShadow.transform.position = BookPanel.TransformPoint(t1);

            //page position and angle
            Left.transform.position = BookPanel.TransformPoint(c);
            float C_T1_dy = t1.y - c.y;
            float C_T1_dx = t1.x - c.x;
            float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
            Left.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - 180 - (T0_T1_Angle - 90));

            Right.transform.SetParent(ClippingPlane.transform, true);
            Right.transform.SetAsFirstSibling();

            ShadowLTR.rectTransform.SetParent(Left.rectTransform, true);
        }
        private float Calc_T0_T1_Angle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
        {
            Vector3 t0 = (c + bookCorner) / 2;
            float T0_CORNER_dy = bookCorner.y - t0.y;
            float T0_CORNER_dx = bookCorner.x - t0.x;
            float T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);
            float T0_T1_Angle = 90 - T0_CORNER_Angle;

            float T1_X = t0.x - T0_CORNER_dy * Mathf.Tan(T0_CORNER_Angle);
            T1_X = normalizeT1X(T1_X, bookCorner, sb);
            t1 = new Vector3(T1_X, sb.y, 0);
            ////////////////////////////////////////////////
            //clipping plane angle=T0_T1_Angle
            float T0_T1_dy = t1.y - t0.y;
            float T0_T1_dx = t1.x - t0.x;
            T0_T1_Angle = Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;
            return T0_T1_Angle;
        }
        private float normalizeT1X(float t1, Vector3 corner, Vector3 sb)
        {
            if (t1 > sb.x && sb.x > corner.x)
                return sb.x;
            if (t1 < sb.x && sb.x < corner.x)
                return sb.x;
            return t1;
        }
        private Vector3 Calc_C_Position(Vector3 followLocation)
        {
            Vector3 c;
            f = followLocation;
            float F_SB_dy = f.y - sb.y;
            float F_SB_dx = f.x - sb.x;
            float F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
            Vector3 r1 = new Vector3(radius1 * Mathf.Cos(F_SB_Angle), radius1 * Mathf.Sin(F_SB_Angle), 0) + sb;

            float F_SB_distance = Vector2.Distance(f, sb);
            if (F_SB_distance < radius1)
                c = f;
            else
                c = r1;
            float F_ST_dy = c.y - st.y;
            float F_ST_dx = c.x - st.x;
            float F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);
            Vector3 r2 = new Vector3(radius2 * Mathf.Cos(F_ST_Angle),
               radius2 * Mathf.Sin(F_ST_Angle), 0) + st;
            float C_ST_distance = Vector2.Distance(c, st);
            if (C_ST_distance > radius2)
                c = r2;
            return c;
        }
        #endregion

    }
    [Serializable]
    public class Paper
    {
        public GameObject Front;
        public GameObject Back;
    }


    public static class BookUtility
    {
        /// <summary>
        /// Call this function to Show a Hidden Page
        /// </summary>
        /// <param name="page">the page to be shown</param>
        public static void ShowPage(GameObject page)
        {
            CanvasGroup cgf = page.GetComponent<CanvasGroup>();
            cgf.alpha = 1;
            cgf.blocksRaycasts = true;
        }

        /// <summary>
        /// Call this function to hide any page
        /// </summary>
        /// <param name="page">the page to be hidden</param>
        public static void HidePage(GameObject page)
        {
            CanvasGroup cgf = page.GetComponent<CanvasGroup>();
            cgf.alpha = 0;
            cgf.blocksRaycasts = false;
            page.transform.SetAsFirstSibling();
        }

        public static void CopyTransform(Transform from, Transform to)
        {
            to.position = from.position;
            to.rotation = from.rotation;
            to.localScale = from.localScale;

        }
    }
    
}