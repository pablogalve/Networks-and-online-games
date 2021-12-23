using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum MessageType
{
    INSTATIATION,
    DESTROY,
    PLAYER_POSITION,
    PING_PONG,
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

        //Debug.Log(json);

        switch (type)
        {
            case MessageType.INSTATIATION:
                InstanceMessage instantiationMessage = JsonUtility.FromJson<InstanceMessage>(json);
                return instantiationMessage;

            case MessageType.PLAYER_POSITION:
                VectorMessage playerPositionMessage = JsonUtility.FromJson<VectorMessage>(json);
                return playerPositionMessage;

            case MessageType.PING_PONG:
                VectorMessage ping_pong = JsonUtility.FromJson<VectorMessage>(json);
                return ping_pong;

            default:
                return new Message();
        }

    }
}

public class InstanceMessage : Message
{
    public enum InstanceType
    {
        PLAYER_BULLET,
        ENEMY_BULLET,
        EXPLOSION_PARTICLES
    }

    public InstanceMessage (MessageType messageType, string id, InstanceType instanceType, Vector3 position, float speed)
    {
        type = messageType;
        objectId = id;
        _instanceType = instanceType;
        _position = fromVector(position);
        _speed = speed;

    }

    public InstanceType _instanceType;
    public float[] _position;
    public float _speed;

    public Vector3 toVector3(float[] floatVector)
    {
        return new Vector3(floatVector[0], floatVector[1], floatVector[2]); 
    }

    public float[] fromVector(Vector3 vector)
    {
        return new float[] { vector.x, vector.y, vector.z };
    }
}

public class VectorMessage : Message
{
    public VectorMessage(MessageType type, string id, Vector3 vector)
    {
        this.objectId = id;
        this.type = type;
        floatVector = new float[3] { vector.x, vector.y, vector.z };
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

