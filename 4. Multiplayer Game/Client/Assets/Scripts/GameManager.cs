using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Server serverPrefab;

    private int score = 0;

    public GameResultMenu gameResultMenu;

    public float timePassed = 0.0f;

    void Start()
    {
        instance = this;
        timePassed = 0.0f;
    }

    private void Update()
    {
        //Debug.Log("Ping: " + PhotonNetwork.GetPing().ToString());
        timePassed += Time.deltaTime;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        //Debug.Log(score);
    }

    public int GetScore()
    {
        return score;
    }

    public void OnGameEnded(GameResult gameResult)
    {
        gameResultMenu.gameObject.SetActive(true);
        gameResultMenu.SetLabels(gameResult);
    }
}
