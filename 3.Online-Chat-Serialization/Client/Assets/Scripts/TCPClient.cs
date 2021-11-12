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
    private readonly int port = 7777; //0 means take the first free port you get

    public UserList userlist;

    public TextLogControl logControl;
    private string messageToSend = null;

    float timeBetweenMessageChecks = 0.5f;
    public string username = null;

    bool chatOpen = false;
    public Dictionary<string, Command> commands;

    void Start()
    {
        username = null;
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(endPoint);
        logControl.LogText("Server", "Connected");

        Debug.Log("Connected to: " + endPoint.Address.ToString());

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
            _message.SerializeJson(id, username == null ? "None" : username, DateTime.Now, message);

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
                logControl.LogText(message._username, message._message);
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
