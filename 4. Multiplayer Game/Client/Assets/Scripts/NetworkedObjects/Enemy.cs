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

    PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
        enemyMovement = GetComponent<EnemyMovement>();

        //StartCoroutine(Shoot());
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
            GameObject GO = PhotonNetwork.Instantiate(projectile.name, shootPoint.transform.position, Quaternion.identity); // Create projectile
            Projectile projScript = GO.GetComponent<Projectile>(); // Access script of projectile and set direction
            // Set projectile direction
            if (projScript != null)
            {
                projScript.SetDirection(Projectile.ProjectileDirection.LEFT_STRAIGHT);
            }
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    public void Die()
    {
        if (view != null && view.IsMine)
        {
            GameManager.instance.AddScore(points);
            WaveManager.instance.IsWaveDone();

            PhotonNetwork.Instantiate(destroyParticles.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy collision");
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            Die();
        }
    }
}
