using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class BobbyNO : NetworkBehaviour
{
    private const float scaleIt = .5f;
    public float offsetIt = 0.0f;
    public bool enableBobby = true; 
    
    void Update()
    {
        if (IsServer && enableBobby)
        {
            float t = Time.frameCount / 10.0f;
            // actually doesn't look right for 'position' or 'local position' on the client
            transform.localPosition = new Vector3(0.0f, offsetIt + scaleIt * Mathf.Sin(t), 0.0f);
        }
    }
}