using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System;
using System.Collections.Generic;

public class TCPClient : MonoBehaviour
{
    public int id = 0;

    private Socket socket;
    private IPEndPoint endPoint;

    private Thread sendThread;
    private Thread receiveThread;
    private readonly int port = 7777;

    public UserList userlist;

    public TextLogControl logControl;
    private string messageToSend = null;

    float timeBetweenMessageChecks = 0.5f;
    public string username = null;

    bool chatOpen = false;
    public Dictionary<string, Command> commands;

    public Color color = Color.white;

    void Start()
    {
        username = null;

        try
        {
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);

            //Send a test message to check if the user has been properly connected
            byte[] tmp = new byte[1];
            socket.Send(tmp, 0, 0);

            logControl.LogText("Server", "Connected", -1, Color.magenta, DateTime.Now);
            Debug.Log("Connected to IP: " + endPoint.Address.ToString() + " with port: " + port.ToString());
        }
        catch
        {
            logControl.LogText("Server", "Could not connect properly to server, please restart the server", -1, Color.magenta, DateTime.Now);
            socket = null;
            //Close();
            return;
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

        chatOpen = true;

        sendThread = new Thread(new ThreadStart(StartSending));
        sendThread.Start();

        receiveThread = new Thread(new ThreadStart(StartReceiving));
        receiveThread.Start();

    }

    void StartSending()
    {
        //Receive();
        logControl.LogText("Server", "Please write a username", -1, Color.magenta, DateTime.Now);

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
          
            Message message = Message.DeserializeJson(msg);
            Debug.Log("Decoded message: " + message._message);
            ProcessMessage(message);

            return message;
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
           
            byte[] msg = _message.SerializeJson(id, username == null ? "None" : username, DateTime.Now, message, color);
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
        if (message._type == MessageType.COMMAND)
        {
            Debug.Log("Command: " + message._message);

            int index = message._message.IndexOf(" ");

            //We start at 1 to avoid "/"
            string commandName = message._message.Substring(1, index - 1);

            if (commands.ContainsKey(commandName))
            {
                message._username = "Server";
                commands[commandName].Execute(this, message);

                Debug.Log("Command: " + commandName + " executed");
            }
        }
        else
        {
            Debug.Log("User message: " + message._message);

            if (logControl != null)
            {
                logControl.LogText(message._username, message._message, message._userId, message._userColor, message._timestamp);
            }
        }
    }

    public void OnSendMessage(string message)
    {
        //Debug.Log(message);
        if (username == null)
        {
            messageToSend = "/changeName " + message;
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
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;

            Debug.Log("Socket closed");
        }
    }

    private void OnDestroy()
    {
        Close();

        //Close all threads
        if (sendThread != null)
        {
            sendThread.Abort();
        }

        if(receiveThread != null)
        {
            receiveThread.Abort();
        }
    }
}
