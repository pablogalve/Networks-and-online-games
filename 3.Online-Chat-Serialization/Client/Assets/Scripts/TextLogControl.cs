using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLogControl : MonoBehaviour
{
    [SerializeField]
    private GameObject textTemplate;

    private List<GameObject> textItems;

    public int maxTextItems = 200;

    string _username;
    string _newText;

    bool createNewLog = false;

    private void Start()
    {
        textItems = new List<GameObject>();
    }

    private void Update()
    {
        if (createNewLog)
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

            TextLogItem textLogItem = newText.GetComponent<TextLogItem>();

            textLogItem.SetText(_username, _newText, color);
            newText.transform.SetParent(textTemplate.transform.parent, false);

            textItems.Add(newText);

            createNewLog = false;
        }
    }

    public void LogText(string username, string newTextString)
    {
        //Debug.Log(newTextString);
        _username = username;
        _newText = newTextString;

        createNewLog = true;
    }
}
