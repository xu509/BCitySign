using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace BCity
{
    public class ReminderAgent : MonoBehaviour
    {
        BCManager _bcManager;

        [SerializeField] EnterAgent _enterAgent;


        // Start is called before the first frame update
        void Start()
        {
            _bcManager = GameObject.Find("MainBrain").GetComponent<BCManager>();



        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DoChoose() {
            Debug.Log("Do Choose It!");
            _enterAgent.GetComponent<CanvasGroup>().DOFade(0, Time.deltaTime);
            _enterAgent.gameObject.SetActive(false);
            
            GetComponent<RectTransform>().DOScale(0.1f, 0.5f)
                .OnComplete(()=> {
                    // 生成menu
                    
                    var menuAgent = GameObject.Instantiate(_bcManager.screenProtectManager.menuAgentPrefab, _bcManager.screenProtectManager.opContainer);
                    menuAgent.Init();
                    //menuAgent.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                    //menuAgent.GetComponent<Image>().DOFade(1f, 2f);
                    _bcManager.menuAgent = menuAgent;
                });
        }

        public void Recover() {
            _enterAgent.gameObject.SetActive(true);
            _enterAgent.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }

    }
}