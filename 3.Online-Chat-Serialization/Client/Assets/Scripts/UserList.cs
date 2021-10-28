using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserList : MonoBehaviour
{
    public GameObject userPrefab;
    private List<GameObject> users;

    // Start is called before the first frame update
    void Start()
    {
        users = new List<GameObject>();
    }

    public void AddUsers(List<string> usernames)
    {
        for(int i = 0; i < usernames.Count; ++i)
        {
            AddNewUser(usernames[i]);
        }
    }

    private void AddNewUser(string username)
    {
        if(userPrefab != null)
        {
            GameObject userInstance = Instantiate(userPrefab, transform.position, transform.rotation);
            GameObject usernameObject = userInstance.transform.Find("username").gameObject;

            if(usernameObject != null) 
            {
                Text text = usernameObject.GetComponent<Text>();
                if(text != null)
                {
                    text.text = username;
                }
            }

            users.Add(userInstance);
            userInstance.transform.parent = gameObject.transform;
        }
    }
}
