using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EggRPC_Call : NetworkBehaviour
{
    public Material redMaterial;

    [ServerRpc]
    public void ChangeColorServerRpc()
    {
        ChangeColorClientRpc(); 
    }
    [ClientRpc]
    public void ChangeColorClientRpc()
    {
        GetComponent<MeshRenderer>().material = redMaterial;
    }

}
