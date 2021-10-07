using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private Socket socket;
    IPEndPoint sender;
    EndPoint senderRemote;

    readonly int port = 7777;

    Thread receiveThread;

    bool startNewThread = false;

    // Start is called before the first frame update
    void Start()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        //sender = new IPEndPoint(IPAddress.Any, 0);
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
            //Debug.Log("Trying to receive a message: ");
            byte[] msg = new byte[256];

            //Debug.Log(senderRemote.ToString());

            var recv = socket.ReceiveFrom(msg, ref senderRemote);

            string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);

            Debug.Log(decodedMessage);

            Thread.Sleep(500);

            Send();

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
