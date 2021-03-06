using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Missile : MonoBehaviour
{
    public int lives = 10;
    public float speed = 0.1f;
    private PhotonView view;

    public List<GameObject> powerUps;
    public MeshRenderer meshRenderer;
    Color originalColor;

    GameObject[] players;

    public GameObject destroyParticles;

    // Start is called before the first frame update
    void Start()
    {
        originalColor = meshRenderer.material.color;
        view = GetComponent<PhotonView>();
        StartCoroutine(DelayedDestroy(10.0f));

        players = GameObject.FindGameObjectsWithTag("Player");
    }

    IEnumerator DelayedDestroy(float time)
    {
        if (view.IsMine)
        {
            yield return new WaitForSeconds(time);

            int randomPowerUp = Random.Range(0, powerUps.Count);
            PhotonNetwork.Instantiate(powerUps[randomPowerUp].name, transform.position, Quaternion.identity);
            
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if(players.Length>0 && players[0] != null)
        {
            GameObject closestPlayer = GetClosestPlayer();
            //Move towards closest player
            transform.position = Vector3.MoveTowards(transform.position, closestPlayer.transform.position, speed);

            //Rotate towards closest player
            Vector3 targetDirection = closestPlayer.transform.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 0.05f, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
            transform.Translate(new Vector3(-1.0f, 0.0f, 0.0f) * speed * Time.deltaTime, Space.World);
        }            
    }

    GameObject GetClosestPlayer()
    {
        if (players.Length == 0) return null;
        if (players.Length == 1) return players[0];

        float distance1 = Vector3.Distance(transform.position, players[0].transform.position);
        float distance2 = Vector3.Distance(transform.position, players[1].transform.position);

        if (distance1 < distance2) return players[0];
        else return players[1];
    }

    public void OnTriggerEnter(Collider collision)
    {
        StartCoroutine("Blink");
        if (view != null && view.IsMine)
        {
            if (collision.gameObject.CompareTag("PlayerProjectile"))
            {
                lives--;
                if (lives <= 0)
                {
                    StartCoroutine(DelayedDestroy(0.1f));
                }
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                PhotonNetwork.Instantiate(destroyParticles.name, transform.position, Quaternion.identity);
                PhotonNetwork.Destroy(gameObject);
            }
        }        
    }

    public IEnumerator Blink()
    {
        Color[] colors = { Color.red, Color.white, originalColor };
        for (int j = 0; j < 3; j++)
        {
            meshRenderer.material.color = colors[j];
            yield return new WaitForSeconds(0.05f);
        }
    }
}
