using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : UDPObject
{
    public class Player
    {
        public Player(int _id, EndPoint socket, float _lastPing = 0.0f)
        {
            id = _id;
            lastPing = _lastPing;
            clientSocket = socket;
        }
        public int id;
        public float lastPing;
        public EndPoint clientSocket;
    };

    public static List<Player> connectedPlayers = new List<Player>(); //id of connected players
    public float maxPingAllowed = 2.0f;

    public override void Start()
    {
        //StartCoroutine(SpawnEnemy());
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Bind(ipep);

        ConnectionConfirmed();

        base.Start();
    }

    public override void Update()
    {
        base.Update();
        /*
        if(Input.GetKeyDown(KeyCode.W) == true)
        {
            GameOver();
        }
        */
        for (int i = 0; i < connectedPlayers.Count; ++i)
        {
            connectedPlayers[i].lastPing += Time.deltaTime;
            if (connectedPlayers[i].lastPing >= maxPingAllowed)
                DisconnectPlayer(connectedPlayers[i].id);
        }
        
    }

    public void GameOver()
    {
        SendMessageToBothPlayers(new Message(MessageType.GAME_FINISHED));
        foreach (var item in connectedPlayers)
        {
            DisconnectPlayerMessage disconnectMessage = new DisconnectPlayerMessage(item.id);
            SendMessageToBothPlayers(disconnectMessage);
        }
        DisconnectAllPlayers();
    }

    public override void ProcessMessage(Message receivedMessage, EndPoint clientSocket = null)
    {
        //Debug.Log("Message being processed by server");
        base.ProcessMessage(receivedMessage);

        switch (receivedMessage.type)
        {

            case MessageType.CONNECTION:
                Debug.Log("Client connecting");
                Message connectionMsg = new Message(MessageType.CONNECTION);
                ConnectPlayer(connectedPlayers.Count, clientSocket);

                //if (IsConnected(receivedMessage.senderId) == false)
                //{
                //    ConnectPlayer(receivedMessage.senderId);
                //}
                //Player godIsDead = connectedPlayers.Find((player) => player.id == receivedMessage.senderId);

                break;

            case MessageType.PLAYER_POSITION:
                if (connectedPlayers.Count == 2)
                {
                    VectorMessage playerPosition = receivedMessage as VectorMessage;
                    playerPosition.senderId = GetOtherPlayer(receivedMessage.senderId).id;
                    SendMessage(playerPosition);
                }
                break;

            case MessageType.OBJECT_POSITION:
                if (connectedPlayers.Count == 2)
                {
                    VectorMessage objectPositionMessage = receivedMessage as VectorMessage;
                    objectPositionMessage.senderId = GetOtherPlayer(receivedMessage.senderId).id;
                    SetObjectDesiredPosition(objectPositionMessage.objectId, objectPositionMessage.vector);
                }
                break;

            case MessageType.INSTANTIATE:
                InstanceMessage instanceMessage = receivedMessage as InstanceMessage;
                if (instanceMessage.objectId == "-1")
                {
                    instanceMessage.objectId = Message.GenerateNewGuid().ToString();
                }
                InstantiateObject(instanceMessage.objectId, GetObjectToInstantiate(instanceMessage._instanceType), instanceMessage.toVector3(instanceMessage._position), Quaternion.identity);
                break;

            case MessageType.COLLISION:
                CollisionMessage collisionMessage = receivedMessage as CollisionMessage;
                SolveCollision(collisionMessage.colliderObjectId, collisionMessage.collidedObjectId);
                break;

            case MessageType.PING_PONG:
                PingReceived(receivedMessage.senderId);
                //if(IsConnected(receivedMessage.senderId) == false)
                //{
                //    ConnectPlayer(receivedMessage.senderId);
                //}
                break;
            default:

                break;
        }

        SendMessage(receivedMessage);
    }

    //IEnumerator SpawnEnemy()
    //{
    //    while (gameObject.activeSelf)
    //    {
    //        string id = InstanceMessage.GenerateNewGuid().ToString();
    //        Vector3 position = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), 0.0f);
    //        InstantiateObject(id, enemyPrefab, position);

    //        InstanceMessage enemyInstanceMessage = new InstanceMessage(MessageType.INSTANTIATE, id, InstanceMessage.InstanceType.ENEMY, position, 0.0f);
    //        SendMessageToBothPlayers(enemyInstanceMessage);

    //        yield return new WaitForSeconds(3.0f);
    //    }
    //}

    public void InstantiateToAll(GameObject objectToInstantiate, InstanceMessage.InstanceType instanceType, Vector3 position, Quaternion rotation)
    {
        string id = InstanceMessage.GenerateNewGuid().ToString();
        InstantiateObject(id, GetObjectToInstantiate(instanceType), position, rotation);

        InstanceMessage enemyInstanceMessage = new InstanceMessage(MessageType.INSTANTIATE, id, instanceType, position, 0.0f);
        SendMessageToBothPlayers(enemyInstanceMessage);
    }

    bool IsConnected(byte id)
    {
        for (int i = 0; i < connectedPlayers.Count; ++i)
        {
            if (connectedPlayers[i].id == id) return true;
        }
        return false;
    }

    void PingReceived(int id)
    {
        Debug.Log("Ping" + " " + id);
        for (int i = 0; i < connectedPlayers.Count; ++i)
        {
            if (connectedPlayers[i].id == id)
            {
                connectedPlayers[i].lastPing = 0.0f;
            }
        }
    }

    void ConnectPlayer(int id, EndPoint clientSocket)
    {
        if (connectedPlayers.Count < 2)
        {
            connectedPlayers.Add(new Player(id, clientSocket, 0.0f));

            Message msg = new IdMessage(MessageType.CONNECTION, id.ToString());
            msg.senderId = id;

            SendMessage(msg);

            Debug.Log("Player connected" + connectedPlayers.Count);
            if (connectedPlayers.Count == 2)
            {
                //TODO: Send global msg to start the game
                Message startMsg = new Message(MessageType.START_GAME);
                SendMessageToBothPlayers(startMsg);

                functionsToRunInMainThread.Add(() =>
                {
                    WaveManager.instance.StartGame();
                });
                Debug.Log("Game starting");
            }
            Debug.Log("Player with id: " + (int)id + " has been connected to server successfully");
        }
        else Debug.Log("Connection rejected. There are already 2 connected players");
    }

    void DisconnectPlayer(int id)
    {
        for (int i = 0; i < connectedPlayers.Count; ++i)
        {
            if (connectedPlayers[i].id == id)
            {                
                DisconnectPlayerMessage disconnectMessage = new DisconnectPlayerMessage(connectedPlayers[i].id);
                SendMessageToBothPlayers(disconnectMessage);
                lock (connectedPlayers)
                {
                    connectedPlayers.RemoveAt(i);
                Debug.Log("Player disconnected");
                break;
                }

            }
        }
    }

    void DisconnectAllPlayers()
    {
        connectedPlayers.Clear();
    }

    Server.Player GetOtherPlayer(int playerId)
    {
        if (connectedPlayers[0].id == playerId)
        {
            return connectedPlayers[1];
        }
        else
        {
            return connectedPlayers[0];
        }
    }

    void SolveCollision(string colliderObejctId, string collidedObjectId)
    {
        Debug.Log("Collided Objects: " + colliderObejctId + " " + collidedObjectId);

        DestroyObject(colliderObejctId);
        DestroyObject(collidedObjectId);
    }

    public override void DestroyObject(string objectId)
    {
        IdMessage destroyMessage = new IdMessage(MessageType.DESTROY, objectId);

        SendMessageToBothPlayers(destroyMessage);

        base.DestroyObject(objectId);
    }

    public void SendMessageToBothPlayers(Message message)
    {
        //TODO: Send to both players
        message.senderId = 2;
        SendMessage(message);
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

                        foreach (var item in connectedPlayers)
                        {
                            if (messagesToSend[0].senderId == 2 || messagesToSend[0].senderId == item.id) //2 means send to everyone
                            {
                                int bytesSent = socket.SendTo(msg, msg.Length, SocketFlags.None, item.clientSocket);
                            }
                        }

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

                EndPoint receivedFrom = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
                int recv = socket.ReceiveFrom(msg, ref receivedFrom);

                if (recv > 0)
                {
                    Debug.Log("Msg received");
                    Message receivedMessage = Message.Deserialize(msg);
                    if (receivedMessage != null)
                    {
                        ProcessMessage(receivedMessage, receivedFrom);
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
