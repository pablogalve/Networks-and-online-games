using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class Client : UDPObject
{
    public NetworkedObject player1;
    public NetworkedObject player2;

    public float secondsBetweenPlayerPositionUpdates = 0.1f;

    private float secondsBetweenPings = 0.5f;
    private float currentTimer = 0.5f;
    private bool timerActive = true;

    byte playerId = 0;

    public override void Start()
    {
        //base.Start();
        port = 7778;
        networkedObjects = new Dictionary<string, NetworkedObject>();

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        senderRemote = (EndPoint)endPoint;

        socket.Bind(endPoint);

        sendThread = new Thread(StartSending);
        sendThread.Start();

        receiveThread = new Thread(new ThreadStart(StartReceiving));
        receiveThread.Start();
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
                PingPongMessage msg = new PingPongMessage();
                Send(msg);
                currentTimer = secondsBetweenPings;
            }
        }
    }

    public void Send(Message message)
    {
        if (message != null)
        {
            message.senderId = playerId;
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
                InstantiateObject(instanceMessage.objectId, GetObjectToInstantiate(instanceMessage), instanceMessage.toVector3(instanceMessage._position));
                break;

            case MessageType.DESTROY:
                IdMessage idMessage = receivedMessage as IdMessage;
                DestroyObject(idMessage.objectId);
                break;

            case MessageType.OBJECT_POSITION:
                VectorMessage objectPositionMessage = (VectorMessage)receivedMessage;
                SetObjectDesiredPosition(objectPositionMessage.objectId, objectPositionMessage.vector);

                //TODO: Remove this, only for debug
                if(objectPositionMessage.objectId == player1.id)
                {
                    player2.desiredPosition = objectPositionMessage.vector;
                }

                break;

            case MessageType.PING_PONG:
                //Debug.Log("Pong received. I'm still connected to server");
                timerActive = true;
                break;
        }
    }
}