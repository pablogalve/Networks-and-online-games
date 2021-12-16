using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : UDPObject
{
    public override void Start()
    {
        base.Start();
    }
    
    public void Send(MessageType type, GameObject complementaryObject)
    {
        Message message = null;

        switch (type)
        {
            case MessageType.INSTANCE:
                message = new VectorMessage(type, complementaryObject.GetComponent<NetworkedObject>());
                break;
        }

        if (message != null)
        {
            SendMessage(message);
        }
    }

    public override void ProcessMessage(Message receivedMessage)
    {
        base.ProcessMessage(receivedMessage);
    }
}
