using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct TextLog
{
    public TextLog(string myUsername, string myMessage, int myUserId, Color myColor)
    {
        username = myUsername;
        message = myMessage;
        userId = myUserId;
        color = myColor;
    }

    public string username;
    public string message;
    public int userId;
    public Color color;
}


public class TextLogControl : MonoBehaviour
{
    [SerializeField]
    private GameObject textTemplate;

    private List<TextLogItem> textItems;
    private List<TextLog> textLogs;

    public int maxTextItems = 200;

    private void Start()
    {
        textItems = new List<TextLogItem>();
        
        if(textLogs == null)
        {
            textLogs = new List<TextLog>();
        }
    }

    private void Update()
    {
        while (textLogs.Count > 0)
        {
            //If we have too many messages, delete the first one received
            if (textItems != null && textItems.Count > maxTextItems)
            {
                TextLogItem tempItem = textItems[0];
                textItems.RemoveAt(0);
                Destroy(tempItem.gameObject);
            }

            //Create a new text to add to the chat
            GameObject newText = Instantiate(textTemplate);
            newText.SetActive(true);

            //Set all its parameters
            TextLogItem textLogItem = newText.GetComponent<TextLogItem>();
            textLogItem.SetText(textLogs[0].username, textLogs[0].message, textLogs[0].color, textLogs[0].userId);
            newText.transform.SetParent(textTemplate.transform.parent, false);

            //Add it to the items create and remove it from the list to be created
            textItems.Add(textLogItem);
            textLogs.RemoveAt(0);
        }
    }

    public void LogText(string username, string message, int id, Color color)
    {
        if (textLogs == null)
        {
            textLogs = new List<TextLog>();
        }

        //Debug.Log(newTextString);
        textLogs.Add(new TextLog(username, message, id, color));
    }

    public void ChangeUsername(int id, string newUsername)
    {
        for (int i = 0; i < textLogs.Count; ++i)
        {
            if(textItems[i].userId == id)
            {
                textItems[i].ChangeUsername(newUsername);
            }
        }
    }
}
