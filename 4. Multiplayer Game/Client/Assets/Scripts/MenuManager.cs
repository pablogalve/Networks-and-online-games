using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public static MenuManager instance;
    public GameObject connectUI;

    public void Start()
    {
        MenuManager.instance = this;
        TurnConnectUI_ON();
    }


    public void TurnConnectUI_ON()
    {
        connectUI.SetActive(true);
    }

    public void TurnConnectUI_OFF()
    {
        connectUI.SetActive(false);
    }


}
