using System;
using System.IO;
using UnityEngine;

public enum MessageType
{
    COMMAND,
    MESSAGE
}

public class Message
{
    string jsonMsg;
    private MemoryStream stream;

    public int _userId;
    public string _username;
    public DateTime _timestamp;
    public string _message;
    public MessageType _type;
    public int _returnCode = 200;
    public Color _userColor;

    public void Serialize()
    {
        jsonMsg = JsonUtility.ToJson(this);
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(jsonMsg);
    }

    public void SerializeJson(int id, string username, DateTime timestamp, string message, Color userColor)
    {
        _userId = id;
        _username = username;
        _timestamp = timestamp;
        _message = message;

        _userColor = userColor;

        if (message[0] == '/')
        {
            _type = MessageType.COMMAND;
        }
        else
        {
            _type = MessageType.MESSAGE;
        }

        Serialize();
    }

    public static Message DeserializeJson(string json)
    {
        //MemoryStream stream = new MemoryStream();
        /*BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);*/
        //BinaryReader reader = new BinaryReader(stream);
        //stream.Seek(0, SeekOrigin.Begin);

        //string jsonMsg = reader.ReadString();

        if (json == null)
        {
            return null;
        }

        Message message = new Message();

        message = JsonUtility.FromJson<Message>(json);
        Debug.Log("deserialized message: " + message._message);
        return message;
    }

    public string json
    {
        get { return jsonMsg; }
        set { jsonMsg = value; }
    }
}