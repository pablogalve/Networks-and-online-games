using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemy;

    [SerializeField]
    private GameObject[] totalWaves;


    [SerializeField]
    private GameObject[] firstWave_enemies;

    int waveCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        CheckWave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckWave()
    {
        switch(waveCount)
        {
            case 0:
                SpawnWave(0);
                break;
        }
    }

    void SpawnWave(int wave_number)
    {
        if(wave_number==0)
        {
            Debug.Log(firstWave_enemies[0].transform.position.x);
            GameObject enemyInstance = Instantiate(enemy, firstWave_enemies[0].transform.position, Quaternion.identity);
            
        }
    }

    public static void IsWaveDone()
    {

    }
}
