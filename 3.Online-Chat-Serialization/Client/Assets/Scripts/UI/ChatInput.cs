using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatInput : MonoBehaviour
{
    InputField inputField; 
    
    [SerializeField]
    TCPClient client;

    void Start()
    {
        inputField = GetComponent<InputField>();
    }

    void Update()
    {
        
    }

    public void OnMessageWritten(string message)
    {
        if(client != null)
        {
            client.OnSendMessage(message);
        }

        if(inputField != null)
        {
            inputField.text = "";
        }
    }
}
