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

    bool startNewThread = false;

    Animator animator;

    bool bow = false;

    public string messageToSend = "Ping";
    public int millisecondsBetweenMessages = 500;

    public Dialog dialog;

    void Start()
    {
        animator = GetComponent<Animator>();

        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        Remote = (EndPoint)endPoint;

        socket = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        //socket.Bind(ipep);

        sendThread = new Thread(new ThreadStart(SendPing));
        sendThread.Start();
    }

    void Update()
    {
        if(startNewThread)
        {
            startNewThread = false;
            sendThread = new Thread(new ThreadStart(SendPing));
            sendThread.Start();
        }

        if (bow)
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
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(messageToSend);
            int bytesSent = socket.SendTo(bytes, bytes.Length, SocketFlags.None, endPoint);
            bow = true;

            //Debug.Log(bytesSent.ToString() + " : of " + bytes.Length + " bytes sent");

            ReceivePong();
            
            startNewThread = true;
        }
        catch (System.Exception exception)
        {
            Debug.Log("Error. Couldn't send message: " + exception.ToString());
            Close();
        }

    }

    void ReceivePong()
    {
        //Debug.Log("Trying to receive a message: ");
        byte[] msg = new byte[256];

        //Debug.Log(senderRemote.ToString());

        var recv = socket.ReceiveFrom(msg, ref Remote);

        string decodedMessage = System.Text.Encoding.ASCII.GetString(msg);

        Debug.Log(decodedMessage);

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
