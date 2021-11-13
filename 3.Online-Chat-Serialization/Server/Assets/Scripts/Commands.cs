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

    public string GetContent(Message message)
    {
        int index = message._message.IndexOf(" ");
        string content = message._message.Substring(index + 1, message._message.Length - index - 1);
        return content;
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

        foreach (KeyValuePair<string, Command> command in server.commands)
        {
            concatenatedCommands += "/" + command.Key;
            concatenatedCommands += ": " + command.Value.description + "\n";
        }

        originalMessage.SerializeJson(-1, "Server", System.DateTime.Now, concatenatedCommands, server.color);
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

        string content = GetContent(originalMessage);

        //Debug.Log("Username: " + username);

        bool userHadUsernameSet = originalMessage._userId != 0;

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
                originalMessage._type = MessageType.MESSAGE;
                usernameFound = true;
                break;
            }
        }

        if (!usernameFound)
        {
            originUser.username = username;
            originUser.color = server.availableColors[0];
            originalMessage._userColor = originUser.color;
            server.availableColors.RemoveAt(0);

            Debug.Log("Username: " + username + " accepted");
        }

        //Send the user the command to change his name
        originalMessage.Serialize();
        server.Send(originUser, originalMessage);

        //Tell everyone about the change
        if (!userHadUsernameSet) 
        {
            originalMessage.SerializeJson(-1, "Server", System.DateTime.Now, "User " + username + " has joined the chat", server.color);
            server.SendToEveryone(originalMessage, originUser);
        }
        else
        {
            originalMessage.SerializeJson(originUser.id, "Server", System.DateTime.Now, "/changeOtherUsername " + username, originUser.color);
            server.SendToEveryone(originalMessage, originUser);
        }

    }
}

public class ListUsers : Command
{
    public ListUsers()
    {
        name = "list";
        description = "list all connected users";
    }

    public override void Execute(TCPServer server, User originUser, Message originalMessage)
    {
        originalMessage._message = "List of users: ";
        for (int i = 0; i < server.users.Count; ++i)
        {
            originalMessage._message += server.users[i].username + " ";
        }
        originalMessage.SerializeJson(-1, "Server", System.DateTime.Now, originalMessage._message, server.color);
        server.Send(originUser, originalMessage);
    }
}

public class KickUser : Command
{
    public KickUser()
    {
        name = "kick";
        description = "kicks a user out of the group chat";
    }

    public override void Execute(TCPServer server, User originUser, Message originalMessage)
    {
        string usernameToRemove = GetContent(originalMessage);
        User userToRemove = server.GetUserByName(usernameToRemove);

        if (server.RemoveUser(userToRemove))
        {
            originalMessage.SerializeJson(-1, "Server", System.DateTime.Now, "User " + usernameToRemove + " kicked", server.color);
            server.SendToEveryone(originalMessage, null);
        }
        else
        {
            originalMessage.SerializeJson(-1, "Server", System.DateTime.Now, "Error. User" + usernameToRemove + " wasn't kicked", server.color);
            server.SendToEveryone(originalMessage, null);
        }        
    }
}

public class Leave : Command
{
    public Leave()
    {
        name = "leave";
        description = "leave the server";
    }

    public override void Execute(TCPServer server, User originUser, Message originalMessage)
    {
        server.RemoveUser(originUser);
        
        originalMessage.SerializeJson(-1, "Server", System.DateTime.Now, originUser.username + " left the chat", server.color);
        server.SendToEveryone(originalMessage, null);        
    }
}

public class Whisper : Command
{
    public Whisper()
    {
        name = "whisper";
        description = "send a message only to a desired user";
    }

    public override void Execute(TCPServer server, User originUser, Message originalMessage)
    {
        string text = GetContent(originalMessage);
        int index = text.IndexOf(' ', 0);
        string usernameToWhisper = text.Substring(0, index);
        User userToWhisper = server.GetUserByName(usernameToWhisper);
        if (userToWhisper != null)
        {
            originalMessage._message = text.Substring(index + 1);

            originalMessage.SerializeJson(originalMessage._userId, originUser.username, System.DateTime.Now, originalMessage._message, originUser.color);
            server.Send(userToWhisper, originalMessage);
        }
        else
        {
            originalMessage.SerializeJson(-1, "Server", System.DateTime.Now, "User doesn't exist", server.color);
            server.Send(userToWhisper, originalMessage);
        }  
    }
}