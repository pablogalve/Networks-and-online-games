using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : UDPObject
{
    private class Player
    {
        public Player(string _id, float _lastPing = 0.0f) {
            id = _id;
            lastPing = _lastPing; }
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
        for(int i = 0; i < connectedPlayers.Count; ++i)
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
                break;

            case MessageType.INSTATIATION:
                InstanceMessage instanceMessage = receivedMessage as InstanceMessage;
                if (receivedMessage.objectId == "-1")
                {
                    receivedMessage.objectId = Message.GenerateNewGuid().ToString();
                }
                Action instantationAction = () =>
                {
                    GameObject objectToInstance = GetObjectToInstantiate(instanceMessage);
                    GameObject objectInstance = Instantiate(objectToInstance, instanceMessage.toVector3(instanceMessage._position), Quaternion.identity);
                    NetworkedObject networkedInstance = objectInstance.GetComponent<NetworkedObject>();
                    networkedInstance.id = instanceMessage.objectId;
                    networkedObjects[networkedInstance.id] = networkedInstance;
                };
                functionsToRunInMainThread.Add(instantationAction);
                
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
            InstanceMessage enemyInstanceMessage = new InstanceMessage(MessageType.INSTATIATION, "-1", InstanceMessage.InstanceType.ENEMY, new Vector3(0.0f, 0.0f, 0.0f), 0.0f);
            messagesToSend.Add(enemyInstanceMessage);

            yield return new WaitForSeconds(3.0f);
        }
    }

    void ConnectPlayer(string id) {
        connectedPlayers.Add(new Player(id, 0.0f));
    }

    void DisconnectPlayer(string id) { 
        for(int i = 0; i < connectedPlayers.Count; ++i)
        {
            if(connectedPlayers[i].id == id)
            {
                //TODO: Disconnect player
                connectedPlayers.RemoveAt(i);
                break;
            }
        }
    }
}
