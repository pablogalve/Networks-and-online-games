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
    public Color color = Color.white;
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
    public List<Color> availableColors;

    private Thread listenThread;
    private Thread chatThread;

    private List<int> availableIds;

    bool serverOpen = true;

    public Dictionary<string, Command> commands;
    public Color color = Color.magenta;

    // Start is called before the first frame update
    void Start()
    {
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

        //Create a set of basic colors and some random ones just in case there are a lot of players
        Color[] colors = { Color.green, Color.yellow, Color.blue, Color.red, Color.magenta};
        availableColors = new List<Color>();
        availableColors.AddRange(colors);
        for(int i = availableColors.Count; i < maximumUsers; ++i)
        {
            availableColors.Add(new Color(UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255)));
        }
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

            //Clear the listen list so we can fill it in the next iteration
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
                //Get all users who are willing to send
                List<User> receiveList = SelectUsers();
                for (int i = 0; i < receiveList.Count; ++i)
                {
                    Message receivedMessage = ReceiveMessage(receiveList[i].socket);

                    //If the message is correct process it 
                    if (receivedMessage != null)
                    {
                        ProcessMessage(receivedMessage, receiveList[i]);
                        if (receivedMessage != null)
                        {
                            Debug.Log(receivedMessage._message);
                        }
                    }
                    else
                    {
                        //If there is an error receiving the message remove the user to avoid problems
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
            Message message = Message.DeserializeJson(msg);

            Debug.Log("Encoded message: " + message._message);

            return message;
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Exception caught: " + exception.ToString());
            return null;
        }
    }

    void ProcessMessage(Message message, User originUser)
    {
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

            //Check if it is a correct command and execute
            if (commands.ContainsKey(commandName))
            {
                message._username = "Server";
                commands[commandName].Execute(this, originUser, message);

                Debug.Log("Command: " + commandName + " executed");
            }
            //In the case it does not exist tell the user to send another one
            else
            {
                message.SerializeJson(-1, "Server", DateTime.Now, "Invalid command, please write one form the list. Type /help to see all commands", color);
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
            byte[] msg = message.Serialize();
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

        //Close all threads
        if (listenThread != null)
        {
            listenThread.Abort();
        }

        if(chatThread != null)
        {
            chatThread.Abort();
        }

        users.Clear();
    }
}
