using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System;

public class TCPClient : MonoBehaviour
{
    private Socket socket;
    private IPEndPoint endPoint;
    private EndPoint Remote;

    private Thread sendThread;
    private readonly int port = 7777; //0 means take the first free port you get

    private bool startNewThread = false;
    public string messageToSend = "Ping";

    public UserList userlist;

    void Start()
    {
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        Remote = (EndPoint)endPoint;

        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Remote: " + endPoint.Address.ToString());

        /*
        socket.Connect(Remote);

        sendThread = new Thread(new ThreadStart(Connect));
        sendThread.Start();
        */
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

            for (int i = 0; i < 5; ++i)
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
        Send("/getUsers");

        byte[] msg = new byte[512];
        var recv = socket.Receive(msg);
        string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);
    }

    void Receive()
    {
        byte[] msg = new byte[512];
        var recv = socket.Receive(msg);
        string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);

        Message message = Message.DeserializeJson(decodedMessage);
        ProcessMessage(message);
        //Debug.Log(message.json);
    }

    void Send(string message)
    {
        Message _message = new Message();
        _message.SerializeJson(0, DateTime.Now, message);
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(_message.json);
        int bytesCount = socket.Send(msg, msg.Length, SocketFlags.None);
    }

    void ProcessMessage(Message message)
    {
        if (message._id == -1)
        {
            Debug.Log("Server message");
        }
        else
        {
            Debug.Log("User message");
        }
    }

    public void OnSendMessage(String message)
    {
        Debug.Log(message);
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
        if (sendThread != null) {
            sendThread.Abort();
        }
    }
}
