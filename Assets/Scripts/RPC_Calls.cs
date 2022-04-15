using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class RPC_Calls : NetworkBehaviour
{
    // NOTE: This is all necessary because the name variable (used for labels) is local.
    // Meaning that running PlayerScript().SetName only changes the name on the computer of the client that ran it
    //      So to update a name, every client has to run SetName for every player

    // this seems redundant but it's really the only way I could think to do this
    // by default, only a client can make a Server RPC call and only a server can make a client RPC call
    // logic:
    // Getting old player names to new player:
    /*
     *      1. Run GetExising ServerRpc --> calls client Rpc
     *      2. client Rpc: gets ID and name of all clients, then all clients call TellNewPerson ServerRpc
     *      3. server Rpc: calls client Rpc
     *      4. client Rpc: for each client, ends unless the client is the new client
     *              if is new client: runs through all players on their system, 
     *              if the player is the old player who send out the RPC, updates the old client's name on new client's side
     *                  this is because, as of now, network variables don't work in netcode so
     *                  all variables have to be updated every individual client's network
     */

    // Getting new player name to old players:
    /*     1. Run TellEveryone ServerRpc --> calls client Rpc
     *     2. client Rpc: runs through all players on their system, 
     *          if it is the new client, updates the new client name on old client's side
     */


    // for all old players, tells them that a new player joined and wants to know their name
    [ServerRpc]
    public void GetExistingPlayerNamesServerRpc(ulong newPlayerId)
    {
        GetExistingPlayerNamesClientRpc(newPlayerId);
    }
    [ClientRpc]
    public void GetExistingPlayerNamesClientRpc(ulong newPlayerId)
    {
        ulong tempClientId = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        string tempName = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerScript>().clientName;
        TellNewPersonYourNameServerRpc(tempClientId, newPlayerId, tempName); 
    }

    // for all old players, tells new player their name
    [ServerRpc(RequireOwnership = false)]
    public void TellNewPersonYourNameServerRpc(ulong oldPlayer, ulong newPlayer, string oldPlayerName)
    {
        TellNewPersonYourNameClientRpc(oldPlayer, newPlayer, oldPlayerName); 
    }
    [ClientRpc]
    public void TellNewPersonYourNameClientRpc(ulong oldPlayer, ulong newPlayer, string oldPlayerName)
    {
        ulong clientNetworkId = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        if (clientNetworkId == newPlayer)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                ulong playerID = player.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
                if (playerID == oldPlayer)
                {
                    GameObject playerLabel = player.transform.GetChild(0).gameObject;
                    playerLabel.GetComponent<TextMeshPro>().text = oldPlayerName;
                }
            }
        }
    }

    // for new player, tells old players their name
    [ServerRpc]
    public void TellEveryoneYourNameServerRpc(string name, ulong newPlayerId)
    {
        TellEveryoneYourNameClientRpc(name, newPlayerId); 
    }

    [ClientRpc]
    public void TellEveryoneYourNameClientRpc(string name, ulong newPlayerId)
    {
        ulong clientNetworkId = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        if (clientNetworkId != newPlayerId)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                ulong playerID = player.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
                if (playerID == newPlayerId)
                {
                    GameObject playerLabel = player.transform.GetChild(0).gameObject;
                    playerLabel.GetComponent<TextMeshPro>().text = name;
                }
            }
        }
    }

    public GameObject localPurpleEggPrefab;
    public GameObject localBlueEggPrefab;

    // for easter egg game
    public void DisperseEggs()
    {
        DisperseEggsServerRpc(); 
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisperseEggsServerRpc()
    { 
        DisperseEggsClientRpc();
    }

    [ClientRpc]
    public void DisperseEggsClientRpc()
    {
        for (int i = 0; i < 5; i++)
        {
            var position = new Vector3(Random.Range(-40.0f, 35.0f), .5f, Random.Range(-40.0f, 35.0f));
            float temp = Random.Range(0, 1);
            if (temp == 1)
            {
                Instantiate(localPurpleEggPrefab, position, Quaternion.identity);
                Debug.Log("purple egg");
            }
            else
            {
                Instantiate(localBlueEggPrefab, position, Quaternion.identity);
                Debug.Log("blue egg");
            }
        }
    }
}
