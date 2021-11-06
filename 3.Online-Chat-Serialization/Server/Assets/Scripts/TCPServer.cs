using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

struct User
{
    public int uid;
    public string username;
    public Socket socket;
    public Thread receiveThread;
}

public class TCPServer : MonoBehaviour
{
    private Socket socket;

    private IPEndPoint endPoint;

    private readonly int port = 7777;

    private Thread listenThread;
    private Dictionary<int, User> users;

    private int maxListeningClients = 5;

    public string messageToSend = "Pong";

    // Start is called before the first frame update
    void Start()
    {
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        socket.Bind(endPoint);

        listenThread = new Thread(new ThreadStart(ListenForUsers));
        listenThread.Start();

        users = new Dictionary<int, User>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ListenForUsers()
    {
        //Listen for a single client
        try
        {
            Debug.Log("Listening for users to connect");
            socket.Listen(maxListeningClients);

            for (int i = 0; i < maxListeningClients; ++i)
            {
                User newUser = new User();

                Socket clientSocket = socket.Accept();
                Debug.Log("Client accepted");

                newUser.uid = i; 
                newUser.socket = clientSocket;
                users[newUser.uid] = newUser;

                newUser.receiveThread = new Thread(new ParameterizedThreadStart(Chat));
                newUser.receiveThread.Start(newUser);
            }
        }
        catch (System.Exception exception)
        {
            Debug.Log(exception.ToString());
            Close();
        }
    }

    private void Chat(object objectUser)
    {
        try
        {
            User user = (User)objectUser;
            for (int i = 0; i < maxListeningClients; ++i)
            {
                //Debug.Log("Trying to receive a message: ");
                byte[] msg = new byte[256];

                int recv = user.socket.Receive(msg);

                string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);
                messageToSend = decodedMessage;
                Debug.Log("Message: " + decodedMessage);

                Thread.Sleep(1000);

                Send(user);

                //Close();
            }
        }
        catch (System.Exception exception)
        {
            Debug.Log("Exception caught: " + exception.ToString());
        }
    }

    void Send(User user)
    {
        try
        {
            //Debug.Log("Sending Pong");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(messageToSend);
            int bytesSent = user.socket.Send(msg, msg.Length, SocketFlags.None);
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error. Couldn't send message: " + exception.ToString());
            Close();
        }
    }
    
    void Close()
    {
        socket.Close();
        Debug.Log("Socket closed");
    }

    private void OnDestroy()
    {
        //Close server
        socket.Close();
        if (listenThread != null)
        {
            listenThread.Abort();
        }

        // Close clients
        foreach (KeyValuePair<int, User> user in users)
        {
            user.Value.receiveThread.Abort();
            user.Value.socket.Close();
        }
        users.Clear();
    }

    private void SendMessage()
    {

    }
}
