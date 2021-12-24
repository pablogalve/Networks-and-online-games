using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemy;

    private GameObject[] currentWave;

    [SerializeField]
    private GameObject[] waveEnemies_1;

        [SerializeField]
    private GameObject[] waveEnemies_2;

    //[SerializeField]
    //private GameObject[] waveEnemies_3;

    //[SerializeField]
    //private GameObject[] waveEnemies_4;

    //[SerializeField]
    //private GameObject[] waveEnemies_5;

    //[SerializeField]
    //private GameObject[] waveEnemies_6;

    //[SerializeField]
    //private GameObject[] waveEnemies_7;

    //[SerializeField]
    //private GameObject[] waveEnemies_8;

    //[SerializeField]
    //private GameObject[] waveEnemies_9;


    static int waveCount = 0;
    int oldWaveCount = 0;
    static int current_enemies = 0;


    // Start is called before the first frame update
    void Start()
    {
        SpawnWave(waveCount);
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
        }
        #endregion pickWave


        for (int i = 0; i < currentWave.Length; i++)
        {
            GameObject enemyInstance = Instantiate(enemy, currentWave[i].transform.position, Quaternion.identity);
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
        }
    }
}