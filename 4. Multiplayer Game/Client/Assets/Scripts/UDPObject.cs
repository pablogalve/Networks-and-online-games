using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPObject : MonoBehaviour
{
    Socket socket;
    EndPoint senderRemote;

    int port = 7777;

    Thread receiveThread;
    Thread sendThread;

    bool active = true;

    List<Message> messagesToSend = new List<Message>();

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

    public void StartSending()
    {
        while (active)
        {
            for(int i = 0; i < messagesToSend.Count; ++i)
            {
                try
                {
                    byte[] msg = messagesToSend[i].Serialize();
                    int bytesSent = socket.SendTo(msg, msg.Length, SocketFlags.None, senderRemote);
                }
                catch (System.Exception exception)
                {
                    Debug.LogException(exception);
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

                var recv = socket.ReceiveFrom(msg, ref senderRemote);
                Message receivedMessage = Message.Deserialize(msg);

                ProcessMessage(receivedMessage);
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
            }

        }
    }

    public virtual void ProcessMessage(Message receivedMessage)
    {}

    private void CloseSocket(Socket socket)
    {
        socket.Close();
        Debug.LogWarning("Socket closed");
    }

    private void OnDestroy()
    {
        CloseSocket(socket);
    }
}
