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
    public GameObject missilePrefab;

    PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
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
        Attack3();

        // Start the attack cycle again recursively
        StopAllCoroutines();
        StartCoroutine("AttackCycle");
    }

    void Attack1()
    {
        /*The idea of Attack1 is that the boss will shoot bursts of 3 projectiles each
        Direction of the projectiles is the following
        1st one: Left and diagonally up
        2nd one: Left and straight
        3rd one: Left and diagonally down
         */

        // Create projectiles
        if (view.IsMine)
        {
            GameObject projectile1 = PhotonNetwork.Instantiate(projectile.name, shootPoint.transform.position + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.Euler(0, 0, -15.0f));
            GameObject projectile2 = PhotonNetwork.Instantiate(projectile.name, shootPoint.transform.position, Quaternion.identity);
            GameObject projectile3 = PhotonNetwork.Instantiate(projectile.name, shootPoint.transform.position + new Vector3(0.0f, -1.0f, 0.0f), Quaternion.Euler(0, 0, 15.0f));

            // Access script of projectile and set direction
            Projectile proj1Script = projectile1.GetComponent<Projectile>();
            Projectile proj2Script = projectile2.GetComponent<Projectile>();
            Projectile proj3Script = projectile3.GetComponent<Projectile>();

            // Set projectile directions
            if (proj1Script != null)
            {
                proj1Script.SetDirection(Projectile.ProjectileDirection.DIAGONAL_UP);
            }
            if (proj2Script != null)
            {
                proj2Script.SetDirection(Projectile.ProjectileDirection.LEFT_STRAIGHT);
            }
            if (proj3Script != null)
            {
                proj3Script.SetDirection(Projectile.ProjectileDirection.DIAGONAL_DOWN);
            }
        }
        Debug.Log("Boss: Attack 1");
    }

    void Attack2()
    {
        Debug.Log("Boss: Attack 2");
        // Shoot
    }

    void Attack3()
    {
        Debug.Log("Boss: Attack 3");
        // Shoot

        if (view.IsMine)
        {
            GameObject missile = PhotonNetwork.Instantiate(missilePrefab.name, shootPoint.transform.position, Quaternion.identity);

            // Access script of projectile and set direction
            /*Missile missileScript = missile.GetComponent<Missile>();

            // Set projectile directions
            if (missileScript != null)
            {
                
            }   */         
        }
    }

    void Die()
    {
        GameManager.instance.AddScore(points);

        if (view.IsMine)
        {
            WaveManager.instance.OnBossKilled();
            PhotonNetwork.Instantiate(destroyParticles.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            lives--;
        }
    }
}
