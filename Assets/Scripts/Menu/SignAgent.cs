using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BCity { 

    /// <summary>
    /// 签字版代理
    /// </summary>
    public class SignAgent : MonoBehaviour
    {
        #region 属性

        private MenuAgent _menuAgent;

        //绘图shader&material
        [SerializeField]
        private Shader _paintBrushShader;
        private Material _paintBrushMat;
        //清理renderTexture的shader&material
        [SerializeField]
        private Shader _clearBrushShader;
        private Material _clearBrushMat;
        //默认笔刷RawImage
        [SerializeField]
        private RawImage _defaultBrushRawImage;
        //默认笔刷&笔刷合集
        [SerializeField]
        private Texture _defaultBrushTex;
        //renderTexture
        private RenderTexture _renderTex;
        //默认笔刷RawImage
        [SerializeField]
        private Image _defaultColorImage;
        //绘画的画布
        [SerializeField]
        private RawImage _paintCanvas;
        //笔刷的默认颜色&颜色合集
        [SerializeField]
        private Color _defaultColor;
        //笔刷大小的slider
        private Text _brushSizeText;
        //笔刷的大小
        private float _brushSize;
        //屏幕的宽高
        private int _screenWidth;
        private int _screenHeight;

        private int _canvasWidth;
        private int _canvasHeight;
        //笔刷的间隔大小
        private float _brushLerpSize;
        //默认上一次点的位置
        private Vector2 _lastPoint;
        #endregion

        public void Init(MenuAgent menuAgent) {
            _menuAgent = menuAgent;

            _brushSize = 150.0f;
            _brushLerpSize = (_defaultBrushTex.width + _defaultBrushTex.height) / 2.0f / _brushSize;
            _lastPoint = Vector2.zero;

            if (_paintBrushMat == null)
            {
                UpdateBrushMaterial();
            }
            if(_clearBrushMat==null)
            _clearBrushMat = new Material(_clearBrushShader);
            _canvasWidth = (int)_paintCanvas.rectTransform.rect.width;
            _canvasHeight = (int)_paintCanvas.rectTransform.rect.height;
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            if (_renderTex == null)
            {
                _renderTex = RenderTexture.GetTemporary(_canvasWidth, _canvasHeight, 24);
                _paintCanvas.texture = _renderTex;
            }
            Graphics.Blit(null, _renderTex, _clearBrushMat);
        }

        //更新笔刷材质
        private void UpdateBrushMaterial()
        {
            _paintBrushMat = new Material(_paintBrushShader);
            _paintBrushMat.SetTexture("_BrushTex", _defaultBrushTex);
            _paintBrushMat.SetColor("_Color", _defaultColor);
            _paintBrushMat.SetFloat("_Size", _brushSize);
        }
        /// <summary>
        ///     打开
        /// </summary>
        public void Open() {
            GetComponent<Image>().DOFade(1f, 2f);
        }

        /// <summary>
        ///     关闭
        /// </summary>
        public void Close() { 
        
        }

        //插点
        private void LerpPaint(Vector2 point)
        {
            Paint(point);

            if (_lastPoint == Vector2.zero)
            {
                _lastPoint = point;
                return;
            }
            
            float dis = Vector2.Distance(point, _lastPoint);
            if (dis > _brushLerpSize)
            {
                Vector2 dir = (point - _lastPoint).normalized;
                int num = (int)(dis / _brushLerpSize);
                for (int i = 0; i < num; i++)
                {
                    Vector2 newPoint = _lastPoint + dir * (i + 1) * _brushLerpSize;
                    Paint(newPoint);
                }
            }
            _lastPoint = point;
        }

        //画点
        private void Paint(Vector2 point)
        {

            float canvasX = (_screenWidth - _canvasWidth)/2;
            float canvasY = (_screenHeight - _canvasHeight)/2 + 47;

            if (point.x < canvasX || point.x > (canvasX+_canvasWidth) || point.y < canvasY || point.y > (canvasY+_canvasHeight))
                return;

            Vector2 uv = new Vector2((point.x - canvasX) / (float)_canvasWidth,
                (point.y - canvasY) / (float)_canvasHeight);

            _paintBrushMat.SetVector("_UV", uv);
            Graphics.Blit(_renderTex, _renderTex , _paintBrushMat);
        }
        /// <summary>
        ///     点击拍照
        /// </summary>
        public void DoPhoto() {
            gameObject.SetActive(false);
            _menuAgent.OpenPhoto();
        }

        public void DoFinish() {
            gameObject.SetActive(false);
            _menuAgent.OpenAlbum(false);
        }

        /// <summary>
        /// <summary>
        /// 拖拽
        /// </summary>
        public void DragUpdate()
        {
            if (_renderTex && _paintBrushMat)
            {

                if (Input.GetMouseButton(0))
                    LerpPaint(Input.mousePosition);

            }
        }
        /// <summary>
        /// 拖拽结束
        /// </summary>
        public void DragEnd()
        {
            if (Input.GetMouseButtonUp(0))
                _lastPoint = Vector2.zero;
        }
    }
}