using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public GameObject enemy;

    private GameObject[] currentWave;

    [SerializeField]
    private GameObject[] waveEnemies_1;

    [SerializeField]
    private GameObject[] waveEnemies_2;

    [SerializeField]
    private GameObject[] waveEnemies_3;

    [SerializeField]
    private GameObject[] waveEnemies_4;

    [SerializeField]
    private GameObject[] waveEnemies_5;

    [SerializeField]
    private GameObject[] waveEnemies_6;

    [SerializeField]
    private GameObject[] waveEnemies_7;

    [SerializeField]
    private GameObject[] waveEnemies_8;

    [SerializeField]
    private GameObject[] waveEnemies_9;


    static int waveCount = 0;
    int oldWaveCount = 0;
    static int current_enemies = 0;

    // Start is called before the first frame update
    void Start()
    {
        WaveManager.instance = this;
        //SpawnWave(waveCount);
    }

    public void StartGame()
    {
        if(waveCount == 0)
        {
            SpawnWave(waveCount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(waveCount!=oldWaveCount)
        {
            SpawnWave(waveCount);
            oldWaveCount = waveCount;

        }
    }

   void SpawnWave(int wave_num)
    {
        Debug.Log("Wave:" + wave_num);

        #region pickWave
        switch(wave_num)
        {
            case 0:
                currentWave = waveEnemies_1;
                break;
            case 1:
                currentWave = waveEnemies_2;
                break;
            case 2:
                currentWave = waveEnemies_3;
                break;
            case 3:
                currentWave = waveEnemies_4;
                break;
            case 4:
                currentWave = waveEnemies_5;
                break;
            case 5:
                currentWave = waveEnemies_6;
                break;
            case 6:
                currentWave = waveEnemies_7;
                break;
            case 7:
                currentWave = waveEnemies_8;
                break;
            case 8:
                currentWave = waveEnemies_9;
                break;
        }
        #endregion pickWave

        for (int i = 0; i < currentWave.Length; ++i)
        {
            PhotonNetwork.Instantiate(enemy.name, currentWave[i].transform.position, Quaternion.identity);

            //GameObject enemyInstance = Instantiate(enemy, currentWave[i].transform.position, Quaternion.identity);
            //server.InstantiateToAll(enemy, InstanceMessage.InstanceType.ENEMY, currentWave[i].transform.position, Quaternion.Euler(new Vector3(0.0f, -0.0f, 0.0f)));
            current_enemies++;
        }
    }

    public static void IsWaveDone()
    {
        current_enemies--;
        if(current_enemies==0)
        {
            waveCount++;
            current_enemies = 0;

            if(waveCount >= 8)
            {
               //WaveManager.instance.server.GameOver();
            }
        }
    }
}
