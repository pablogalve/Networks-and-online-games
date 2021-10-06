using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    private Socket socket;
    private Socket client;
    IPEndPoint ipep;
    EndPoint Remote;

    Thread sendThread;

    // Start is called before the first frame update
    void Start()
    {        
        int port = 7777;

        ipep = new IPEndPoint(IPAddress.Parse("95.17.95.173"), port);
        Remote = (EndPoint)ipep;

        socket = new Socket(ipep.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        //socket.Bind(ipep);

        sendThread = new Thread(new ThreadStart(SendPing));
        sendThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReceivePong(){
        byte[] buffer = new byte[256];
        var revc = socket.ReceiveFrom(buffer, ref Remote);
    }

    void SendPing(){
        Debug.Log("1");
        try
        {            
            socket.SendTo(System.Text.Encoding.UTF8.GetBytes("Ping"), SocketFlags.None, Remote);
            Debug.Log("2");
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error. Couldn't send ping: " + exception.ToString());
        }
        Debug.Log("3");
    }
}
