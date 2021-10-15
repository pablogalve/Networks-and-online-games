using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    private Socket socket;
    IPEndPoint endPoint;
    EndPoint Remote;

    Thread sendThread;

    int port = 7777; //0 means take the first free port you get

    bool startNewThread = false;
    int pongsReceived = 0;

    Animator animator;

    public string messageToSend = "Ping";
    public int millisecondsBetweenMessages = 500;

    bool bow = false;

    public Dialog dialog;

    public bool versionA = true;

    void Start()
    {
        animator = GetComponent<Animator>();

        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        Remote = (EndPoint)endPoint;

        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Remote: " + endPoint.Address.ToString());

        socket.Connect(Remote);

        sendThread = new Thread(new ThreadStart(SendPing));
        sendThread.Start();
    }

    void Update()
    {
        if (startNewThread)
        {
            startNewThread = false;
            sendThread = new Thread(new ThreadStart(SendPing));
            sendThread.Start();
        }

        if(bow)
        {
            bow = false;
            animator.SetTrigger("Bow");
            if (dialog != null)
            {
                dialog.SetMessage(messageToSend);
            }
        }
    }

    void SendPing()
    {
        try
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(messageToSend);
            int bytesCount = socket.Send(msg, msg.Length, SocketFlags.None);
            bow = true;

            ReceivePong();

            //Set version A to true to only exchange ping pong for 5 times and set it to false to exchange until the end of the program
            if(pongsReceived < 5 || versionA == false)
            {
                startNewThread = true;
            }
            else if (versionA)
            {
                Debug.Log("Enough ping pong for today, disconnecting");
                Close();
            }
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error. Couldn't send message: " + exception.ToString());
            Close();
        }
    }

    void ReceivePong()
    {
        byte[] msg = new byte[256];
        var recv = socket.Receive(msg);
        string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);

        Debug.Log(decodedMessage);

        pongsReceived++;

        Thread.Sleep(millisecondsBetweenMessages);
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
        sendThread.Abort();
    }
}
