﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System;
using BCity;
using System.Collections.Generic;
using System.IO;

public enum FlipMode1
{
    RightToLeft,
    LeftToRight
}
public class BookPro : MonoBehaviour
{
    Canvas canvas;
    [SerializeField]
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
    //[HideInInspector]
    public int currentPaper = 0;
    //[HideInInspector]
    public Paper[] papers;

    [SerializeField] GameObject paperFstPrefab;
    [SerializeField] GameObject paperFontPrefab;
    [SerializeField] GameObject paperBackPrefab;
    [SerializeField] GameObject paperEndPrefab;
    /// <summary>
    /// OnFlip invocation list, called when any page flipped
    /// </summary>
    public UnityEvent OnFlip;

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
                else
                    currentPaper = value;
                UpdatePages();
            }
        }
    }

    //public Paper[] updatePapersWithDaoList(List<PageRecord> list) {

    //}

    //[HideInInspector]
    public int StartFlippingPaper = 0;
    //[HideInInspector]
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

    // Use this for initialization
    void Start()
    {


    }

    private void InitData(List<PageRecord> datas)
    {
        //
        Debug.Log("此处初始化数据");
        Debug.Log("数据数量： " + datas.Count);

        GameObject page0 = GameObject.Find("Page0");
        GameObject page1 = GameObject.Find("Page1");
        GameObject page2 = GameObject.Find("Page2");
        GameObject page3 = GameObject.Find("Page3");

        //papers = new Paper[6];
        // Paper paper = new Paper();
        papers = new Paper[datas.Count + 1];

        for (int i = 0; i < (datas.Count + 1); i++)
        {
            Debug.Log("new Paper" + i);
            Paper paper = new Paper();
            Transform fontTransform = LeftPageTransform;
            // fontTransform.sizeDelta = new Vector2( yourWidth, yourHeight);
            Transform backTransform = RightPageTransform;

            if (i == 0)
            {
                paper.Front = GameObject.Instantiate(paperFstPrefab, transform);
                paper.Back = GameObject.Instantiate(paperFontPrefab, transform);
                BookLeftPage leftBook = paper.Back.GetComponent<BookLeftPage>();
                //BookRightPage rightBook = paper.Back.GetComponent<BookRightPage>();
                RawImage signImg = leftBook.signImg;
                PageRecord record = datas[0];
                signImg.texture = LoadImageByte(record.SignAddress); 
            }
            else if (i == datas.Count)
            {
                paper.Front = GameObject.Instantiate(paperBackPrefab, transform);
                paper.Back = GameObject.Instantiate(paperEndPrefab, transform);

                BookLeftPage leftBook = paper.Front.GetComponent<BookLeftPage>();
                BookRightPage rightBook = paper.Back.GetComponent<BookRightPage>();
                RawImage signImg = rightBook.signImg;
                PageRecord record = datas[0];
                signImg.texture = LoadImageByte(record.SignAddress); 

            }
            else
            {
                paper.Front = GameObject.Instantiate(paperBackPrefab, transform);
                paper.Back = GameObject.Instantiate(paperFontPrefab, transform);

                BookLeftPage leftBook = paper.Back.GetComponent<BookLeftPage>();
                BookRightPage rightBook = paper.Back.GetComponent<BookRightPage>();
                RawImage signImg = rightBook.signImg;
                PageRecord record = datas[0];
                signImg.texture = LoadImageByte(record.SignAddress); 
            }

            // paperFontPrefab
            // LogoAgent agent = GameObject.Instantiate(_logoAgentPrefab, _logoContainer);
            papers[i] = paper;
        }

        EndFlippingPaper = datas.Count;

        page0.SetActive(false);
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(false);


    }

    public static Texture2D LoadImageByte(string path)
    {
        FileStream files = new FileStream(Application.dataPath + "/BCityAsset/" + path, FileMode.Open, FileAccess.Read);
        files.Seek(0, SeekOrigin.Begin);
        byte[] imgByte = new byte[files.Length];

        //少量临时加载会 红问号 
        //files.BeginRead(imgByte,0,(int)files.Length,CallBack,files);

        files.Read(imgByte, 0, imgByte.Length);
        files.Close();

        Texture2D tx = new Texture2D(512, 512);
        tx.LoadImage(imgByte);
        return tx;
    }
    static void CallBack(IAsyncResult ar)
    {
        FileStream fileStream = ar.AsyncState as FileStream;
        fileStream.Close();
        fileStream.Dispose();
    }

    public void Init(List<PageRecord> datas)
    {
        InitData(datas);

        Debug.Log("Start papers.Length is " + papers.Length);
        Canvas[] c = GetComponentsInParent<Canvas>();
        if (c.Length > 0)
            canvas = c[c.Length - 1];
        else
            Debug.LogError("Book Must be a child to canvas diectly or indirectly");

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
    }



    /// <summary>
    /// transform point from global (world-space) to local space
    /// </summary>
    /// <param name="global">poit iin world space</param>
    /// <returns></returns>
    public Vector3 transformPoint(Vector3 global)
    {

        Debug.Log("transformPoint");
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
        Debug.Log("transformPointMousePosition");
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector3 mouseWorldPos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, canvas.planeDistance));
            Vector2 localPos = BookPanel.InverseTransformPoint(mouseWorldPos);

            return localPos;
        }
        else if (canvas.renderMode == RenderMode.WorldSpace)
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

    /// <summary>
    /// Update page orders
    /// This function should be called whenever the current page changed, the dragging of the page started or the page has been flipped
    /// </summary>
    public void UpdatePages()
    {
        Debug.Log(" Update Pages ! ");

        int previousPaper = pageDragging ? currentPaper - 2 : currentPaper - 1;

        Debug.Log("previousPaper is " + previousPaper);

        //Hide all pages
        for (int i = 0; i < papers.Length; i++)
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

            Debug.Log("papers.Length is " + papers.Length);

            //Show the front page of all next papers
            for (int i = papers.Length - 1; i >= currentPaper; i--)
            {
                BookUtility.ShowPage(papers[i].Front);
                papers[i].Front.transform.SetSiblingIndex(papers.Length - i + previousPaper);
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
            if (currentPaper <= papers.Length - 1)
            {
                BookUtility.ShowPage(papers[currentPaper].Front);
                papers[currentPaper].Front.transform.SetSiblingIndex(papers.Length - currentPaper + previousPaper);
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

            if (currentPaper < papers.Length)
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
        Debug.Log("OnMouseDragRightPage");
        if (interactable && !tweening)
        {

            DragRightPageToPoint(transformPointMousePosition(Input.mousePosition));
        }

    }
    public void DragRightPageToPoint(Vector3 point)
    {
        Debug.Log("DragRightPageToPoint");
        if (currentPaper > EndFlippingPaper) return;
        pageDragging = true;
        mode = FlipMode.RightToLeft;
        f = point;

        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
        currentPaper += 1;

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
        Debug.Log("OnMouseDragLeftPage");
        if (interactable && !tweening)
        {
            DragLeftPageToPoint(transformPointMousePosition(Input.mousePosition));

        }

    }
    public void DragLeftPageToPoint(Vector3 point)
    {
        Debug.Log("DragLeftPageToPoint");
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
        Debug.Log("OnMouseRelease");
        if (interactable)
            ReleasePage();
    }
    public void ReleasePage()
    {
        Debug.Log("OnMouseRelease");
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
        Debug.Log("Flip");
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
    }

    public void TweenForward()
    {
        if (mode == FlipMode.RightToLeft)
        {
            tweening = true;
            Tween.ValueTo(gameObject, f, ebl * 0.98f, 0.3f, TweenUpdate, () => {
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
        Debug.Log("UpdateBookRTLToPoint");
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
        Debug.Log("UpdateBookLTRToPoint");
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