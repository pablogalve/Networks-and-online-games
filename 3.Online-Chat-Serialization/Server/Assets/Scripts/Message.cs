using System;
using System.IO;
using UnityEngine;

public enum MessageType
{
    COMMAND,
    MESSAGE
}

[Serializable]
public struct JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt)
    {
        //Debug.Log("Converted to time");
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        //Debug.Log("Converted to JDT");
        JsonDateTime jdt = new JsonDateTime();
        jdt.value = dt.ToFileTimeUtc();
        return jdt;
    }
}

public class Message
{
    public int _userId;
    public string _username;
    public JsonDateTime _timestamp;
    public string _message;
    public MessageType _type;
    public int _returnCode = 200;
    public Color _userColor;

    public byte[] Serialize()
    {
        string json = JsonUtility.ToJson(this);
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);
        return stream.ToArray();
    }

    public byte[] SerializeJson(int id, string username, DateTime timestamp, string message, Color userColor)
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

        return Serialize();
    }

    public static Message DeserializeJson(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        Message message = new Message();
        message = JsonUtility.FromJson<Message>(reader.ReadString());
        Debug.Log("deserialized message: " + message._message);

        return message;
    }
}