using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public GameObject mainPlayerHUD;
    public GameObject liveHolder;
    public Slider healthDisplay;

    public Slider bossHealth;

    public void Start()
    {
        MenuManager.instance = this;
        bossHealth.gameObject.SetActive(false);
        //TurnConnectUI_ON();
    }


    public void TurnConnectUI_ON()
    {
        if (mainPlayerHUD != null)
        {
            mainPlayerHUD.SetActive(true);
        }
        //connectUI.SetActive(true);
    }

    public void TurnConnectUI_OFF()
    {
        if (mainPlayerHUD != null)
        {
            mainPlayerHUD.SetActive(false);
        }
        //connectUI.SetActive(false);
    }


}
