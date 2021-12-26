using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    protected int port = 7777;

    public Thread receiveThread;
    public Thread sendThread;

    protected bool active = true;

    public List<Message> messagesToSend = new List<Message>();
    public List<Action> functionsToRunInMainThread = new List<Action>();

    [Header("Instanceable Objects")]
    public GameObject playerProjectilePrefab = null;
    public GameObject enemyProjectilePrefab = null;
    public GameObject enemyPrefab = null;

    public virtual void Start()
    {
        networkedObjects = new Dictionary<string, NetworkedObject>();
    }

    public void ConnectionConfirmed()
    {
        receiveThread = new Thread(new ThreadStart(StartReceiving));
        receiveThread.Start();

        sendThread = new Thread(new ThreadStart(StartSending));
        sendThread.Start();
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

    public virtual void StartSending() 
    {}

    public void SendMessage(Message messageToSend)
    {
        messagesToSend.Add(messageToSend);
    }

    public virtual void StartReceiving()
    {}

    public virtual void ProcessMessage(Message receivedMessage, EndPoint clientSocket = null)
    {}

    private void CloseSocket(Socket socket)
    {
        if (socket != null)
        {
            socket.Close();
            Debug.LogWarning("Socket closed");
            socket = null;
        }
    }

    private void OnDestroy()
    {
        messagesToSend.Clear();
        CloseSocket(socket);
        active = false;
    }

    public GameObject GetObjectToInstantiate(InstanceMessage.InstanceType instanceType)
    {
        switch (instanceType)
        {
            case InstanceMessage.InstanceType.PLAYER_BULLET:
                return playerProjectilePrefab;

            case InstanceMessage.InstanceType.ENEMY_BULLET:
                return enemyProjectilePrefab;

            case InstanceMessage.InstanceType.ENEMY:
                return enemyPrefab;

            default:
                return null;
        }
    }

    public void InstantiateObject(string objectId, GameObject objectToInstantiate, Vector3 position, Quaternion rotation)
    {
        Action instantationAction = () =>
        {
            if(connectionType==ConnectionType.SERVER)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Server"));
            }
            else
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("SampleScene"));
            }

            GameObject objectInstance = Instantiate(objectToInstantiate, position, rotation);
            NetworkedObject networkedInstance = objectInstance.GetComponent<NetworkedObject>();

            networkedInstance.udpObject = this;

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
                NetworkedObject networkedObject = networkedObjects[objectId];
                if (networkedObject.networkedObjectType == NetworkedObjectType.PLAYER)
                {
                    Player player = networkedObject as Player;
                    player.DecreaseLives(1);

                }
                else
                {
                    networkedObjects[objectId].Die();
                    Destroy(networkedObjects[objectId].gameObject);
                    networkedObjects.Remove(objectId);
                }
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
