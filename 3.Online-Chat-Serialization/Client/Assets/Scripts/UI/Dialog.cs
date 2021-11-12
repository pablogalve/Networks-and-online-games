using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        animator.SetBool("open", true);
    }

    public void Close()
    {
        animator.SetBool("open", false);
    }
}
