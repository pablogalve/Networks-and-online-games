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
    public string messageToSend = "Ping";

    public UserList userlist;

    void Start()
    {
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        Remote = (EndPoint)endPoint;

        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Remote: " + endPoint.Address.ToString());

        socket.Connect(Remote);

        sendThread = new Thread(new ThreadStart(Connect));
        sendThread.Start();
    }

    void Update()
    {
        if (startNewThread)
        {
            startNewThread = false;
            sendThread = new Thread(new ThreadStart(Connect));
            sendThread.Start();
        }
    }

    void Connect()
    {
        try
        {
            GetUsers();

            for(int i = 0; i < 5; ++i)
            {
                Send(messageToSend);

                Thread.Sleep(1000);

                Receive();
            }
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error. Couldn't send message: " + exception.ToString());
            Close();
        }
    }

    void GetUsers()
    {
        /*
        var t = new testClass();
        t.hp = 40;
        t.pos = new List<int> { 10, 3, 12 };
        string json = JsonUtility.ToJson(t);
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);
        */

        Send("getUsers");

        byte[] msg = new byte[256];
        var recv = socket.Receive(msg);
        string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);
    }

    void Receive()
    {
        byte[] msg = new byte[256];
        var recv = socket.Receive(msg);
        string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);

        Debug.Log(decodedMessage);
    }

    void Send(string message)
    {
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
        int bytesCount = socket.Send(msg, msg.Length, SocketFlags.None);
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
