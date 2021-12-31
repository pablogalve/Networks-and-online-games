using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int lives = 5;

    public float timerBetweenAttacks = 5.0f;
    public float timerBetweenShots = 1.0f;
    public int totalShotsPerAttack = 5;

    public GameObject destroyParticles;

    [SerializeField]
    private int points = 25;

    public GameObject shootPoint;
    public GameObject projectile;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("AttackCycle");
    }

    // Update is called once per frame
    void Update()
    {
        if (lives <= 0)
        {
            StopAllCoroutines();
            Die();
        }            
    }

    IEnumerator AttackCycle()
    {
        // Wait between attacks
        yield return new WaitForSeconds(timerBetweenAttacks);

        // Attack 1
        for (int i = 0; i < totalShotsPerAttack; ++i)
        {
            Attack1();
            yield return new WaitForSeconds(timerBetweenShots);
        }

        // Wait between attacks
        yield return new WaitForSeconds(timerBetweenAttacks);

        // Attack 2
        for (int i = 0; i < totalShotsPerAttack; ++i)
        {
            Attack2();
            yield return new WaitForSeconds(timerBetweenShots);
        }

        // Wait between attacks
        yield return new WaitForSeconds(timerBetweenAttacks);

        // Attack 3
        for (int i = 0; i < totalShotsPerAttack; ++i)
        {
            Attack3();
            yield return new WaitForSeconds(timerBetweenShots);
        }        

        // Start the attack cycle again recursively
        StopAllCoroutines();
        StartCoroutine("AttackCycle");
    }

    void Attack1()
    {
        //PhotonNetwork.Instantiate(projectile.name, shootPoint.transform.position, Quaternion.Euler(0, 0, -15.0f));
        PhotonNetwork.Instantiate(projectile.name, shootPoint.transform.position, Quaternion.identity);
        //PhotonNetwork.Instantiate(projectile.name, shootPoint.transform.position, Quaternion.Euler(0, 0, 15.0f));
        Debug.Log("Attack 1");
        // Shoot
    }

    void Attack2()
    {
        Debug.Log("Attack 2");
        // Shoot
    }

    void Attack3()
    {
        Debug.Log("Attack 3");
        // Shoot
    }

    void Die()
    {
        GameManager.instance.AddScore(points);
        WaveManager.IsWaveDone();

        PhotonNetwork.Instantiate(destroyParticles.name, transform.position, Quaternion.identity);

        PhotonNetwork.Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            lives--;
        }
    }
}
