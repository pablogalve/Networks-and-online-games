using System;
using System.IO;
using UnityEngine;

public class Message
{
    string jsonMsg;
    private MemoryStream stream;

    public int _id;
    public DateTime _timestamp;
    public string _message;

    public void SerializeJson(int id, DateTime timestamp, string message)
    {
        _id = id;
        _timestamp = timestamp;
        _message = message;

        jsonMsg = JsonUtility.ToJson(this);
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(jsonMsg);
    }

    public static Message DeserializeJson(string json)
    {        
        //MemoryStream stream = new MemoryStream();
        /*BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);*/
        //BinaryReader reader = new BinaryReader(stream);
        //stream.Seek(0, SeekOrigin.Begin);

        //string jsonMsg = reader.ReadString();
        Message message = new Message();
        message = JsonUtility.FromJson<Message>(json);
        Debug.Log("deserialize message: " + message._message);
        return message;
    }

    public string json
    {
        get { return jsonMsg; }
        set { jsonMsg = value; }
    }
}