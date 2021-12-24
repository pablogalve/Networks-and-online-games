using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : UDPObject
{
    private class Player
    {
        public Player(string _id, float _lastPing = 0.0f)
        {
            id = _id;
            lastPing = _lastPing;
        }
        public string id;
        public float lastPing;
    };
    private List<Player> connectedPlayers = new List<Player>(); //id of connected players
    public float maxPingAllowed = 5.0f;

    public override void Start()
    {
        base.Start();
        StartCoroutine(SpawnEnemy());
    }

    public override void Update()
    {
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
                InstantiateObject(instanceMessage.objectId, GetObjectToInstantiate(instanceMessage), instanceMessage.toVector3(instanceMessage._position));
                break;

            case MessageType.COLLISION:
                CollisionMessage collisionMessage = receivedMessage as CollisionMessage;
                SolveCollision(collisionMessage.colliderObjectId, collisionMessage.collidedObjectId);
                break;

            case MessageType.PING_PONG:
                Debug.Log("Pong Sent to client!");
                //TODO: Return a pong to the client
                break;
        }

        SendMessage(receivedMessage);
    }

    IEnumerator SpawnEnemy()
    {
        while (gameObject.activeSelf)
        {
            InstanceMessage enemyInstanceMessage = new InstanceMessage(MessageType.INSTANTIATE, "-1", InstanceMessage.InstanceType.ENEMY, new Vector3(0.0f, 0.0f, 0.0f), 0.0f);
            messagesToSend.Add(enemyInstanceMessage);

            yield return new WaitForSeconds(3.0f);
        }
    }

    void ConnectPlayer(string id)
    {
        connectedPlayers.Add(new Player(id, 0.0f));
    }

    void DisconnectPlayer(string id)
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
        DestroyObject(colliderObejctId);
        DestroyObject(collidedObjectId);
    }

    public override void DestroyObject(string objectId)
    {
        IdMessage destroyMessage = new IdMessage(MessageType.DESTROY, objectId);

        //TODO: Send to both players
        SendMessage(destroyMessage);

        base.DestroyObject(objectId);
    }
}
