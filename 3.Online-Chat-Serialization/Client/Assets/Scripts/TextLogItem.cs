using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLogItem : MonoBehaviour
{
    public Text _username;

    public Text _message;

    void SetUsername(string username)
    {
        _username.text = username;
    }

    public void SetText(string username, string text, Color color)
    {
        _message = gameObject.GetComponent<Text>();

        if (_message != null)
        {
            _message.text = text;
            _message.color = Color.white;
        }

        if (_username != null)
        {
            _username.text = username;
            _username.color = color;
        }
    }
}
