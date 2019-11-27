using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading;
using System;

namespace BCity
{
    public class WritePadAgent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        // [SerializeField] WritePanelConfig writePanelConfig;
        [SerializeField] RawImage raw;                   //使用UGUI的RawImage显示，方便进行添加UI,将pivot设为(0.5,0.5)
        [SerializeField] Material mat;     //给定的shader新建材质
        [SerializeField] Texture brushTypeTexture;   //画笔纹理，半透明
        [SerializeField] Color brushColor = Color.black;
        [SerializeField] int num = 50;

        [SerializeField, Header("毛刺效果")] bool _bushEffect;


        private RenderTexture texRender;   //画布

        private float brushScale = 0.5f;
        private float lastDistance;
        private Vector3[] PositionArray = new Vector3[3];
        private int a = 0;
        private Vector3[] PositionArray1 = new Vector3[4];
        private int b = 0;
        private float[] speedArray = new float[4];
        private int s = 0;

        Vector2 rawMousePosition;            //raw图片的左下角对应鼠标位置 
        float rawWidth;                               //raw图片宽度
        float rawHeight;                              //raw图片长度

        private WriteStatus _writeStatus = WriteStatus.Init;   //  书写状态
        private float _lastWriteTime = 0f;  //  最近的书写时间点

        //[SerializeField] private float _recognizeIntervalTime = 2f; // 识别周期


        // 灵云识别相关
        private List<short> _letterData;    //笔记数据
        private Vector2 _lastWriterPoint = Vector2.zero;


        //  设置该笔记的中心点
        private Vector2 _middlePoint;



        // 书写状态
        private enum WriteStatus
        {
            Init,   //  复位
            Writing,    // 手写中
            WritingPause,   // 手写暂停
            WriteFinished,  // 手写结束
            RecognizeStart,    // 开始识别
            Recognizing,    // 识别中
            RecognizeFinished    // 识别结束

        }


        void Start()
        {
            Debug.Log("WritePadAgent Start");

            //raw图片鼠标位置，宽度计算
            rawWidth = raw.rectTransform.rect.width;
            rawHeight = raw.rectTransform.rect.height;

            _middlePoint = new Vector2(rawWidth / 2, rawHeight / 2);

            UpdateRawMousePosition();

            texRender = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            Clear(texRender);

            DrawImage();

        }

        public void ClearPad() {
            rawWidth = raw.rectTransform.rect.width;
            rawHeight = raw.rectTransform.rect.height;
            texRender = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            Clear(texRender);
            DrawImage();

        }

        Vector3 startPosition = Vector3.zero;
        Vector3 endPosition = Vector3.zero;
        void FixedUpdate()
        {
            float now = Time.time;

            if (_writeStatus == WriteStatus.Writing)
            {

            }


            if (_writeStatus == WriteStatus.WritingPause && ((now - _lastWriteTime) > 2f))
            {
                _writeStatus = WriteStatus.WriteFinished;

            }
            if (_writeStatus == WriteStatus.WriteFinished)
            {
                // 开始确认
                _writeStatus = WriteStatus.Init;
            }


        }

        #region 手写功能相关

        //设置画笔宽度
        float SetScale(float distance)
        {
            float Scale = 0;
            if (distance < 100)
            {
                Scale = 0.8f - 0.005f * distance;
            }
            else
            {
                Scale = 0.425f - 0.00125f * distance;

            }
            if (Scale <= 0.05f)
            {
                Scale = 0.05f;
            }
            return Scale;
        }

        void OnMouseMove(Vector3 pos)
        {
            if (startPosition == Vector3.zero)
            {
                startPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            }

            endPosition = pos;
            float distance = Vector3.Distance(startPosition, endPosition);

            brushScale = SetScale(distance);

            // 此时存在没有动过的点，scale = 0.8f

            //if (distance == 0f) {
            //    Debug.Log("On Mouse Move Drag Exists， brushScale : " + brushScale);
            //}


            ThreeOrderBézierCurse(pos, distance, 4.5f);

            startPosition = endPosition;
            lastDistance = distance;
        }

