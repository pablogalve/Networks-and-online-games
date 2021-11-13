using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command 
{
    public string name;

    public virtual void Execute(TCPClient client,  Message originalMessage)
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

public class ChangeName : Command
{
    public ChangeName()
    {
        name = "changeName";
    }

    public override void Execute(TCPClient client, Message originalMessage)
    {
        string content = GetContent(originalMessage);

        if(originalMessage._returnCode == 200)
        {
            client.username = content;
            client.color = originalMessage._userColor;
            client.id = originalMessage._userId;
        }

        client.logControl.LogText("Server", "Username set to: " + content, -1, Color.magenta);
        Debug.Log("Username id is: " + client.id);
    }
}

public class ChangeOtherUsername : Command
{
    public ChangeOtherUsername()
    {
        name = "changeOtherUsername";
    }

    public override void Execute(TCPClient client, Message originalMessage)
    {
        string content = GetContent(originalMessage);

        client.logControl.ChangeUsername(originalMessage._userId, content);
    }
}