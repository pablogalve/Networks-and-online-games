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

    public Server server;

    private PhotonView view;

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
        view = GetComponent<PhotonView>();

        if (view != null && view.IsMine)
        {
            StartCoroutine(SpawnWave(currentWave, 4.0f));
        }
    }


    IEnumerator SpawnWave(int wave_num, float delayBeforeWave)
    {
        Debug.Log("Wave:" + wave_num);

        yield return new WaitForSeconds(delayBeforeWave);

        for (int i = 0; i < waves[currentWave].Count(); ++i)
        {
            PhotonNetwork.Instantiate(enemyPrefab.name, waves[currentWave][i].transform.position, Quaternion.identity);
            current_enemies++;
        }
    }

    void SpawnBoss()
    {
        Vector3 startPosition = new Vector3(28.79f, -6.5f, -35f);

        PhotonNetwork.Instantiate(bossPrefab.name, startPosition, Quaternion.identity);
    }

    public void IsWaveDone()
    {
        if (view != null && view.IsMine)
        {
            current_enemies--;

            Debug.Log("Current enemies: " + current_enemies.ToString());

            if (current_enemies == 0)
            {
                currentWave++;
                if (currentWave < waves.Count)
                {
                    StartCoroutine(SpawnWave(currentWave, 2.0f));
                }
                else
                {
                    SpawnBoss();
                }
            }
        }
    }

    public void OnBossKilled()
    {
        if (view != null && view.IsMine)
        {
            StartCoroutine("ClosServerDelay");
        }
    }

    public IEnumerator ClosServerDelay()
    {
        yield return new WaitForSeconds(0.0f);
        server.EndGame(GameResult.VICTORY);
    }
}
