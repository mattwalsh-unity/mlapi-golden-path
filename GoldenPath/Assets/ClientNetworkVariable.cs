using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

public class ClientNetworkVariable : NetworkBehaviour
{
    private NetworkVariable<float> nv = new NetworkVariable<float>();
    private float last_t = 0.0f;

    void Start()
    {
        nv.Settings.WritePermission = NetworkVariablePermission.OwnerOnly;
        nv.Settings.ReadPermission = NetworkVariablePermission.ServerOnly;
        if (IsClient)
        {
            nv.Value = 0.0f;
            Debug.Log("ClientNetworkVariable initialized var to: " + nv.Value);
        }
    }

    void Update()
    {
        var t_now = Time.time;
        if (IsServer)
        {
            nv.Value = nv.Value + 0.1f;
            if (t_now - last_t > 0.5f)
            {
                last_t = t_now;
                Debug.Log("Server thinks the ClientNetworkVariable is is: " + nv.Value);
            }
        }
        else if (IsClient)
        {
            if (t_now - last_t > 0.5f)
            {
                last_t = t_now;
                Debug.Log("Client set the ClientNetworkVariable is is: " + nv.Value);
            }
        }
    }
}