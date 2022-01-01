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
    }

    private void Update()
    {
        //Debug.Log("Ping: " + PhotonNetwork.GetPing().ToString());
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        Debug.Log(score);
    }
}
