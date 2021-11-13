using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLogItem : MonoBehaviour
{
    public GameObject _usernameObject;
    private Text _username;
    public int userId;

    RectTransform _rectTransform;
    RectTransform _textRectTransform;

    public GameObject _messageObject;
    private Text _message;

    private void Start()
    {
        if(_rectTransform == null)
        {
            _username = _usernameObject.GetComponent<Text>();
            _message = _messageObject.GetComponent<Text>();

            _rectTransform = gameObject.GetComponent<RectTransform>();
            _textRectTransform = _messageObject.GetComponent<RectTransform>();
        }
    }

    public void SetText(string username, string text, Color color, int id)
    {
        if (_rectTransform == null)
        {
            _username = _usernameObject.GetComponent<Text>();
            _message = _messageObject.GetComponent<Text>();

            _rectTransform = gameObject.GetComponent<RectTransform>();
            _textRectTransform = _messageObject.GetComponent<RectTransform>();
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
        _rectTransform.sizeDelta =_rectTransform.rect.size + new Vector2(0.0f, entersAmount * 10.0f);
        _textRectTransform.sizeDelta = _textRectTransform.rect.size + new Vector2(0.0f, entersAmount * 10.0f);
        _textRectTransform.localPosition -= new Vector3(0.0f, entersAmount * 4.5f, 0.0f);

        userId = id;
    }

    public void ChangeUsername(string newUsername)
    {
        _username.text = newUsername;
    }
}
