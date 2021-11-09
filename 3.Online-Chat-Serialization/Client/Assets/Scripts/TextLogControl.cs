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

    private void Start()
    {
        textItems = new List<GameObject>();
    }

    public void LogText(string username, string newTextString)
    {
        //Debug.Log(newTextString);

        Color color = Color.green;

        //If we have too many messages, delete the first one received
        if(textItems.Count > maxTextItems)
        {
            GameObject tempItem = textItems[0];
            textItems.RemoveAt(0);
            Destroy(tempItem.gameObject);
        }
        
        //Create a new text to add to the chat
        GameObject newText = Instantiate(textTemplate);
        newText.SetActive(true);

        TextLogItem textLogItem = newText.GetComponent<TextLogItem>();

        textLogItem.SetText(username, newTextString, color);
        newText.transform.SetParent(textTemplate.transform.parent, false);

        textItems.Add(newText.gameObject);
    }
}
