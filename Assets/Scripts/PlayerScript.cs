using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerScript : NetworkBehaviour
{
    public string clientName;
    public GameObject label; 

    // whenever this is called, sets label to name passed as a parameter
    public void SetName(string newName)
    {
        clientName = newName;
        label.GetComponent<TextMeshPro>().text = clientName; 
    }
}
