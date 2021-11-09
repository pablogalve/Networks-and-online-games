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
    // private Dictionary<int, User> users;
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
            acceptList[i] = ((Socket)listenList[i]).Accept();
            Debug.Log("Accepted");
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
                string receivedMessage = ReceiveMessage(socket);

                //Add difference between commands and messages
                Send(socket, receivedMessage);
            }

            Thread.Sleep(500);
        }
    }

    string ReceiveMessage(Socket socket)
    {
        try
        {
            //Debug.Log("Trying to receive a message: ");
            byte[] msg = new byte[256];
            int recv = socket.Receive(msg);
            string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);

            Debug.Log("Message: " + decodedMessage);

            return decodedMessage;
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Exception caught: " + exception.ToString());
            Close(socket);
            return "Error receiving message";
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
