using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : UDPObject
{ 
    public override void ProcessMessage(Message receivedMessage)
    {
        base.ProcessMessage(receivedMessage);

        switch(receivedMessage.type)
        {
            case MessageType.PLAYER_POSITION:
                SendMessage(receivedMessage);
                break;
        }
    }
}
