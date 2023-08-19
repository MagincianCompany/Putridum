using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineScript : MonoBehaviour
{
    public GameObject body;
    public GameObject rect;
    // Update is called once per frame
    void Update()
    {
        rect.transform.localScale = body.transform.localScale;
    }
}