        void OnMouseUp()
        {
            startPosition = Vector3.zero;
            //brushScale = 0.5f;
            a = 0;
            b = 0;
            s = 0;

            UpdateRawMousePosition();
        }

        void Clear(RenderTexture destTexture)
        {
            Graphics.SetRenderTarget(destTexture);
            GL.PushMatrix();
            GL.Clear(true, true, Color.clear);
            GL.PopMatrix();
        }

        #region 绘图实现（方法与贝塞尔曲线）
        void DrawBrush(RenderTexture destTexture, int x, int y, Texture sourceTexture, Color color, float scale)
        {
            DrawBrush(destTexture, new Rect(x, y, sourceTexture.width, sourceTexture.height), sourceTexture, color, scale);
        }
        void DrawBrush(RenderTexture destTexture, Rect destRect, Texture sourceTexture, Color color, float scale)
        {
            //Debug.Log("scale : " + scale);


            //增加鼠标位置根据raw图片位置换算。
            float left = (destRect.xMin - rawMousePosition.x) * Screen.width / rawWidth - destRect.width * scale / 2.0f;
            float right = (destRect.xMin - rawMousePosition.x) * Screen.width / rawWidth + destRect.width * scale / 2.0f;
            float top = (destRect.yMin - rawMousePosition.y) * Screen.height / rawHeight - destRect.height * scale / 2.0f;
            float bottom = (destRect.yMin - rawMousePosition.y) * Screen.height / rawHeight + destRect.height * scale / 2.0f;

            Graphics.SetRenderTarget(destTexture);

            GL.PushMatrix();
            GL.LoadOrtho();

            mat.SetTexture("_MainTex", brushTypeTexture);
            mat.SetColor("_Color", color);
            mat.SetPass(0);

            GL.Begin(GL.QUADS);

            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(left / Screen.width, top / Screen.height, 0);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(right / Screen.width, top / Screen.height, 0);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(right / Screen.width, bottom / Screen.height, 0);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(left / Screen.width, bottom / Screen.height, 0);


            GL.End();
            GL.PopMatrix();
        }
        bool bshow = true;


        
        //三阶贝塞尔曲线，获取连续4个点坐标，通过调整中间2点坐标，
        //画出部分（我使用了num/1.5实现画出部分曲线）来使曲线平滑;通过速度控制曲线宽度。
        private void ThreeOrderBézierCurse(Vector3 pos, float distance, float targetPosOffset)
        {
            //记录坐标
            PositionArray1[b] = pos;
            b++;
            //记录速度
            speedArray[s] = distance;
            s++;
            if (b == 4)
            {
                Vector3 temp1 = PositionArray1[1];
                Vector3 temp2 = PositionArray1[2];

                //修改中间两点坐标
                Vector3 middle = (PositionArray1[0] + PositionArray1[2]) / 2;
                PositionArray1[1] = (PositionArray1[1] - middle) * 1.5f + middle;
                middle = (temp1 + PositionArray1[3]) / 2;
                PositionArray1[2] = (PositionArray1[2] - middle) * 2.1f + middle;

                for (int index1 = 0; index1 < num / 1.5f; index1++)
                {
                    float t1 = (1.0f / num) * index1;
                    Vector3 target = Mathf.Pow(1 - t1, 3) * PositionArray1[0] +
                                     3 * PositionArray1[1] * t1 * Mathf.Pow(1 - t1, 2) +
                                     3 * PositionArray1[2] * t1 * t1 * (1 - t1) + PositionArray1[3] * Mathf.Pow(t1, 3);
                    //float deltaspeed = (float)(distance - lastDistance) / num;
                    //获取速度差值（存在问题，参考）
                    float deltaspeed = (float)(speedArray[3] - speedArray[0]) / num;

                    //模拟毛刺效果
                    float randomOffset;

                    // randomOffset = UnityEngine.Random.Range(-targetPosOffset, targetPosOffset);

                    if (_bushEffect)
                    {
                        randomOffset = UnityEngine.Random.Range(-targetPosOffset, targetPosOffset);
                    }
                    else
                    {
                        randomOffset = 0;
                    }

                    // 调用
                    DrawBrush(
                        texRender,
                        (int)(target.x + randomOffset),
                        (int)(target.y + randomOffset),
                        brushTypeTexture,
                        brushColor,
                        SetScale(speedArray[0] + (deltaspeed * index1)));
                }

                PositionArray1[0] = temp1;
                PositionArray1[1] = temp2;
                PositionArray1[2] = PositionArray1[3];

                speedArray[0] = speedArray[1];
                speedArray[1] = speedArray[2];
                speedArray[2] = speedArray[3];
                b = 3;
                s = 3;
            }
            else
            {
                DrawBrush(
                    texRender,
                    (int)endPosition.x,
                    (int)endPosition.y,
                    brushTypeTexture,
                    brushColor,
                    brushScale);
            }

        }

