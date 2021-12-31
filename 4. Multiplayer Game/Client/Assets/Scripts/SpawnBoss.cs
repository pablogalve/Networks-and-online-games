using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnBoss : MonoBehaviour
{
    public GameObject bossPrefab;
    public Vector3 spawnPosition;

    void Start()
    {
        Vector3 startPosition = new Vector3(26.79f, -0.6f, -35f);
        PhotonNetwork.Instantiate(bossPrefab.name, startPosition, Quaternion.identity);
    }

}
