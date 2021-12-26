using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using UnityEngine.SceneManagement;

public class Client : UDPObject
{
    public NetworkedObject player1;
    public NetworkedObject player2;

    public float secondsBetweenPlayerPositionUpdates = 0.1f;

    private float secondsBetweenPings = 0.5f;
    private float currentTimer = 0.5f;
    private bool timerActive = true;

    byte playerId = 0;

    public int maxConnectionTries = 5;
    private int connectionTries = 0;

    public override void Start()
    {
        IPEndPoint ipep = new IPEndPoint(StaticVariables.userPointIP == null ? IPAddress.Parse("127.0.0.1") : StaticVariables.userPointIP, port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        int sentBytes = socket.SendTo((new ConnectionMessage(playerId)).Serialize(), ipep);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)sender;

        base.Start();
    }

    void AddConnectionTry()
    {
        connectionTries++;
        if(connectionTries >= maxConnectionTries)
        {
            SceneManager.LoadSceneAsync(0);
        }
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
                InstantiateObject(instanceMessage.objectId, GetObjectToInstantiate(instanceMessage._instanceType), instanceMessage.toVector3(instanceMessage._position), Quaternion.identity);
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
            case MessageType.DISONNECT_PLAYER:
                Debug.Log("Another player has been disconnected from server.");
                break;
        }
    }
}