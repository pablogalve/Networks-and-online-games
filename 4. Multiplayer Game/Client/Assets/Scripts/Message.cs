using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum MessageType
{
    CONNECTION,
    INSTANTIATE,
    DESTROY,
    PLAYER_POSITION,
    OBJECT_POSITION,
    COLLISION,
    PING_PONG,
    DISONNECT_PLAYER,
    PLAYER_DEATH,
    START_GAME,
    GAME_FINISHED,
    EMPTY,
}

public class Message
{
    public int senderId = 0; 
    //public string objectId = "-1";
    public MessageType type;

    //public Guid guid
    //{
    //    get { return new Guid(objectId); }
    //    set { objectId = value.ToString(); }
    //}

    public Message(MessageType _type)
    {
        type = _type;
    }

    public static int GenerateNewGuid()
    {
        return UnityEngine.Random.Range(2, int.MaxValue);
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

        if(json.Length == 0)
        {
            return new Message(MessageType.EMPTY);
        }

        Debug.Log(json);
        MessageType type = JsonUtility.FromJson<Message>(json).type;

        //Debug.Log(json);

        switch (type)
        {
            case MessageType.CONNECTION:
                return JsonUtility.FromJson<IdMessage>(json);

            case MessageType.INSTANTIATE:
                InstanceMessage instantiationMessage = JsonUtility.FromJson<InstanceMessage>(json);
                return instantiationMessage;

            case MessageType.DESTROY:
                IdMessage destroyMessage = JsonUtility.FromJson<IdMessage>(json);
                return destroyMessage;

            case MessageType.PLAYER_POSITION:
                VectorMessage playerPositionMessage = JsonUtility.FromJson<VectorMessage>(json);
                return playerPositionMessage;

            case MessageType.OBJECT_POSITION:
                VectorMessage objectPositionMessage = JsonUtility.FromJson<VectorMessage>(json);
                return objectPositionMessage;

            case MessageType.COLLISION:
                CollisionMessage collisionMessage = JsonUtility.FromJson<CollisionMessage>(json);
                return collisionMessage;

            case MessageType.PING_PONG:
                PingPongMessage ping_pong = JsonUtility.FromJson<PingPongMessage>(json);
                return ping_pong;

            case MessageType.DISONNECT_PLAYER:
                DisconnectPlayerMessage diconnectMessage = JsonUtility.FromJson<DisconnectPlayerMessage>(json);
                return diconnectMessage;

            default:
                Debug.LogWarning("Needs to add new message type");
                return JsonUtility.FromJson<Message>(json);
        }

    }
}

public class InstanceMessage : Message
{
    public enum InstanceType
    {
        PLAYER_BULLET,
        ENEMY_BULLET,
        ENEMY,
        EXPLOSION_PARTICLES
    }

    public InstanceMessage(MessageType messageType, string id, InstanceType instanceType, Vector3 position, float speed) : base(messageType)
    {
        type = MessageType.INSTANTIATE;
        objectId = id;
        _instanceType = instanceType;
        _position = fromVector(position);
        _speed = speed;
    }

    public InstanceType _instanceType;
    public float[] _position;
    public float _speed;
    public string objectId;

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
    public VectorMessage(MessageType type, string id, Vector3 vector) : base(type)
    {
        this.objectId = id;
        this.type = type;
        floatVector = new float[3] { vector.x, vector.y, vector.z };
    }

    public string objectId;
    public float[] floatVector;

    public Vector3 vector
    {
        get
        {
            return new Vector3(floatVector[0], floatVector[1], floatVector[2]);
        }
    }
}

public class CollisionMessage : Message
{
    public string colliderObjectId;
    public string collidedObjectId;

    public CollisionMessage(string colliderObjectId, string collidedObjectId) : base(MessageType.COLLISION)
    {
        type = MessageType.COLLISION;
        this.colliderObjectId = colliderObjectId;
        this.collidedObjectId = collidedObjectId;
    }
}

public class IdMessage : Message
{
    public string objectId;

    public IdMessage(MessageType messageType, string objectToDestroyId) : base(messageType)
    {
        type = messageType;
        this.objectId = objectToDestroyId; 
    }
}

public class PingPongMessage : Message
{
    public PingPongMessage() : base(MessageType.PING_PONG)
    {
    }
}

public class DisconnectPlayerMessage : Message
{
    public DisconnectPlayerMessage(int id) : base(MessageType.DISONNECT_PLAYER)
    {
        senderId = id;
    }
}