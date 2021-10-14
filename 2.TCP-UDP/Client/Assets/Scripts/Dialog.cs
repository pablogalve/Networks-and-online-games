using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public Text text;
    public float timeToReset = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(Camera.main.transform.position);
        text.text = "...";
    }

    public void SetMessage(string message)
    {
        text.text = message;

        StartCoroutine(ResetMessage());
    }

    IEnumerator ResetMessage()
    {
        yield return new WaitForSeconds(timeToReset);

        text.text = "...";
    }

}
