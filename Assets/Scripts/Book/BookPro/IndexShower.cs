using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexShower : MonoBehaviour
{
    public int siblingIndex;

    // Start is called before the first frame update
    void Start()
    {
        siblingIndex =  GetComponent<RectTransform>().GetSiblingIndex();

    }


}
