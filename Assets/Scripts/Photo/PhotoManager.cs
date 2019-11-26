using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;  

namespace BCity { 
    public class PhotoManager : MonoBehaviour
    {
    public string deviceName;
    public bool isClick;//是否点击了按钮
    public bool isCaptured;//是否截屏

    public Image photoCanvas;

    public RawImage photoRaw;
    public Image icon;

    public Text timeText;
    private int timerInt;

    WebCamTexture camTexture;
    // Use this for initialization
    IEnumerator Start()
    {
        isCaptured = false;
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            deviceName = devices[0].name;
            Debug.Log(devices[0].name);
            camTexture = new WebCamTexture(deviceName, 400, 300, 20);
            photoRaw.texture = camTexture;
            camTexture.Play();

            isClick = true; 
       }

        timerInt = 3;
        timeText.text = timerInt.ToString();
        //Invoke("setTimer", 1.0f);
        //Invoke("setTimer", 2.0f);
        //Invoke("setTimer", 3.0f);
    }

    void setTimer() {
        Debug.Log("setTimer");
        timerInt = timerInt - 1;
        timeText.text = timerInt.ToString();
        if (timerInt == 0) {

            timeText.text = "";
            isClick = false;

            Save(camTexture);
            Debug.Log("photoFinish");
        }
    }


    //通过GUI绘制摄像头要显示的窗口
    private void OnGUI()
    {
    }

    public void Save(WebCamTexture t)
    {
        Texture2D t2d = new Texture2D(t.width,t.height,TextureFormat.ARGB32,true);
       //将WebCamTexture 的像素保存到texture2D中
        t2d.SetPixels(t.GetPixels());
        //t2d.ReadPixels(new Rect(200,200,200,200),0,0,false);
        t2d.Apply();
//编码
        byte[] imageTytes = t2d.EncodeToJPG();

        // t2d


        // cameraTexture

        Sprite temp = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0, 0));  
        // transform.sprite = temp;  

        // photoCanvas.GetComponent<Renderer>().enabled = true;
        //photoCanvas.sprite = temp;
        photoRaw.texture = t2d; 

        //存储
        File.WriteAllBytes(Application.dataPath + "/BCityAsset/" + Time.time + ".jpg", imageTytes);
    }

    public void photoFinish() {
        Invoke("setTimer", 1.0f);
        Invoke("setTimer", 2.0f);
        Invoke("setTimer", 3.0f);

        // icon.transform.SetAsLastSibling();
    }

    public void rePhoto() {
        // photoCanvas.GetComponent<Renderer>().enabled = false;
        isClick = true;
        isCaptured = false;
    }
    
    }


}