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
    [HideInInspector]
    public List<User> users;

    public int acceptWaitTime = 5;

    private readonly int port = 7777;
    private int maximumSockets = 1;
    private int maximumUsers = 30;

    List<int> availablePorts;
    private List<Socket> listenSockets;
    private List<Socket> listenList;

    private Thread listenThread;
    private Thread chatThread;

    private List<int> availableIds;

    bool serverOpen = true;

    public Dictionary<string, Command> commands;
    Message auxiliarMessage;

    // Start is called before the first frame update
    void Start()
    {
        auxiliarMessage = new Message();
        listenSockets = new List<Socket>();
        listenList = new List<Socket>();

        availablePorts = new List<int>();
        for (int i = 0; i < maximumSockets; ++i)
        {
            availablePorts.Add(port + i);
        }

        //Create a command for each type
        commands = new Dictionary<string, Command>();
        foreach (var assemblies in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assemblies.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Command)))
                {
                    Command cmd = (Command)Activator.CreateInstance(type);
                    commands[cmd.name] = cmd;
                }
            }
        }

        //Generate random ids to identificate users
        availableIds = new List<int>();
        for (int i = 0; i < maximumUsers; ++i)
        {
            availableIds.Add(UnityEngine.Random.Range(0, int.MaxValue));
        }

        users = new List<User>();
        listenThread = new Thread(new ThreadStart(ListenForUsers));
        listenThread.Start();
    }

    void ListenForUsers()
    {
        Debug.Log("Binding...");
        for (int i = 0; i < availablePorts.Count; i++)
        {
            listenSockets.Add(new Socket(AddressFamily.InterNetwork,
                                       SocketType.Stream,
                                       ProtocolType.Tcp));

            ((Socket)listenSockets[i]).Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), availablePorts[i]));
        }
        Debug.Log("Binding completed");

        bool chatStarted = false;

        while (serverOpen)
        {
            //Debug.Log("Listening");
            for (int i = 0; i < listenSockets.Count; i++)
            {
                listenSockets[i].Listen(10);
                listenList.Add(listenSockets[i]);
            }

            if (listenList.Count > 0)
            {
                Socket.Select(listenList, null, null, 1000);
            }

            //Accept new clients
            for (int i = 0; i < listenList.Count; i++)
            {
                Socket socket = listenList[i].Accept();
                // = acceptList[i];
                Debug.Log("Accepted");

                //Create user with a random Id
                users.Add(new User(availableIds[0], socket));
                availableIds.RemoveAt(0);

                //Remove the port from the available ones
                availablePorts.Remove(GetPort(socket));

                //Remove also from listenning sockets to avoid being used again
                //listenSockets.Remove(listenList[i]);
            }

            listenList.Clear();

            if (!chatStarted)
            {
                chatStarted = true;
                chatThread = new Thread(new ThreadStart(StartChat));
                chatThread.Start();
            }

            Thread.Sleep(acceptWaitTime * 1000);
        }

    }

    private void StartChat()
    {
        while (serverOpen)
        {
            //Debug.Log("Checking for messages");

            if (users.Count > 0)
            {
                List<User> receiveList = SelectUsers();

                for (int i = 0; i < receiveList.Count; ++i)
                {
                    Message receivedMessage = ReceiveMessage(receiveList[i].socket);

                    if (receivedMessage != null)
                    {
                        ProcessMessage(receivedMessage, receiveList[i]);
                        if (receivedMessage != null)
                        {
                            Debug.Log(receivedMessage.json);
                        }
                    }
                    else
                    {
                        RemoveUser(receiveList[i]);
                    }
                }
            }

            Thread.Sleep(250);
        }

        Debug.Log("Exiting the chat loop");
    }

    public User GetUserByName(string username)
    {
        for(int i = 0; i < users.Count; ++i)
        {
            if (users[i].username == username)
                return users[i];
        }

        return null;
    }

    Message ReceiveMessage(Socket socket)
    {
        try
        {
            byte[] msg = new byte[512];
            int recv = socket.Receive(msg);
            string encodedMessage = System.Text.Encoding.ASCII.GetString(msg);
            Message message = Message.DeserializeJson(encodedMessage);

            Debug.Log("Encoded message: " + encodedMessage);

            return message;
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Exception caught: " + exception.ToString());
            //CloseSocket(socket);
            return null;
        }
    }

    void ProcessMessage(Message message, User originUser)
    {
        message._userId = originUser.id;

        if (message._type == MessageType.COMMAND)
        {
            int index = message._message.IndexOf(" ");
            //We start at 1 to avoid "/"
            string commandName;

            if (index != -1)
            {
                commandName = message._message.Substring(1, index - 1);
            }
            else
            {
                commandName = message._message.Substring(1);
            }

            if (commands.ContainsKey(commandName))
            {
                message._username = "Server";
                commands[commandName].Execute(this, originUser, message);

                Debug.Log("Command: " + commandName + " executed");
            }
            else
            {
                message.SerializeJson(-1, "Server", DateTime.Now, "Invalid command, please write one form the list. Type /help to see all commands");
                message._returnCode = 404;

                Send(originUser, message);
            }            
        }
        else
        {
            SendToEveryone(message, null);
        }
    }

    public void Send(User user, Message message)
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
            CloseSocket(user.socket);
            users.Remove(user);
        }
    }

    public void SendToEveryone(Message messageToSend, User userToIgnore)
    {
        for (int i = 0; i < users.Count; ++i)
        {
            if (users[i] == null || users[i] != userToIgnore)
            {
                Send(users[i], messageToSend);
            }
        }
    }

    List<User> SelectUsers()
    {
        List<User> selectedUsers = new List<User>();

        //Copy sockets
        List<Socket> receiveSockets = new List<Socket>();
        for (int i = 0; i < users.Count; ++i)
        {
            receiveSockets.Add(users[i].socket);
        }

        //Select the ones that have pending messages
        Socket.Select(receiveSockets, null, null, 1000);

        for (int i = 0; i < receiveSockets.Count; ++i)
        {
            for (int j = 0; j < users.Count; ++j)
            {
                if (receiveSockets[i] == users[j].socket)
                {
                    selectedUsers.Add(users[j]);
                }
            }
        }

        return selectedUsers;
    }

    public bool RemoveUser(User user)
    {
        if (user == null)
            return false;

        users.Remove(user);

        auxiliarMessage.SerializeJson(-1, "Server", DateTime.Now, "User: " + user.username + " has left the room");
        SendToEveryone(auxiliarMessage, null);
        CloseSocket(user.socket);

        Debug.Log("User: " + user.username + " kicked");
        return true;
    }

    int GetPort(Socket socket)
    {
        IPEndPoint ipEndPoint = socket.LocalEndPoint as IPEndPoint;
        if (ipEndPoint != null)
        {
            return ipEndPoint.Port;
        }
        else
        {
            Debug.LogWarning("Trying to get port when no IPEndPoint is set");
            return -1;
        }
    }

    void CloseSocket(Socket socket)
    {
        if (socket != null)
        {
            socket.Close();
            Debug.Log("Socket closed");
        }
    }

    private void CloseSockets(List<Socket> sockets)
    {
        for (int i = 0; i < sockets.Count; ++i)
        {
            CloseSocket(sockets[i]);
        }
    }

    private void OnDestroy()
    {
        //Close server
        CloseSockets(listenSockets);
        //CloseSockets(acceptSockets);

        if (listenThread != null)
        {
            listenThread.Abort();
        }

        // Close clients
        users.Clear();
    }
}
