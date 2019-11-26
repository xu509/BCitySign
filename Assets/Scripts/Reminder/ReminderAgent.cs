using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BCity
{
    public class ReminderAgent : MonoBehaviour
    {
        BCManager _bcManager;

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
            GameObject enter = GameObject.Find("Enter");
            enter.SetActive(false);
            GetComponent<RectTransform>().DOScale(0.1f, 0.5f)
                .OnComplete(()=> {
                    // 生成menu
                    
                    var menuAgent = GameObject.Instantiate(_bcManager.screenProtectManager.menuAgentPrefab, _bcManager.screenProtectManager.opContainer);
                    menuAgent.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                    menuAgent.GetComponent<RectTransform>().DOScale(1f, 2f);
                });

        }

    }
}