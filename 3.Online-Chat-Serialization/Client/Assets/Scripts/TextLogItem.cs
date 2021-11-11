using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLogItem : MonoBehaviour
{
    public GameObject _usernameObject;
    private Text _username;

    public GameObject _messageObject;
    private Text _message;

    private void Start()
    {
        if (_username == null)
        {
            _username = _usernameObject.GetComponent<Text>();
        }

        if (_message == null)
        {
            _message = _messageObject.GetComponent<Text>();
        }
    }

    void SetUsername(string username)
    {
        _username.text = username;
    }

    public void SetText(string username, string text, Color color)
    {
        if(_username == null)
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
    }
}
