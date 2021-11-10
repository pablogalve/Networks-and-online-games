using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

class User
{
    public User(int _uid, Socket _socket)
    {
        uid = _uid;
        socket = _socket;
    }

    public int uid;
    public string username = "No username";
    public Socket socket;
}

public class TCPServer : MonoBehaviour
{
    private Dictionary<int, User> users;
    public int acceptWaitTime = 5;

    private readonly int port = 7777;
    private int maximumSockets = 1;

    private Socket[] listenSockets;
    private Socket[] acceptSockets;
    private Socket[] receiveSockets;

    private ArrayList listenList;
    private ArrayList acceptList;

    private Thread listenThread;

    // Start is called before the first frame update
    void Start()
    {
        listenSockets = new Socket[maximumSockets];
        listenList = GenerateSocketsArrayList(listenSockets);

        acceptSockets = new Socket[maximumSockets];
        acceptList = GenerateSocketsArrayList(acceptSockets);

        listenThread = new Thread(new ThreadStart(ListenForUsers));
        listenThread.Start();

        users = new Dictionary<int, User>();
    }

    void ListenForUsers()
    {
        Debug.Log("Binding and listening...");
        for (int i = 0; i < maximumSockets; i++)
        {
            listenList[i] = new Socket(AddressFamily.InterNetwork,
                                       SocketType.Stream,
                                       ProtocolType.Tcp);

            ((Socket)listenList[i]).Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port + i));
            ((Socket)listenList[i]).Listen(10);
        }
        Debug.Log("Binding and listening completed");
        Debug.Log("Accepting");

        Thread.Sleep(acceptWaitTime * 1000);

        Socket.Select(listenList, null, null, 1000);

        for (int i = 0; i < listenList.Count; i++)
        {
            Socket socket = (Socket)listenList[i];
            acceptList[i] = socket.Accept();
            Debug.Log("Accepted");
            //SendServerMessage(socket, "/id " + i);
            //int id = Random.Range(0, 1000);
            users[i] = new User(i, socket);
        }

        //Debug.Log("Listen List: " + listenList.Count.ToString());
        //Debug.Log("Accept List: " + acceptList.Count.ToString());

        StartChat();
    }

    private void StartChat()
    {
        for (int j = 0; j < 5; ++j)
        {
            Debug.Log("Checking for messages");

            if (acceptList.Count <= 0)
            {
                return;
            }

            //Copy sockets
            receiveSockets = new Socket[acceptList.Count];
            for (int i = 0; i < acceptList.Count; ++i)
            {
                receiveSockets[i] = (Socket)acceptList[i];
            }

            //Generate a new list to receive messages
            ArrayList receiveList = GenerateSocketsArrayList(receiveSockets);
            receiveList = Select(receiveList);
            for (int i = 0; i < receiveList.Count; ++i)
            {
                Socket socket = (Socket)receiveList[i];
                Message receivedMessage = ReceiveMessage(socket);

                ProcessMessage(receivedMessage, socket);
            }

            Thread.Sleep(250);
        }
    }

    Message ReceiveMessage(Socket socket)
    {
        try
        {
            //Debug.Log("Trying to receive a message: ");
            byte[] msg = new byte[256];
            int recv = socket.Receive(msg);
            string encodedMessage = System.Text.Encoding.ASCII.GetString(msg);
            Message message = Message.DeserializeJson(encodedMessage);

            Debug.Log("Message: " + encodedMessage);

            return message;
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Exception caught: " + exception.ToString());
            Close(socket);
            return null;
        }
    }

    void ProcessMessage(Message message, Socket socket)
    {
        if(message._type == MessageType.COMMAND)
        {
            int index = message._message.IndexOf(" ");
            string command = message._message.Substring(0, index);
            Debug.Log("Command: " + command);

            switch (command)
            {
                case "/setUsername":
                    string username = message._message.Substring(index, message._message.Length - index);
                    Debug.Log("Username: " + username);
                    break;

                default:
                    break;
            }
        }
        else
        {
            //Add difference between commands and messages
            SendToEveryone(message);
        }
    }

    void Send(Socket socket, string message)
    {
        try
        {
            //Debug.Log("Sending Pong");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
            int bytesSent = socket.Send(msg, msg.Length, SocketFlags.None);

            if (bytesSent > 0)
            {
                Debug.Log("Bytes sent: " + bytesSent.ToString());
            }
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Error. Couldn't send message: " + exception.ToString());
            Close(socket);
            acceptList.Remove(socket);
        }
    }

    void SendToEveryone(Message messageToSend)
    {
        foreach (KeyValuePair<int, User> user in users)
        {
            Send(user.Value.socket, messageToSend.json);
        }
    }

    void SendServerMessage(Socket socket, string message)
    {
        Message _message = new Message();
        _message.SerializeJson(-1, DateTime.Now, message);
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(_message.json);
        Debug.Log("Want to send message");
        int bytesCount = socket.Send(msg, msg.Length, SocketFlags.None);
        Debug.Log("Message sent with: " + bytesCount.ToString());
    }

    ArrayList GenerateSocketsArrayList(Socket[] sockets)
    {
        ArrayList arrayList = new ArrayList();
        for (int i = 0; i < sockets.Length; ++i)
        {
            arrayList.Add(sockets[i]);
        }

        return arrayList;
    }

    ArrayList Select(ArrayList arrayToBeSelected)
    {
        Socket.Select(arrayToBeSelected, null, null, 1000);
        return arrayToBeSelected;
    }

    void Close(Socket socket)
    {
        if (socket != null)
        {
            socket.Close();
            Debug.Log("Socket closed");
        }
    }

    private void CloseSockets(Socket[] sockets)
    {
        for (int i = 0; i < sockets.Length; ++i)
        {
            Close(sockets[i]);
        }
    }

    private void OnDestroy()
    {
        //Close server
        CloseSockets(listenSockets);
        CloseSockets(acceptSockets);
        CloseSockets(receiveSockets);


        if (listenThread != null)
        {
            listenThread.Abort();
        }

        // Close clients
        /*
        foreach (KeyValuePair<int, User> user in users)
        {
            user.Value.receiveThread.Abort();
            user.Value.socket.Close();
        }
        */
        //users.Clear();
    }
}
