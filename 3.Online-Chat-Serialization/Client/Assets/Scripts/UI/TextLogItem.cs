using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLogItem : MonoBehaviour
{
    RectTransform _rectTransform;
    public GameObject _usernameObject;
    private Text _username;

    public GameObject _messageObject;
    private Text _message;

    private void Start()
    {
        if(_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        if (_username == null)
        {
            _username = _usernameObject.GetComponent<Text>();
        }

        if (_message == null)
        {
            _message = _messageObject.GetComponent<Text>();
        }
    }


    public void SetText(string username, string text, Color color)
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        if (_username == null)
        {
            _username = _usernameObject.GetComponent<Text>();
        }

        if(_message == null)
        {
            _message = _messageObject.GetComponent<Text>();
        }


        if (_username != null)
        {
            _username.text = username;
            _username.color = color;
        }

        if (_message != null)
        {
            _message.text = text;
            _message.color = Color.white;
        }

        int entersAmount = text.Split('\n').Length - 1;
        _rectTransform.sizeDelta = (_rectTransform.rect.size) + new Vector2(0.0f, entersAmount * 10.0f);
    }
}
