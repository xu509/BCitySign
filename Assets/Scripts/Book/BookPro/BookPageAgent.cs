using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BCity {

    public class BookPageAgent : MonoBehaviour
    {
        [SerializeField] BookPageType _bookPageType;

        [SerializeField] RectTransform _coverContainer;
        [SerializeField] RectTransform _normalContainer;
        [SerializeField] RectTransform _defaultContainer;

        [SerializeField,Header("Left")] Image _signImage;
        [SerializeField] Text _cdateText;
        [SerializeField, Header("Right")] Image _photoImage;
        [SerializeField, Header("Empty Image")] Sprite _emptyImage;

        PageRecord _pageRecord;


        BCManager _manager;

        public void Init(PageRecord pageRecord,bool isCover,bool isDefault) {
            _pageRecord = pageRecord;

            _manager = GameObject.Find("MainBrain").GetComponent<BCManager>();

            if (isDefault)
            {
                _defaultContainer.gameObject.SetActive(true);
                _normalContainer.gameObject.SetActive(false);
                _coverContainer.gameObject.SetActive(false);
            }
            else {
                _defaultContainer.gameObject.SetActive(false);

                if (isCover)
                {
                    _normalContainer.gameObject.SetActive(false);
                    _coverContainer.gameObject.SetActive(true);
                }
                else {
                    _coverContainer.gameObject.SetActive(false);

                    if (_bookPageType == BookPageType.Left)
                    {
                        // 设置签名图片
                        _signImage.sprite = _manager.daoManager.GetImageSprite(_pageRecord.SignAddress);

                        // 设置时间
                        _cdateText.text = _pageRecord.Cdate.ToString("yyyy.MM.dd");
                    }
                    else
                    {

                        _normalContainer.gameObject.SetActive(true);
                        // 设置照片
                        if (_pageRecord.PhotoAddress == null)
                        {
                            _photoImage.sprite = _emptyImage;
                        }
                        else {
                            _photoImage.sprite = _manager.daoManager.GetImageSprite(_pageRecord.PhotoAddress);
                        }
                    }

                }
            }
        }
    }

}
