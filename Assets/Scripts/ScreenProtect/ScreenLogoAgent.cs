using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCity { 
    public class ScreenLogoAgent : MonoBehaviour
    {
        [SerializeField,Header("是否正转")] bool _foreward;
        [SerializeField,Header("速度")] float _speed;

        // Update is called once per frame
        void Update()
        {
            Vector3 dir;
            if (_foreward) {
                dir = Vector3.left;
            } else {
                dir = Vector3.right;
            }

            transform.Rotate(dir, Time.deltaTime * _speed);
        
        }
    }
}