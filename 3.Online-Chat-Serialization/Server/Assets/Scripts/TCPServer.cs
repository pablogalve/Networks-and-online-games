using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class User
{
    public User(int _uid, Socket _socket)
    {
        id = _uid;
        socket = _socket;
    }

    public int id = -1;
    public string username = "No username";
    public Socket socket = null;
}

public class TCPServer : MonoBehaviour
{
    private List<User> users;
    public int acceptWaitTime = 5;

    private readonly int port = 7777;
    private int maximumSockets = 1;

    private Socket[] listenSockets;
    private Socket[] acceptSockets;

    private ArrayList listenList;
    private ArrayList acceptList;

    private Thread listenThread;
    private List<int> availableIds;

    bool serverOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        listenThread = new Thread(new ThreadStart(ListenForUsers));
        listenThread.Start();

        users = new List<User>();

        availableIds = new List<int>();
        for (int i = 0; i < maximumSockets; ++i)
        {
            availableIds.Add(UnityEngine.Random.Range(0, int.MaxValue));
        }
    }

    void ListenForUsers()
    {
        Debug.Log("Binding and listening...");

        listenSockets = new Socket[maximumSockets];
        listenList = GenerateSocketsArrayList(listenSockets);

        acceptSockets = new Socket[maximumSockets];
        acceptList = GenerateSocketsArrayList(acceptSockets);

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

            //Create user with a random Id
            users.Add(new User(availableIds[0], (Socket)acceptList[i]));
            availableIds.RemoveAt(0);
        }

        if (users.Count > 0)
        {
            serverOpen = true;
        }

        StartChat();
    }

    private void StartChat()
    {
        while (serverOpen)
        {
            //Debug.Log("Checking for messages");

            if (acceptList.Count <= 0)
            {
                serverOpen = false;
            }

            List<User> receiveList = SelectUsers();
            for (int i = 0; i < receiveList.Count; ++i)
            {
                Message receivedMessage = ReceiveMessage(receiveList[i].socket);

                if (receivedMessage != null)
                {
                    ProcessMessage(receivedMessage, receiveList[i]);
                }
                Debug.Log(receivedMessage.json);
            }

            Thread.Sleep(250);
        }

        Debug.Log("Exiting the chat loop");
    }

    Message ReceiveMessage(Socket socket)
    {
        try
        {
            //Debug.Log("Trying to receive a message: ");
            byte[] msg = new byte[256];
            int recv = socket.Receive(msg);
            string encodedMessage = System.Text.Encoding.ASCII.GetString(msg);
            //Debug.Log("Encoded message: " + encodedMessage);
            Message message = Message.DeserializeJson(encodedMessage);
            Debug.Log("Decoded message: " + encodedMessage);

            return message;
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Exception caught: " + exception.ToString());
            Close(socket);
            return null;
        }
    }

    void ProcessMessage(Message message, User originUser)
    {
        message._userId = originUser.id;

        if (message._type == MessageType.COMMAND)
        {
            message._username = "Server";

            int index = message._message.IndexOf(" ");
            string command = message._message.Substring(0, index);
            string content = message._message.Substring(index, message._message.Length - index);

            Debug.Log("Command: " + command);

            switch (command)
            {
                case "/setUsername":
                    bool usernameFound = false;

                    //Debug.Log("Username: " + username);

                    //OK
                    message._userId = -1;
                    message._returnCode = 200;
                    string username = content;
                    
                    //Iterate all users to check if it is available
                    for (int i = 0; i < users.Count; ++i)
                    {
                        if (users[i].username == username)
                        {
                            Debug.Log("Username already taken");

                            //Bad request, username already taken
                            message._returnCode = 400;
                            message._message = "Username not available";
                            usernameFound = true;
                            break;
                        }
                    }

                    if (!usernameFound)
                    {
                        originUser.username = username;
                        Debug.Log("Username: " + username + " accepted");
                    }
                    
                    message.Serialize();
                    Send(originUser, message);

                    break;

                default:
                    break;
            }
        }
        else
        {
            SendToEveryone(message);
        }
    }

    void Send(User user, Message message)
    {
        if (message._message.Length <= 0)
        {
            Debug.LogWarning("Error sending message, length <= 0");
            return;
        }

        try
        {
            //Debug.Log("Sending Pong");
            message.Serialize();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(message.json);
            int bytesSent = user.socket.Send(msg, msg.Length, SocketFlags.None);

            if (bytesSent > 0)
            {
                //Debug.Log("Bytes sent: " + bytesSent.ToString());
            }
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Error. Couldn't send message: " + exception.ToString());
            Close(user.socket);
            acceptList.Remove(user.socket);
            users.Remove(user);
        }
    }

    void SendToEveryone(Message messageToSend)
    {
        for (int i = 0; i < users.Count; ++i)
        {
            Send(users[i], messageToSend);
        }
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

    List<User> SelectUsers()
    {
        List<User> selectedUsers = new List<User>();

        //Copy sockets
        Socket[] receiveSockets = new Socket[users.Count];
        for (int i = 0; i < users.Count; ++i)
        {
            receiveSockets[i] = users[i].socket;
        }

        //Generate a new list to receive messages
        ArrayList receiveList = GenerateSocketsArrayList(receiveSockets);

        //Select the ones that have pending messages
        receiveList = Select(receiveList);

        for (int i = 0; i < receiveList.Count; ++i)
        {
            for (int j = 0; j < users.Count; ++j)
            {
                if (receiveList[i] == users[j].socket)
                {
                    selectedUsers.Add(users[j]);
                }
            }
        }

        return selectedUsers;
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

        if (listenThread != null)
        {
            listenThread.Abort();
        }

        // Close clients
        users.Clear();
    }
}
