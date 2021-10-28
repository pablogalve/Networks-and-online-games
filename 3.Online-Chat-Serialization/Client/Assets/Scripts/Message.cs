using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MessageType
{
    COMMAND,
    USER_MESSAGE
}

public class Message : MonoBehaviour
{
    MessageType messageType;
}

public class Command : Message
{

}

public class UserMessages : Message
{

}