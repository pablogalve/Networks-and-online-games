using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public enum GameResult
{
    VICTORY,
    DEFEAT
}

public class Server : MonoBehaviourPunCallbacks
{
    public GameObject waveManagerPrefab;
    public int neededClients = 1;
    private PhotonView view;

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.Log("Player entered room");

        //TODO: Change to 3 for 2 players needed
        if (PhotonNetwork.CurrentRoom.PlayerCount == neededClients + 1)
        {
            Debug.Log("Starting game");
            StartGame();
        }
    }

    public void StartGame()
    {
        view = GetComponent<PhotonView>();

        GameObject waveManagerObject = PhotonNetwork.Instantiate(waveManagerPrefab.name, transform.position, transform.rotation);

        WaveManager waveManager = waveManagerObject.GetComponent<WaveManager>();
        waveManager.server = this;
        waveManager.StartGame();
    }

    public void EndGame(GameResult gameResult)
    {
        if (view != null && view.IsMine)
        {
            if (gameResult == GameResult.VICTORY)
            {

            }
            else
            {

            }

            PhotonNetwork.CurrentRoom.IsOpen = false;
            view.RPC("OnGameEnded", RpcTarget.Others, gameResult);

            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("ServerMenu");
        }
    }

    [PunRPC]
    void OnGameEnded(GameResult gameResult, PhotonMessageInfo info)
    {
        Debug.Log("End game");
        SceneManager.LoadScene("MainMenu");
    }
}
