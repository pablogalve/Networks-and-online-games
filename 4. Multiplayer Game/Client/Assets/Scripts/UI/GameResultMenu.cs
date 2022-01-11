using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultMenu : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        //OpenMenu();
    }

    private void OnEnable()
    {
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

    public void OnMenuClosed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
