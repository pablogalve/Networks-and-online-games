using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command 
{
    public string name;
    public string description;


    public virtual void Execute(TCPClient client,  Message originalMessage)
    {
        Debug.Log("Override execute function");
    }
}


public class ChangeName : Command
{
    public ChangeName()
    {
        name = "changeName";
        description = "change user's name";
    }

    public override void Execute(TCPClient client, Message originalMessage)
    {
        int index = originalMessage._message.IndexOf(" ");
        string content = originalMessage._message.Substring(index + 1, originalMessage._message.Length - index -1);

        if(originalMessage._returnCode == 200)
        {
            client.username = content;
            client.id = originalMessage._userId;
        }

        client.logControl.LogText("Server", "Username set to: " + content);
        Debug.Log("Username id is: " + client.id);
    }
}