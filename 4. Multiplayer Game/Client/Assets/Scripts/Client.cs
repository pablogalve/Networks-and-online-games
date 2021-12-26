using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Client : UDPObject
{
    public NetworkedObject player1;
    public NetworkedObject player2;

    public float secondsBetweenPlayerPositionUpdates = 0.1f;

    private float secondsBetweenPings = 0.5f;
    private float currentTimer = 0.5f;
    private bool timerActive = true;

    byte playerId = 0;

    public int maxConnectionTries = 20;
    private int connectionTries = 0;

    public Text connectionDisplayText;

    public Thread connectionThread;
    public EndPoint Remote;

    public override void Start()
    {
        base.Start();

        //Time.timeScale = 0.0f;
        player1.transform.gameObject.SetActive(false);

        IPEndPoint ipep = new IPEndPoint(StaticVariables.userPointIP == null ? IPAddress.Parse("127.0.0.1") : StaticVariables.userPointIP, port);
        Debug.Log(StaticVariables.userPointIP);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        int sentBytes = socket.SendTo(new byte[1], (EndPoint)ipep);

        Remote = (EndPoint)ipep;

        connectionThread = new Thread(new ThreadStart(TryConnection));
        connectionThread.Start();

        player1.Init();
        networkedObjects[player1.id] = player1;

        player2.Init();
        networkedObjects[player2.id] = player2;
    }

    void AddConnectionTry()
    {
        connectionTries++;
        if(connectionTries >= maxConnectionTries)
        {
            functionsToRunInMainThread.Add(() => {
                SceneManager.LoadSceneAsync(0);
            });
        }
        else
        {
            functionsToRunInMainThread.Add(() => {
                connectionDisplayText.text = connectionTries.ToString() + " / " + maxConnectionTries.ToString() + "\ntrying to connect";
            });
        }
    }

    public void TryConnection()
    {
        socket.ReceiveTimeout = 5000;
        bool connected = false;

        byte[] data = new byte[256];
        while (connectionTries < maxConnectionTries && connected == false)
        {
            try
            {
                socket.SendTo((new Message(MessageType.CONNECTION)).Serialize(), Remote);
                Debug.Log(connectionTries.ToString());

                EndPoint received = new IPEndPoint(IPAddress.Any, 0);
                int sentBytes = socket.ReceiveFrom(data, ref received);
                Debug.Log(connectionTries.ToString());

                Message connectionMessage = Message.Deserialize(data);
                if(connectionMessage.type == MessageType.CONNECTION)
                {
                    this.playerId = connectionMessage.senderId;
                    base.ConnectionConfirmed();
                    connected = true;
                    functionsToRunInMainThread.Add(() => {
                        connectionDisplayText.text = "Waiting for player 2";
                    });

                    Player player;
                    if(playerId == 0)
                    {
                        player = player1 as Player;
                    }
                    else
                    {
                        player = player2 as Player;
                    }

                    player.ActivatePlayer();
                }

            }
            catch (Exception)
            {
                AddConnectionTry();
            }
        }
    }

    public override void Update()
    {
        base.Update();

        // Send pings to server to ensure that client remains active in UDP
        if (timerActive)
        {
            if (currentTimer >= 0)
            {
                currentTimer -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Sending ping");
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

    public override void ProcessMessage(Message receivedMessage, EndPoint clientSocket = null)
    {
        base.ProcessMessage(receivedMessage);

        Debug.Log(receivedMessage.type.ToString());
        switch (receivedMessage.type)
        {
            case MessageType.START_GAME:
                MenuManager.instance.TurnConnectUI_OFF();
                player1.transform.gameObject.SetActive(true);
                break;

            case MessageType.GAME_FINISHED:
                functionsToRunInMainThread.Add(() => {
                    SceneManager.LoadSceneAsync(0);
                });
                break;

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

                break;

            case MessageType.PING_PONG:
                //Debug.Log("Pong received. I'm still connected to server");
                timerActive = true;
                break;
            case MessageType.DISONNECT_PLAYER:
                if(receivedMessage.senderId == this.playerId)
                {
                    functionsToRunInMainThread.Add(() => {
                        SceneManager.LoadSceneAsync(0);
                    });
                }
                else
                {
                    //TODO: Remove player2 mesh or something
                }
                break;
        }
    }

    public override void StartSending()
    {
        while (active)
        {
            while (messagesToSend.Count > 0)
            {
                try
                {
                    if (messagesToSend[0] != null)
                    {
                        byte[] msg = messagesToSend[0].Serialize();
                        int bytesSent = socket.SendTo(msg, msg.Length, SocketFlags.None, Remote);

                        //Debug.Log("Message sent!");
                        messagesToSend.RemoveAt(0);
                    }
                }
                catch (System.Exception exception)
                {
                    Debug.LogException(exception);
                    //CloseSocket(socket);
                    //active = false;
                }
            }
        }
    }

    public override void StartReceiving()
    {
        while (active)
        {
            try
            {
                byte[] msg = new byte[256];

                EndPoint received = new IPEndPoint(IPAddress.Any, 0);
                int recv = socket.ReceiveFrom(msg, ref received);

                if (recv > 0)
                {
                    Message receivedMessage = Message.Deserialize(msg);
                    if (receivedMessage != null)
                    {
                        ProcessMessage(receivedMessage);
                        //Debug.Log("Received message: " + receivedMessage.type.ToString());
                    }
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
                //active = false;
                //CloseSocket(socket);
            }
        }
    }
}