using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : UDPObject
{ 
    public override void ProcessMessage(Message receivedMessage)
    {
        Debug.Log("Message being processed by server");
        base.ProcessMessage(receivedMessage);

        switch(receivedMessage.type)
        {
            case MessageType.PLAYER_POSITION:
                SendMessage(receivedMessage);
                break;

            case MessageType.PING_PONG:
                Debug.Log("Pong Sent to client!");
                SendMessage(receivedMessage);
                break;
        }
    }
}
