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

    private float secondsBetweenPings = 5.0f;
    private float currentTimer = 0.0f;
    private bool timerActive = true;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        player2.transform.position = Vector3.Lerp(player2.transform.position, otherPlayerLastPosition, interpolationSpeed * Time.deltaTime);

        // Send pings to server to ensure that client remains active in UDP
        if (timerActive)
        {
            if (currentTimer >= 0)
                currentTimer -= Time.deltaTime;            
            else
            {
                timerActive = false;
                Send(MessageType.PING_PONG, null);
                currentTimer = secondsBetweenPings;
            }
        }        
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

            case MessageType.PING_PONG:
                Debug.Log("Ping sent");
                message = new VectorMessage(type, "0", new Vector3(0, 0, 0));
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

            case MessageType.PING_PONG:
                timerActive = true;
                break;
        }
    }
}
