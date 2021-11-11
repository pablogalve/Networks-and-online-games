using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System;
using System.Collections;

public class TCPClient : MonoBehaviour
{
    private int id = 0;

    private Socket socket;
    private IPEndPoint endPoint;

    private Thread sendThread;
    private Thread receiveThread;
    private readonly int port = 7777; //0 means take the first free port you get

    public UserList userlist;

    public TextLogControl logControl;
    private string messageToSend = null;

    float timeBetweenMessageChecks = 0.5f;
    string username = null;

    bool chatOpen = false;

    void Start()
    {
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(endPoint);

        Debug.Log("Remote: " + endPoint.Address.ToString());

        chatOpen = true;

        sendThread = new Thread(new ThreadStart(StartSending));
        sendThread.Start();

        receiveThread = new Thread(new ThreadStart(StartReceiving));
        receiveThread.Start();
    }

    void StartSending()
    {
        //Receive();
        logControl.LogText("Server", "Please write a username");

        Thread.Sleep(2500);

        while (chatOpen)
        {
            if (messageToSend != null)
            {
                Send(messageToSend);
                messageToSend = null;
            }

            Thread.Sleep((int)(timeBetweenMessageChecks * 1000.0f));
        }
    }

    void StartReceiving()
    {
        Thread.Sleep(2500);

        while (chatOpen)
        {
            Receive();
        }

        Thread.Sleep((int)(timeBetweenMessageChecks * 1000.0f));
    }

    Message Receive()
    {
        try
        {
            Debug.Log("Waiting to receive");
            byte[] msg = new byte[512];
            var recv = socket.Receive(msg);
            string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);
            Debug.Log("Decoded message: " + decodedMessage);

            Message message = Message.DeserializeJson(decodedMessage);
            ProcessMessage(message);
            return message;
            //Debug.Log(message.json);
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error in receive: " + exception.ToString());
            Close();
            chatOpen = false;
            return null;
        }
    }

    void Send(string message)
    {
        try
        {
            Message _message = new Message();
            _message.SerializeJson(id, username == null ? "" : username, DateTime.Now, message);

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(_message.json);
            int bytesCount = socket.Send(msg, msg.Length, SocketFlags.None);

            Debug.Log("Message sent with: " + bytesCount + "bytes");
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error sending message: " + exception.ToString());
            Close();
        }
    }

    void ProcessMessage(Message message)
    {
        if (message._userId == -1)
        {
            Debug.Log("Server message: " + message._message);

            int index = message._message.IndexOf(" ");
            string command = message._message.Substring(0, index);

            string serverMessage = "a";

            switch (command)
            {
                case "/id":
                    string idString = message._message.Substring(index, message._message.Length);
                    id = Int32.Parse(idString);
                    Debug.Log("Id: " + id.ToString());
                    break;

                case "/setUsername":

                    //OK
                    if (message._returnCode == 200)
                    {
                        string messageUsername = message._message.Substring(index, message._message.Length - index);
                        username = messageUsername;

                        serverMessage = "Username set to: " + username;
                    }
                    else
                    {
                        serverMessage = "Username could not be set";
                    }
                    Debug.Log(serverMessage);
                    break;

                default:
                    break;
            }

            if (logControl != null)
            {
                logControl.LogText("Server", serverMessage);
            }
        }
        else
        {
            Debug.Log("User message: " + message._message);

            if (logControl != null)
            {
                logControl.LogText(message._username, message._message);
            }
        }
    }

    public void OnSendMessage(string message)
    {
        //Debug.Log(message);
        if (username == null)
        {
            messageToSend = "/setUsername " + message;
        }
        else
        {
            messageToSend = message;
        }
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
