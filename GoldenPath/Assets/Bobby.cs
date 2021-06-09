using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bobby : MonoBehaviour
{
    private const float scaleIt = .5f;
    public float offsetIt = 0.0f;

    void Update()
    {
        float t = Time.frameCount / 10.0f;
        transform.localPosition = new Vector3(0.0f, offsetIt + scaleIt*Mathf.Sin(t), 0.0f); 
    }
}
