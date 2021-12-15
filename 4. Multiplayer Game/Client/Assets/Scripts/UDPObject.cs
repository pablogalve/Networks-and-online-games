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

    private void Start()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        senderRemote = (EndPoint)endPoint;

        socket.Bind(endPoint);

        sendThread = new Thread(Send);
        sendThread.Start();

        receiveThread = new Thread(new ThreadStart(Receive));
        receiveThread.Start();
    }

    public void Send()
    {

    }

    public void Receive()
    {

    }
}
