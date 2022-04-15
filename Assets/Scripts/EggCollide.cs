using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EggCollide : MonoBehaviour
{
    public Material redMaterial;
    public Material goldMaterial;

    private void OnTriggerEnter(Collider other)
    {
        ulong localNetworkId = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        ulong hitNetworkId = other.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        if (localNetworkId == hitNetworkId)
        {
            if (TryGetComponent<NetworkObject>(out var egg))
            {
                GetComponent<MeshRenderer>().material = redMaterial;
                TryGetComponent<EggRPC_Call>(out var changeColor);
                changeColor.ChangeColorServerRpc(); 
            }
            else
            {
                GetComponent<MeshRenderer>().material = goldMaterial;
            }
        }
    }
}
