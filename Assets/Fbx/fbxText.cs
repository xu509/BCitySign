using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class fbxText : MonoBehaviour
{

    public Space m_RotateSpace;
    public float m_RotateSpeed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Transform>().Rotate(Vector3.forward * m_RotateSpeed * Time.deltaTime, m_RotateSpace);
    }
}
