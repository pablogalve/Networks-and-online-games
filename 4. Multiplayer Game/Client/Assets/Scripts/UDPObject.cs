using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public enum ConnectionType
{
    SERVER,
    CLIENT
}

public class UDPObject : MonoBehaviour
{
    public ConnectionType connectionType;
    
    [HideInInspector]
    public Dictionary<string, NetworkedObject> networkedObjects;

    public Socket socket;
    public EndPoint senderRemote;

    public int port = 7777;

    public Thread receiveThread;
    public Thread sendThread;

    bool active = true;

    public List<Message> messagesToSend = new List<Message>();
    public List<Action> functionsToRunInMainThread = new List<Action>();

    [Header("Instanceable Objects")]
    public GameObject playerProjectilePrefab = null;
    public GameObject enemyPrefab = null;

    public virtual void Start()
    {
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

    public virtual void Update()
    {
        while (functionsToRunInMainThread.Count > 0)
        {
            Action functionToRun = functionsToRunInMainThread[0];
            functionsToRunInMainThread.RemoveAt(0);
            functionToRun();
        }
    }

    public void AddFunctionToRun(Action actionToRun)
    { 
        functionsToRunInMainThread.Add(actionToRun);
    }

    public void StartSending()
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
                        int bytesSent = socket.SendTo(msg, msg.Length, SocketFlags.None, senderRemote);

                        //Debug.Log("Message sent!");
                        messagesToSend.RemoveAt(0);
                    }
                }
                catch (System.Exception exception)
                {
                    Debug.LogException(exception);
                    CloseSocket(socket);
                    active = false;
                }
            }
        }
    }

    public void SendMessage(Message messageToSend)
    {
        messagesToSend.Add(messageToSend);
    }

    public void StartReceiving()
    {
        while (active)
        {
            try
            {
                byte[] msg = new byte[256];

                int recv = socket.ReceiveFrom(msg, ref senderRemote);

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
                CloseSocket(socket);
                active = false;
            }
        }
    }

    public virtual void ProcessMessage(Message receivedMessage)
    { }

    private void CloseSocket(Socket socket)
    {
        if (socket != null)
        {
            socket.Close();
            Debug.LogWarning("Socket closed");
        }
    }

    private void OnDestroy()
    {
        CloseSocket(socket);
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

    public void InstantiateObject(string objectId, GameObject objectToInstantiate, Vector3 position)
    {
        Action instantationAction = () =>
        {
            GameObject objectInstance = Instantiate(objectToInstantiate, position, Quaternion.identity);
            NetworkedObject networkedInstance = objectInstance.GetComponent<NetworkedObject>();
            networkedInstance.id = objectId;
            networkedObjects[networkedInstance.id] = networkedInstance;
        };
        functionsToRunInMainThread.Add(instantationAction);
    }

    public virtual void DestroyObject(string objectId)
    {
        if(networkedObjects.ContainsKey(objectId))
        {
            Action destroyAction = () =>
            {
                Destroy(networkedObjects[objectId]);
                networkedObjects.Remove(objectId);
            };
            functionsToRunInMainThread.Add(destroyAction);
        }
        else
        {
            Debug.LogWarning("Trying to destroy an object which does not exist");
        }
    }

    public void SetObjectDesiredPosition(string objectId, Vector3 desiredPosition)
    {
        if(networkedObjects.ContainsKey(objectId))
        {
            networkedObjects[objectId].desiredPosition = desiredPosition;
        }
    }
}
