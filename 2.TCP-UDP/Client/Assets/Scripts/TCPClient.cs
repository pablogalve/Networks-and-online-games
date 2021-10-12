using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    private Socket socket;
    IPEndPoint endPoint;
    EndPoint Remote;

    Thread sendThread;

    int port = 7777; //0 means take the first free port you get

    bool startNewThread = false;
    int pongsReceived = 0;

    void Start()
    {
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        Remote = (EndPoint)endPoint;

        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Remote: " + endPoint.Address.ToString());

        socket.Connect(Remote);

        sendThread = new Thread(new ThreadStart(SendPing));
        sendThread.Start();
    }

    void Update()
    {
        if (startNewThread)
        {
            startNewThread = false;
            sendThread = new Thread(new ThreadStart(SendPing));
            sendThread.Start();
        }
    }

    void SendPing()
    {
        try
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes("Ping");
            int bytesCount = socket.Send(msg, msg.Length, SocketFlags.None);

            ReceivePong();

            if(pongsReceived < 5)
            {
                startNewThread = true;
            }
            else
            {
                Debug.Log("Enough ping pong for today, disconnecting");
                Close();
            }
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error. Couldn't send message: " + exception.ToString());
            Close();
        }

    }

    void ReceivePong()
    {
        //Debug.Log("Trying to receive a message: ");
        byte[] msg = new byte[256];
        var recv = socket.Receive(msg);
        string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);

        Debug.Log(decodedMessage);

        pongsReceived++;

        Thread.Sleep(500);
    }

    void Shutdown()
    {
        socket.Shutdown(SocketShutdown.Both);
        Debug.Log("Socket shut down");
    }

    void Close()
    {
        socket.Close();
        Debug.Log("Socket closed");
    }

    private void OnDestroy()
    {
        socket.Close();
        sendThread.Abort();
    }
}
