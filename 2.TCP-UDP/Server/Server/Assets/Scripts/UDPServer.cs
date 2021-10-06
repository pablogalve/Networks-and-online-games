using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private Socket socket;
    private Socket client;
    IPEndPoint ipep;
    EndPoint remote;

    Thread receiveThread;

    // Start is called before the first frame update
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        int port = 7777;

        ipep = new IPEndPoint(IPAddress.Any, port);
        remote = (EndPoint)ipep;

        socket.Bind(ipep);

        receiveThread = new Thread(new ThreadStart(ReceivePing));
        receiveThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("AAAAAAAAAAA");
    }

    void ReceivePing()
    {
        Debug.Log("Receive 1");
        try
        {
            Debug.Log("Receive 1.5");
            byte[] msg = new byte[256];
            var recv = socket.ReceiveFrom(msg, ref remote);

            Debug.Log("Recv: " + recv.ToString());

            Debug.Log("Message received: " + msg.ToString());
        }
        catch (System.Exception exception)
        {
            Debug.Log("Not receiving ping: " + exception.ToString());
        }
        Debug.Log("Receive 2");
    }

    void SendPong()
    {
        socket.SendTo(System.Text.Encoding.UTF8.GetBytes("Pong"), SocketFlags.None, remote);
    }

    void StopDataTransfer()
    {
        socket.Shutdown(SocketShutdown.Both);
    }

    private void OnDestroy()
    {
        socket.Close();
    }
}
