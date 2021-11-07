using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLogItem : MonoBehaviour
{
    private Text _text;

    public void SetText(string text, Color color)
    {
        _text = gameObject.GetComponent<Text>();

        _text.text = text;
        _text.color = color;
    }
}
