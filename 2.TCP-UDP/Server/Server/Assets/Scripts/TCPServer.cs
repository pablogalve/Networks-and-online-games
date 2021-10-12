﻿using System.Collections;
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

    //Versions
    bool versionA = false;
    bool versionB = true;

    // Start is called before the first frame update
    void Start()
    {
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        senderRemote = (EndPoint)endPoint;

        socket.Bind(endPoint);

        listenThread = new Thread(new ThreadStart(Listen));
        listenThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (startNewListenThread)
        {
            startNewListenThread = false;
            listenThread = new Thread(new ThreadStart(Listen));
            listenThread.Start();
        }

        if (startNewReceiveThread)
        {
            startNewReceiveThread = false;
            receiveThread = new Thread(new ThreadStart(Receive));
            receiveThread.Start();
        }
    }

    void Listen()
    {
        //Listen for a single client
        Debug.Log("Listening for clients");
        socket.Listen(1);

        client = socket.Accept();
        Debug.Log("Client accepted");

        receiveThread = new Thread(new ThreadStart(Receive));
        receiveThread.Start();
    }

    void Receive()
    {
        try
        {
            //Debug.Log("Trying to receive a message: ");
            byte[] msg = new byte[256];

            int recv = client.Receive(msg);
            string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);
            Debug.Log("Message: " + decodedMessage);

            Thread.Sleep(500);

            Send();

            startNewReceiveThread = true;
            //Close();
        }
        catch (System.Exception exception)
        {
            Debug.Log("Exception caught: " + exception.ToString());
            if (versionB)
            {
                startNewListenThread = true;
            }
            else
            {
                Close();
            }
        }
    }

    void Send()
    {
        try
        {
            //Debug.Log("Sending Pong");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes("Pong");
            int bytesSent = client.Send(msg, msg.Length, SocketFlags.None);
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
        listenThread.Abort();
        receiveThread.Abort();
    }
}
