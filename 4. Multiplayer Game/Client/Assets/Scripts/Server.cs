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
        public Player(byte _id, float _lastPing = 0.0f)
        {
            id = _id;
            lastPing = _lastPing;
        }
        public byte id;
        public float lastPing;
    };

    public static List<Player> connectedPlayers = new List<Player>(); //id of connected players
    public float maxPingAllowed = 5.0f;

    public override void Start()
    {
        //StartCoroutine(SpawnEnemy());
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Bind(ipep);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = sender;

        base.Start();
    }

    public override void Update()
    {
        if (IsConnected(0))
        {
            //Debug.Log("Player 1 last ping: " + connectedPlayers[0].lastPing);
        }
        else
        {
            //ConnectPlayer(0);
            //Debug.Log("Player 1 is not connected");
        }
        /*if (IsConnected(1))
            Debug.Log("Player 2 last ping: " + connectedPlayers[1].lastPing);
        else Debug.Log("Player 2 is not connected");*/

        base.Update();
        for (int i = 0; i < connectedPlayers.Count; ++i)
        {            
            connectedPlayers[i].lastPing += Time.deltaTime;
            if (connectedPlayers[i].lastPing >= maxPingAllowed)
                DisconnectPlayer(connectedPlayers[i].id);
        }
    }

    public override void ProcessMessage(Message receivedMessage)
    {
        //Debug.Log("Message being processed by server");
        base.ProcessMessage(receivedMessage);

        switch (receivedMessage.type)
        {
            case MessageType.OBJECT_POSITION:
                VectorMessage objectPositionMessage = (VectorMessage)receivedMessage;
                SetObjectDesiredPosition(objectPositionMessage.objectId, objectPositionMessage.vector);
                //TODO: Send object position to the other player
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
                if(IsConnected(receivedMessage.senderId) == false)
                {
                    ConnectPlayer(receivedMessage.senderId);
                }
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

    void PingReceived(byte id)
    {
        for (int i = 0; i < connectedPlayers.Count; ++i)
        {
            if(connectedPlayers[i].id == id)
            {
                connectedPlayers[i].lastPing = 0.0f;
            }
        }
    }

    void ConnectPlayer(byte id)
    {
        connectedPlayers.Add(new Player(id, 0.0f));
    }

    void DisconnectPlayer(byte id)
    {
        for (int i = 0; i < connectedPlayers.Count; ++i)
        {
            if (connectedPlayers[i].id == id)
            {
                //TODO: Disconnect player
                connectedPlayers.RemoveAt(i);
                break;
            }
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
        //messagesToSend.Add(message);
        SendMessage(message);
    }
}
