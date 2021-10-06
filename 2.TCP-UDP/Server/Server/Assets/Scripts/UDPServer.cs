using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private Socket socket;
    private Socket client;
    IPEndPoint ipep;

    // Start is called before the first frame update
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        int port = 7777;
        ipep = new IPEndPoint(IPAddress.Any, port);
        socket.Bind(ipep);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SendPong()
    {

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
