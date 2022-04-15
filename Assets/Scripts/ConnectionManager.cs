using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro; 

public class ConnectionManager : MonoBehaviour
{
    public GameObject loginPanel;
    public TMP_Text enteredName;
    [SerializeField] private string clientName = "anon";


    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    public void OnDestroy()
    {
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    // should be attached to button
    // when pressed: joins new client to prexisting room started by host
    public void JoinRoom()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StartRoom()
    {
        NetworkManager.Singleton.StartServer();
    }

    // does nothing; still needed
    private void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Server works!");
        }
    }

    // calls newly created client and sets data
    private void HandleClientConnected(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            if (!NetworkManager.Singleton.IsHost)
                loginPanel.SetActive(false);
            ulong clientNetworkId = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<NetworkObject>().NetworkObjectId; 
            Debug.Log("Client " + clientID + " connected.");
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerScript>().SetName(enteredName.text);
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<RPC_Calls>().TellEveryoneYourNameServerRpc(enteredName.text, clientNetworkId);
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<RPC_Calls>().GetExistingPlayerNamesServerRpc(clientNetworkId);
        }
    }

    private void HandleClientDisconnected(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Player " + clientID + " Disconnected"); 
        }
    }

    // for easter egg game
    public void DisperseEggs()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Is server");
            NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<RPC_Calls>().DisperseEggs();
        }
    }
}
