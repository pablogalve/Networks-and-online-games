using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System;

public class TCPClient : MonoBehaviour
{
    private int id = 0;

    private Socket socket;
    private IPEndPoint endPoint;

    private Thread sendThread;
    private readonly int port = 7777; //0 means take the first free port you get

    public UserList userlist;

    public TextLogControl logControl;
    private string messageToSend = "";

    void Start()
    {
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Remote: " + endPoint.Address.ToString());

        socket.Connect(endPoint);

        sendThread = new Thread(new ThreadStart(Chat));
        sendThread.Start();
    }

    void Chat()
    {
        //Receive();

        for (int i = 0; i < 5; ++i)
        {
            try
            {
                if (messageToSend != null)
                {
                    //Debug.Log("Wants to send:" + messageToSend);
                    if(i == 0)
                    {
                        Send("/setUsername marcpages2020");
                    }
                    else
                    {
                        Send("Ping");
                    }
                    //messageToSend = null;
                }
            }
            catch (System.Exception exception)
            {
                Debug.Log("Error in send: " + exception.ToString());
                Close();
            }

            try
            {
                Receive();
            }
            catch (System.Exception exception)
            {
                Debug.Log("Error in receive: " + exception.ToString());
                Close();
            }
 
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
        Debug.Log("Waiting to receive");
        byte[] msg = new byte[512];
        var recv = socket.Receive(msg);
        string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);
        Debug.Log("Decoded message: " + decodedMessage);

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
        //Debug.Log("Message sent with: " + bytesCount + "bytes");
    }

    void ProcessMessage(Message message)
    {
        if (message._id == -1)
        {
            Debug.Log("Server message" + message._message);

            int index = message._message.IndexOf(" ");
            string command = message._message.Substring(0, index);

            switch (command)
            {
                case "/id":
                    string idString = message._message.Substring(index + 1, message._message.Length);
                    id = Int32.Parse(idString);
                    Debug.Log("Id: " + id.ToString());
                    break;

                default:
                    break;
            }
        }
        else
        {
            Debug.Log("User message: " + message._message);
        }

        if (logControl != null)
        {
            logControl.LogText("marcpages2020", message._message);
        }
    }

    public void OnSendMessage(string message)
    {
        //Debug.Log(message);
        messageToSend = message;
    }

    void Shutdown()
    {
        socket.Shutdown(SocketShutdown.Both);
        Debug.Log("Socket shut down");
    }

    void Close()
    {
        if (socket != null)
        {
            socket.Close();
            Debug.Log("Socket closed");
        }
    }

    private void OnDestroy()
    {
        Close();
        if (sendThread != null)
        {
            sendThread.Abort();
        }
    }
}
