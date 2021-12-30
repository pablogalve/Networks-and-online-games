using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int points = 5;

    public float timeBetweenShots = 1.0f;

    public GameObject shootPoint;

    public GameObject destroyParticles;

    EnemyMovement enemyMovement;

    public GameObject projectile;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();

        StartCoroutine(Shoot());

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            //TODO: Uncomment
            collider.enabled = false;
        }

    }

    public void Update()
    {
        //Move on server

        if (enemyMovement != null)
        {
            enemyMovement.Move();

        }
    }


    public IEnumerator Shoot()
    {
        while (gameObject.activeSelf)
        {
            PhotonNetwork.Instantiate(projectile.name, shootPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    public void Die()
    {
        GameManager.instance.AddScore(points);
        WaveManager.IsWaveDone();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
        }
    }
}
