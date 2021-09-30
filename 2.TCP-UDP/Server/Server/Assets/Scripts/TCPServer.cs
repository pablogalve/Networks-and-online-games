using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    private Socket socket;
    private Socket client;
    IPEndPoint ipep;

    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        int port = 25;
        ipep = new IPEndPoint(IPAddress.Parse("95.17.95.173"), port);
        socket.Bind(ipep);

        socket.Listen(10);
        client = socket.Accept();
        socket.Connect(ipep);
    }

    void StopDataTransfer()
    {
        socket.Shutdown(SocketShutdown.Both);
    }

    private void OnDestroy()
    {
        socket.Close();
    }
}
