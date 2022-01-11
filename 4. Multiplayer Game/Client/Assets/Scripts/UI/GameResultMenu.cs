using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultMenu : MonoBehaviour
{
    Animator animator;

    public Text title;
    public Text score;
    public Text time;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        OpenMenu();
    }

    public void OpenMenu()
    {
        animator.SetTrigger("OpenMenu");
    }

    public void CloseMenu()
    {
        animator.SetTrigger("CloseMenu");
    }

    public void SetLabels(GameResult gameResult)
    {
        title.text = gameResult.ToString();

        if (gameResult == GameResult.VICTORY)
        {
            title.color = Color.green;
        }
        else if (gameResult == GameResult.DEFEAT)
        {
            title.color = Color.red;
        }
        else
        {
            title.color = new Color(255, 140, 0);
        }

        score.text = "Score: " + GameManager.instance.GetScore().ToString();

        System.TimeSpan timePassed = System.TimeSpan.FromSeconds(GameManager.instance.timePassed);
        time.text = "Time: " + FormatTime(timePassed);
    }

    public void OnMenuClosed()
    {
        SceneManager.LoadScene("MainMenu");
    }
    private string FormatTime(System.TimeSpan timeSpan)
    {
        string formattedTime = timeSpan.Minutes < 10 ? "0" + timeSpan.Minutes.ToString() : timeSpan.Minutes.ToString();
        formattedTime += ":" + (timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds.ToString() : timeSpan.Seconds.ToString());

        return formattedTime;
    }
}