        #endregion

        void DrawImage()
        {
            raw.texture = texRender;
        }



        #region Event Trigger(拖动、点击)

        public void OnBeginDrag(PointerEventData eventData)
        {
            //OnMouseUp();
            //OnMouseMove(new Vector3(eventData.position.x, eventData.position.y, 0));
            _writeStatus = WriteStatus.Init;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //OnMouseMove(new Vector3(eventData.position.x, eventData.position.y, 0));
            OnMouseUp();
            //AddLetterDataEnd();
            _lastWriteTime = Time.time;
            _writeStatus = WriteStatus.WritingPause;

        }

        public void OnDrag(PointerEventData eventData)
        {
            //Debug.Log("OnDrag");
            float x = eventData.position.x - rawMousePosition.x;
            float y = eventData.position.y - rawMousePosition.y;

            //Debug.Log("rawMousePosition : " + rawMousePosition);

            //AddLetterData(x, y);

            OnMouseMove(new Vector3(eventData.position.x, eventData.position.y, 0));
            DrawImage();
            _writeStatus = WriteStatus.Writing;
        }

        #endregion

        #region 工具方法

        //  获取的左下角的屏幕坐标
        private void UpdateRawMousePosition()
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(raw.GetComponent<RectTransform>().position); // canvas render - camera
           // Vector3 screenPos = raw.GetComponent<RectTransform>().position;
            Vector2 rawanchorPositon = new Vector2(screenPos.x - raw.rectTransform.rect.width / 2.0f
            , screenPos.y - raw.rectTransform.rect.height / 2.0f);

            rawMousePosition = rawanchorPositon;
        }

        /// <summary>
        /// 将屏幕坐标转换为画布图片的相对坐标
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector2 ConvertScreenPositionToRawImagePosition(Vector2 position)
        {
            Vector2 to = new Vector2((int)(position.x - rawMousePosition.x),
                (int)(position.y - rawMousePosition.y));

            return to;
        }


        #endregion


        #endregion


        /// <summary>
        ///    将字节保存进文件
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dir"></param>
        /// <param name="filename"></param>
        private void SaveBytesToFile(byte[] bytes, string dir, string filename)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir); // 这步耗时

            // 判断当前目录下有多少个文件，如超过上限则进行清理
            //获取文件信息
            DirectoryInfo direction = new DirectoryInfo(dir);
            FileInfo[] files = direction.GetFiles("*");

            // 如果文件数量超过100，则清空文件夹
            if (files.Length > 100)
            {
                // 清理文件夹下所有的文件
                for (int i = 0; i < files.Length; i++)
                {
                    string fp = dir + "/" + files[i].Name;
                    File.Delete(fp);
                }

            }

            string path = dir + "/" + filename + ".png";
            FileStream file = File.Open(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Close();
        }


        IEnumerator Wait(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            print("WaitAndPrint " + Time.time);
        }

        /// <summary>
        ///     获取 texture
        /// </summary>
        /// <returns></returns>
        public Texture GetTexture() {
            //texRender.
            Texture texture = texRender;
            return texture;
        }

    }
}