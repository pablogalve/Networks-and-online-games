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

    private EnemyMovement enemyMovement;

    public GameObject projectile;

    private PhotonView view;

    public List<GameObject> powerUps;

    void Start()
    {
        view = GetComponent<PhotonView>();
        enemyMovement = GetComponent<EnemyMovement>();

        if (view != null && view.IsMine)
        {
            StartCoroutine(Shoot());
        }
    }

    public void Update()
    {
        //Move on server
        if (view != null && view.IsMine && enemyMovement != null)
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
            WaveManager.instance.IsWaveDone();
            GameManager.instance.AddScore(points);

            PhotonNetwork.Instantiate(destroyParticles.name, transform.position, Quaternion.identity);

            //50% probabilities
            int random = Random.Range(0, 2);
            if(random == 0)
            {
                int randomPowerUp = Random.Range(0, powerUps.Count);
                PhotonNetwork.Instantiate(powerUps[randomPowerUp].name, transform.position, Quaternion.identity);
            }

            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("Enemy collision");
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            Die();
        }
    }

}
