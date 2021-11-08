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
    //private Socket socket;
    //private IPEndPoint endPoint;
   // private Dictionary<int, User> users;

    private readonly int port = 7777;

    private Thread listenThread;

    private int maximumSockets = 1;

    Socket[] listenSockets;
    Socket[] acceptSockets;

    ArrayList listenList;
    ArrayList acceptList;

    // Start is called before the first frame update
    void Start()
    {
        listenSockets = new Socket[maximumSockets];
        listenList = new ArrayList();
        for (int i = 0; i < listenSockets.Length; ++i)
        {
            listenList.Add(listenSockets[i]);
        }

        acceptSockets = new Socket[maximumSockets];
        acceptList = new ArrayList();
        for (int i = 0; i < acceptSockets.Length; ++i)
        {
            acceptList.Add(acceptSockets[i]);
        }

        /*
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endPoint);
        users = new Dictionary<int, User>();
        */

        listenThread = new Thread(new ThreadStart(ListenForUsers));
        listenThread.Start();
    }

    // Update is called once per frame
    void Update()
    {

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

        Thread.Sleep(3000);

        Socket.Select(listenList, null, null, 1000);

        for (int i = 0; i < listenList.Count; i++)
        {
            acceptList[i] = ((Socket)listenList[i]).Accept();
            Debug.Log("Accepted");
        }

        Debug.Log("Listen List: " + listenList.Count.ToString());
        Debug.Log("Accept List: " + acceptList.Count.ToString());


        /*
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
        */
    }

    /*
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
    */

    void Close(Socket socket)
    {
        if(socket != null)
        {
            socket.Close();
            Debug.Log("Socket closed");
        }
    }
    
    private void OnDestroy()
    {
        //Close server
        for(int i = 0; i < listenSockets.Length; ++i)
        {
            Close(listenSockets[i]);
        }

        for (int i = 0; i < acceptSockets.Length; ++i)
        {
            Close(acceptSockets[i]);
        }

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
