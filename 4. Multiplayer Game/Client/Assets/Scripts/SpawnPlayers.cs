using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public GameObject serverPrefab;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(serverPrefab.name, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        }
        else
        {
            Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);
            PhotonNetwork.Instantiate(playerPrefab.name, startPosition, Quaternion.identity);
        }
    }
}
