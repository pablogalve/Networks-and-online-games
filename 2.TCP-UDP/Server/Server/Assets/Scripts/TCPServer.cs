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

    private bool startNewThread = false;
    private Thread receiveThread;


    // Start is called before the first frame update
    void Start()
    {
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        senderRemote = (EndPoint)endPoint;

        socket.Bind(endPoint);

        receiveThread = new Thread(new ThreadStart(Receive));
        receiveThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (startNewThread)
        {
            startNewThread = false;
            receiveThread = new Thread(new ThreadStart(Receive));
            receiveThread.Start();
        }
    }

    void Receive()
    {
        try
        {
            //Listen for a single client
            Debug.Log("Listening for clients");
            socket.Listen(1);

            client = socket.Accept();
            Debug.Log("Client accepted");

            //Debug.Log("Trying to receive a message: ");
            byte[] msg = new byte[256];

            int recv = client.Receive(msg);
            string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);
            Debug.Log(decodedMessage);

            Thread.Sleep(500);

            //Send();

            startNewThread = true;
            //Close();
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Exception caught: " + exception.ToString());
            Close();
        }
    }

    void Send()
    {
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Pong");

        int bytesSent = socket.SendTo(bytes, bytes.Length, SocketFlags.None, senderRemote);
        //Debug.Log(bytesSent.ToString() + " : of " + bytes.Length + " bytes sent");
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
        receiveThread.Abort();
    }
}
