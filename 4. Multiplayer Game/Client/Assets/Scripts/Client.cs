using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : UDPObject
{
    public GameObject player1;
    public GameObject player2;

    private Vector3 otherPlayerLastPosition;

    public float interpolationSpeed = 5.0f;
    public float secondsBetweenPlayerPositionUpdates = 0.1f;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        player2.transform.position = Vector3.Lerp(player2.transform.position, otherPlayerLastPosition, interpolationSpeed * Time.deltaTime);
    }

    public void Send(MessageType type, NetworkedObject primaryNetworkedObject, UnityEngine.Object secondaryObject = null)
    {
        Message message = null;

        switch (type)
        {
            case MessageType.INSTATIATION:
                NetworkedObject secondaryNetworkedObject = secondaryObject as NetworkedObject;
                message = new VectorMessage(type, primaryNetworkedObject.id.ToString(), primaryNetworkedObject.transform.position, secondaryNetworkedObject.GetType());
                break;

            case MessageType.DESTROY:
                //Please write something xd
                break;

            case MessageType.PLAYER_POSITION:
                message = new VectorMessage(type, primaryNetworkedObject.id.ToString(), primaryNetworkedObject.transform.position + new Vector3(0.0f, -10.0f, 0.0f));
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

        switch (receivedMessage.type)
        {
            case MessageType.INSTATIATION:
                Action instantationAction = () =>
                {
                    
                };
                break;

            case MessageType.DESTROY:

                break;

            case MessageType.PLAYER_POSITION:
                VectorMessage playerPositionMessage = (VectorMessage)receivedMessage;
                otherPlayerLastPosition = playerPositionMessage.vector;
                break;

        }
    }
}
