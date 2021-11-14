using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLogItem : MonoBehaviour
{
    RectTransform _rectTransform;

    public Text _username;
    public int userId;

    private RectTransform _messageRectTransform;
    public Text _message;

    public Text _date;

    private void Start()
    {
        if(_rectTransform == null)
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _messageRectTransform = _message.gameObject.GetComponent<RectTransform>();
        }
    }

    public void SetText(string username, string text, Color color, int id, DateTime date)
    {
        if (_rectTransform == null)
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _messageRectTransform = _message.gameObject.GetComponent<RectTransform>();
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

        if(date != null)
        {
           _date.text = date.ToString("g");
            _date.color = Color.white;
        }

        int entersAmount = text.Split('\n').Length - 1;
        _rectTransform.sizeDelta =_rectTransform.rect.size + new Vector2(0.0f, entersAmount * 10.0f);
        _messageRectTransform.sizeDelta = _messageRectTransform.rect.size + new Vector2(0.0f, entersAmount * 10.0f);
        _messageRectTransform.localPosition -= new Vector3(0.0f, entersAmount * 4.5f, 0.0f);

        userId = id;
    }

    public void ChangeUsername(string newUsername)
    {
        _username.text = newUsername;
    }
}
