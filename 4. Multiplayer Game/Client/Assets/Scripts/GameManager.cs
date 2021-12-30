using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Server serverPrefab;

    private int score = 0;

    void Start()
    {
        instance = this;

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject serverObject = PhotonNetwork.Instantiate(serverPrefab.name, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

            Server server = serverObject.GetComponent<Server>();
            server.Init();
        }
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        Debug.Log(score);
    }
}
