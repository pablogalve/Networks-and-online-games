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

    [Header("Instanceable Objects")]
    public GameObject playerProjectilePrefab;
    public GameObject enemyPrefab;

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
                //Send(MessageType.PING_PONG, null);
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
            case MessageType.INSTATIATION:
                InstanceMessage instanceMessage = receivedMessage as InstanceMessage;
                Action instantationAction = () =>
                {
                    GameObject objectToInstance = GetObjectToInstantiate(instanceMessage);
                    Instantiate(objectToInstance, instanceMessage.toVector3(instanceMessage._position), Quaternion.identity);
                };
                functionsToRunInMainThread.Add(instantationAction);
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

    public GameObject GetObjectToInstantiate(InstanceMessage instanceMessage)
    {
        switch (instanceMessage._instanceType)
        {
            case InstanceMessage.InstanceType.PLAYER_BULLET:
                return playerProjectilePrefab;

            case InstanceMessage.InstanceType.ENEMY:
                return enemyPrefab;

            default:
                return null;
        }
    }
}


