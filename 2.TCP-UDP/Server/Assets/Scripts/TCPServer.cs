using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    private Socket socket;
    private Socket client;

    private IPEndPoint sender;
    private IPEndPoint endPoint;
    private EndPoint senderRemote;

    readonly int port = 7777;

    private bool startNewListenThread = false;
    private bool startNewReceiveThread = false;
    private Thread listenThread;
    private Thread receiveThread;

    int maxListeningClients = 5;

    public string messageToSend = "Pong";
    public int millisecondsBetweenMessages = 500;

    //Versions
    public bool versionA = false;

    Animator animator;
    bool wantsToShout = false;

    public Dialog dialog;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        senderRemote = (EndPoint)endPoint;

        socket.Bind(endPoint);

        ThreadPool.QueueUserWorkItem(Listen);
    }

    // Update is called once per frame
    void Update()
    {
        if (startNewListenThread)
        {
            startNewListenThread = false;
            //WaitToAccept();

            ThreadPool.QueueUserWorkItem(Listen);
        }

        if (startNewReceiveThread)
        {
            startNewReceiveThread = false;
            ThreadPool.QueueUserWorkItem(Receive);
        }

        if (wantsToShout)
        {
            wantsToShout = false;
            animator.SetTrigger("Shout");
            if(dialog != null)
            {
                dialog.SetMessage(messageToSend);
            }
        }
    }

    private void Listen(object state)
    {
        //Listen for a single client
        try
        {
            Debug.Log("Listening for clients");
            socket.Listen(maxListeningClients);

            //WaitToAccept();

            client = socket.Accept();
            Debug.Log("Client accepted");

            ThreadPool.QueueUserWorkItem(Receive);
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning(exception.ToString());
            Close();
        }
    }    

    void WaitToAccept()
    {
        client = socket.Accept();
        Debug.Log("Client accepted");
        
        ThreadPool.QueueUserWorkItem(Receive);
    }

    private void Receive(object state)
    {
        try
        {
            //Debug.Log("Trying to receive a message: ");
            byte[] msg = new byte[256];

            int recv = client.Receive(msg);
            string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);
            Debug.Log("Message: " + decodedMessage);

            Thread.Sleep(millisecondsBetweenMessages);

            Send();

            startNewReceiveThread = true;
            //Close();
        }
        catch (System.Exception exception)
        {
            Debug.Log("Exception caught: " + exception.ToString());
            if (versionA)
            {
                Close();
            }
            else
            {
                startNewListenThread = true;
            }
        }
    }

    void Send()
    {
        try
        {
            //Debug.Log("Sending Pong");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(messageToSend);
            int bytesSent = client.Send(msg, msg.Length, SocketFlags.None);
            wantsToShout = true;
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error. Couldn't send message: " + exception.ToString());
            Close();
        }
    }

    void Shutdown()
    {
        socket.Shutdown(SocketShutdown.Both);
        Debug.Log("Socket shut down");
    }

    void Close()
    {
        socket.Close();
        Debug.Log("Socket closed");
    }

    private void OnDestroy()
    {
        socket.Close();
        if (listenThread != null)
        {
            listenThread.Abort();
        }
        if (receiveThread != null)
        {
            receiveThread.Abort();
        }
    }
}
