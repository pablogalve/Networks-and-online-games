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
    DEFEAT,
    DISCONNECTION
}

public class Server : MonoBehaviourPunCallbacks
{
    public GameObject waveManagerPrefab;
    public int neededClients = 1;
    public PhotonView view;

    private bool gameStarted = false;
    private bool gameFinished = false;

    int alivePlayersCount = 0;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        GameManager.instance.server = this;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.Log("Player entered room");
        alivePlayersCount++;

        //TODO: Change to 3 for 2 players needed
        if (!gameStarted && PhotonNetwork.CurrentRoom.PlayerCount == neededClients + 1)
        {
            Debug.Log("Starting game");
            StartGame();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (view != null && view.IsMine)
        {
            if (!gameFinished && PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("ServerMenu");
            }
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        gameFinished = false;

        if(view != null && view.IsMine)
        {
            GameObject waveManagerObject = PhotonNetwork.Instantiate(waveManagerPrefab.name, transform.position, transform.rotation);

            WaveManager waveManager = waveManagerObject.GetComponent<WaveManager>();
            waveManager.server = this;
            waveManager.StartGame();
        }
    }

    public void EndGame(GameResult gameResult)
    {
        gameFinished = true;

        if (view != null && view.IsMine)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            StartCoroutine(DelayedRoomClose(15.0f, gameResult));
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    IEnumerator DelayedRoomClose(float time, GameResult gameResult)
    {
        view.RPC("OnGameEnded", RpcTarget.Others, gameResult);

        yield return new WaitForSeconds(time);

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("ServerMenu");

        gameStarted = false;
    }

    [PunRPC]
    public void OnPlayerDead()
    {
        alivePlayersCount--;
        if(alivePlayersCount <= 0)
        {
            EndGame(GameResult.DEFEAT);
        }
    }

    [PunRPC]
    void OnGameEnded(GameResult gameResult, PhotonMessageInfo info)
    {
        //Debug.Log("End game");

        GameManager.instance.OnGameEnded(gameResult);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (view != null && !view.IsMine)
        {
            GameManager.instance.OnGameEnded(GameResult.DISCONNECTION);
        }
    }

    [PunRPC]
    public void InstantiatePrefab(string prefabName, DateTime instantationTime, Vector3 position)
    {
        GameObject prefab = GameManager.instance.GetReplicableObjectByName(prefabName);
        Instantiate(prefab, position, Quaternion.identity);
    }
}
