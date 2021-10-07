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
        //Debug.Log("AAAAAAAAAAA");
    }

    void Receive()
    {
        try
        {
            Debug.Log("Trying to receive a message: ");
            byte[] msg = new byte[256];
            
            Debug.Log(senderRemote.ToString());

            var recv = socket.ReceiveFrom(msg, ref senderRemote);

            string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);

            Debug.Log("Message " + decodedMessage + " received with " + recv.ToString() + ": bytes received");
           
            Close();
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Exception caught: " + exception.ToString());
            Close();
        }
        Debug.Log("Stopped waiting to receive");
    }

    void SendPong()
    {
        socket.SendTo(System.Text.Encoding.UTF8.GetBytes("Pong"), SocketFlags.None, senderRemote);
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
