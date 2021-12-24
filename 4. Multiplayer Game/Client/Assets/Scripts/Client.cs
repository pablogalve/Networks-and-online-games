using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : UDPObject
{
    public NetworkedObject player1;
    public NetworkedObject player2;

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

        // Send pings to server to ensure that client remains active in UDP
        if (timerActive)
        {
            if (currentTimer >= 0)
                currentTimer -= Time.deltaTime;
            else
            {
                timerActive = false;
                PingPongMessage msg = new PingPongMessage(player1.id, "ping");
                Send(msg);
                currentTimer = secondsBetweenPings;
            }
        }
    }

    public void Send(Message message)
    {
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
            case MessageType.INSTANTIATE:
                InstanceMessage instanceMessage = receivedMessage as InstanceMessage;
                Action instantationAction = () =>
                {
                    GameObject objectToInstance = GetObjectToInstantiate(instanceMessage);
                    GameObject objectInstance = Instantiate(objectToInstance, instanceMessage.toVector3(instanceMessage._position), Quaternion.identity);
                    NetworkedObject networkedInstance = objectInstance.GetComponent<NetworkedObject>();
                    networkedInstance.id = instanceMessage.objectId;
                };
                functionsToRunInMainThread.Add(instantationAction);
                break;

            case MessageType.DESTROY:
                IdMessage idMessage = receivedMessage as IdMessage;
                DestroyObject(idMessage.objectId);
                break;

            case MessageType.OBJECT_POSITION:
                VectorMessage objectPositionMessage = (VectorMessage)receivedMessage;
                SetObjectDesiredPosition(objectPositionMessage.objectId, objectPositionMessage.vector);
                break;

            case MessageType.PING_PONG:
                timerActive = true;
                break;
        }
    }
}