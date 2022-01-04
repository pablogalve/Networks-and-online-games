using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    private int currentWave = 0;

    [System.Serializable]
    public class ListWrapper
    {
        public List<GameObject> myList;

        public int Count()
        {
            return myList.Count;
        }

        public GameObject this[int index]
        {
            get => myList[index];
            set => myList[index] = value;
        }
    }


    [SerializeField]
    public List<ListWrapper> waves;

    static int current_enemies = 0;

    // Start is called before the first frame update
    void Start()
    {
        WaveManager.instance = this;
        //SpawnWave(waveCount);
    }

    public void StartGame()
    {
        //SpawnBoss();

        SpawnWave(currentWave);
    }


    void SpawnWave(int wave_num)
    {
        Debug.Log("Wave:" + wave_num);

        for (int i = 0; i < waves[currentWave].Count(); ++i)
        {
            PhotonNetwork.Instantiate(enemyPrefab.name, waves[currentWave][i].transform.position, Quaternion.identity);
            current_enemies++;
        }
    }

    void SpawnBoss()
    {
        Vector3 startPosition = new Vector3(26.79f, -0.6f, -35f);
        PhotonNetwork.Instantiate(bossPrefab.name, startPosition, Quaternion.identity);
    }

    public void IsWaveDone()
    {
        current_enemies--;
        if (current_enemies == 0)
        {
            currentWave++;
            if (currentWave > waves.Count)
            {
                SpawnBoss();
            }
        }
    }
}
