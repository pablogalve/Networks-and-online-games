using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : UDPObject
{
    public override void Start()
    {
        base.Start();
        StartCoroutine(SpawnEnemy());
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
                //Debug.Log("Pong Sent to client!");
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
}
