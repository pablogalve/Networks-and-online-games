using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatisticsScreenManager : MonoBehaviour
{
   public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
