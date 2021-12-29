using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public static class StaticVariables
{
    //public static string userName;
    public static IPAddress userPointIP = null;
    public static readonly int defaultPort = 7777;

    public static void CleanUp()
    {
        StaticVariables.userPointIP = null;
    }
}


public class BH_LoadScene : MonoBehaviour
{

    //public InputField inputName;
    public InputField inputIP;

    List<string> blockedChars = new List<string> { " ", "{", "}", "{}", "}{"};

    public void OpenClient()
    {
        bool ValidateIP = IPAddress.TryParse(inputIP.text, out StaticVariables.userPointIP);




        if (ValidateIP /*&& inputName.text.Length != 0*/)
        {
            bool isValidUserName = true;

            //foreach (var item in blockedChars)
            //{
            //    if (inputName.text.Contains(item) == true)
            //    {
            //        isValidUserName = false;
            //        Debug.LogWarning(string.Format("Char {0} can't be used in a name, also names must be one word only", item));
            //        break;
            //    }
            //}
            

            if (isValidUserName == true)
            {
                //StaticVariables.userName = inputName.text;
                SceneManager.LoadSceneAsync(1);
            }
        }
    }
}
