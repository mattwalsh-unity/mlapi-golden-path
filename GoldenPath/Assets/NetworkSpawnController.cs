using System;
using MLAPI;
using UnityEngine;

public class NetworkSpawnController : NetworkBehaviour 
{
    public GameObject myPrefabNO;
    public GameObject mySecondPrefabNO;

    private GameObject body;
    private GameObject arm;

    void Update()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer && NetworkManager.Singleton.IsListening)
        {
            if (Input.GetKeyUp(KeyCode.S))
            {
                if (body == null)
                {
                    body = Instantiate(myPrefabNO);
                    var no = body.GetComponent<NetworkObject>(); 
                    no.Spawn(null, true);

                    if (mySecondPrefabNO)
                    {
                        arm = Instantiate(mySecondPrefabNO);
                        no = arm.GetComponent<NetworkObject>();
                        no.Spawn(null, true);
                        arm.transform.parent = body.transform;
                    }

                }
                else
                {
                    var no = body.GetComponent<NetworkObject>(); 
                    no.Despawn();
                    Destroy(body);

                    if (mySecondPrefabNO)
                    {
                        no = arm.GetComponent<NetworkObject>();
                        no.Despawn();
                        Destroy(arm);
                    }
                }
            }
        }
    }
}