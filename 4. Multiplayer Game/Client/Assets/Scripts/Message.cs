using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

enum MessageType
{
    INSTANCE,
    DESTROY
}

public class Message
{
    string objectId;
    MessageType type;

    public Guid guid
    {
        get { return new Guid(objectId); }
        set { objectId = value.ToString(); }
    }

    public byte[] Serialize()
    {
        string json = JsonUtility.ToJson(this);
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);
        return stream.ToArray();
    }

    public static Message Deserialize(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        MessageType type = JsonUtility.FromJson<Message>(reader.ReadString()).type;

        switch (type)
        {
            case MessageType.INSTANCE:
                return JsonUtility.FromJson<VectorMessage>(reader.ReadString());

            default: return new Message();

        }

    }
}

public class VectorMessage : Message
{
    Vector3 vector;
}

