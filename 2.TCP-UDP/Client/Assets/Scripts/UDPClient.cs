using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    private Socket socket;
    IPEndPoint endPoint;
    EndPoint Remote;

    Thread sendThread;

    int port = 7777; //0 means take the first free port you get

    // Start is called before the first frame update
    void Start()
    {        
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        //Remote = (EndPoint)endPoint;

        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        //socket.Bind(ipep);

        sendThread = new Thread(new ThreadStart(SendPing));
        sendThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SendPing(){
        try
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Ping");
            int bytesSent = socket.SendTo(bytes, bytes.Length, SocketFlags.None, endPoint);
            
            Debug.Log(bytesSent.ToString() + " : of " + bytes.Length + " bytes sent");
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error. Couldn't send message: " + exception.ToString());
            Close();
        }
        Debug.Log("Ended sending message");
    }

    void ReceivePong()
    {
        byte[] buffer = new byte[256];
        var revc = socket.ReceiveFrom(buffer, ref Remote);
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
