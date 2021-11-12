using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command 
{
    public string name;
    public string description;


    public virtual void Execute(TCPServer server, User originUser, Message originalMessage)
    {
        Debug.Log("Override execute function");
    }
}

public class HelpCommand : Command
{
    public HelpCommand()
    {
        name = "help";
        description = "list all commands";
    }

    public override void Execute(TCPServer server, User originUser, Message originalMessage)
    {
        string concatenatedCommands = "";

        foreach(KeyValuePair<string, Command> command in server.commands)
        {
            concatenatedCommands += "/" + command.Key;
            concatenatedCommands += ": " + command.Value.description + "\n";
        }

        originalMessage.SerializeJson(-1, "Server", System.DateTime.Now, concatenatedCommands);
        originalMessage._returnCode = 200;
        originalMessage._type = MessageType.MESSAGE;

        server.Send(originUser, originalMessage);
    }
}

public class ChangeName : Command
{
    public ChangeName()
    {
        name = "changeName";
        description = "change user's name";
    }

    public override void Execute(TCPServer server, User originUser, Message originalMessage)
    {
        bool usernameFound = false;

        int index = originalMessage._message.IndexOf(" ");
        string content = originalMessage._message.Substring(index, originalMessage._message.Length - index);

        //Debug.Log("Username: " + username);

        //OK
        originalMessage._userId = originUser.id;
        originalMessage._type = MessageType.COMMAND;
        originalMessage._returnCode = 200;
        string username = content;

        //Iterate all users to check if it is available
        for (int i = 0; i < server.users.Count; ++i)
        {
            if (server.users[i].username == username)
            {
                Debug.Log("Username already taken");

                //Bad request, username already taken
                originalMessage._returnCode = 400;
                originalMessage._message = "Username not available";
                originalMessage._type = MessageType.COMMAND;
                usernameFound = true;
                break;
            }
        }

        if (!usernameFound)
        {
            originUser.username = username;
            Debug.Log("Username: " + username + " accepted");
        }

        originalMessage.Serialize();
        server.Send(originUser, originalMessage);
    }
}