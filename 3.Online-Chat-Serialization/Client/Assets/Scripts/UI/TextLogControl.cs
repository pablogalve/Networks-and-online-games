using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct TextLog
{
    public TextLog(string myUsername, string myMessage)
    {
        username = myUsername;
        message = myMessage;
    }

    public string username;
    public string message;
}


public class TextLogControl : MonoBehaviour
{
    [SerializeField]
    private GameObject textTemplate;

    private List<GameObject> textItems;
    private List<TextLog> textLogs;

    public int maxTextItems = 200;

    private void Start()
    {
        textItems = new List<GameObject>();
        
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
                GameObject tempItem = textItems[0];
                textItems.RemoveAt(0);
                Destroy(tempItem.gameObject);
            }

            Color color = Color.green;

            //Create a new text to add to the chat
            GameObject newText = Instantiate(textTemplate);
            newText.SetActive(true);

            //Set all its parameters
            TextLogItem textLogItem = newText.GetComponent<TextLogItem>();
            textLogItem.SetText(textLogs[0].username, textLogs[0].message, color);
            newText.transform.SetParent(textTemplate.transform.parent, false);

            //Add it to the items create and remove it from the list to be created
            textItems.Add(newText);
            textLogs.RemoveAt(0);
        }
    }

    public void LogText(string username, string message)
    {
        if (textLogs == null)
        {
            textLogs = new List<TextLog>();
        }

        //Debug.Log(newTextString);
        textLogs.Add(new TextLog(username, message));
    }
}
