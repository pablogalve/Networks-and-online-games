using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPObject : MonoBehaviour
{
    public Socket socket;
    EndPoint senderRemote;

    int port = 7777;

    Thread receiveThread;
    Thread sendThread;

    bool active = true;

    List<Message> messagesToSend = new List<Message>();
    List<Action> functionsToRunInMainThread = new List<Action>();

    public virtual void Start()
    {
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
}
