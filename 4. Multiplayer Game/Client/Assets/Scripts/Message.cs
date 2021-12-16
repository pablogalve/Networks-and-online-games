using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum MessageType
{
    INSTANCE,
    DESTROY
}

public class Message
{
    public string objectId;
    public MessageType type;

    public Guid guid
    {
        get { return new Guid(objectId); }
        set { objectId = value.ToString(); }
    }

    public virtual byte[] Serialize()
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

        string json = reader.ReadString();
        MessageType type = JsonUtility.FromJson<Message>(json).type;

        Debug.Log(json);

        switch (type)
        {
            case MessageType.INSTANCE:
                VectorMessage vectorMessage = JsonUtility.FromJson<VectorMessage>(json);
                return vectorMessage;

            default:
                return new Message();
        }

    }
}

public class VectorMessage : Message
{
    public VectorMessage(MessageType type, NetworkedObject networkedObject)
    {
        this.objectId = networkedObject.id.ToString();
        this.type = type;
        floatVector = new float[3] { networkedObject.transform.position.x, networkedObject.transform.position.y, networkedObject.transform.position.z };
    }

    public float[] floatVector;

    public Vector3 vector
    {
        get
        {
            return new Vector3(floatVector[0], floatVector[1], floatVector[2]);
        }
    }
}

