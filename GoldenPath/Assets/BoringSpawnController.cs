using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BoringSpawnController : MonoBehaviour
{
    public GameObject myPrefab;
    public GameObject mySecondPrefab;

    private GameObject body;
    private GameObject arm;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (body == null)
            {
                body = Instantiate(myPrefab);
                if (mySecondPrefab != null)
                {
                    arm = Instantiate(mySecondPrefab);
                    arm.transform.parent = body.transform;
                }
            }
            else
            {
                Destroy(body);
                Destroy(arm);
            }
        }
    }
}
