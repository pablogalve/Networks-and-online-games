using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    public enum ProjectileDirection
    {
        LEFT_STRAIGHT,
        DIAGONAL_UP,
        DIAGONAL_DOWN,
    }
    public ProjectileDirection projectileDirection;
    public float speed = 5.0f;
    public GameObject destroyParticles;
    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        //StartCoroutine(DelayedDestroy(5.0f));
    }

    IEnumerator DelayedDestroy(float time)
    {
        if (view != null && view.IsMine)
        {
            yield return new WaitForSeconds(time);

            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.Instantiate(destroyParticles.name, transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        switch (projectileDirection)
        {
            case ProjectileDirection.DIAGONAL_UP:
                transform.Translate(new Vector3(1.0f, -0.4f, 0.0f) * speed * Time.deltaTime, Space.World);
                break;

            case ProjectileDirection.LEFT_STRAIGHT:
                transform.Translate(new Vector3(1.0f, 0.0f, 0.0f) * speed * Time.deltaTime, Space.World);
                break;

            case ProjectileDirection.DIAGONAL_DOWN:
                transform.Translate(new Vector3(1.0f, 0.4f, 0.0f) * speed * Time.deltaTime, Space.World);
                break;

            default:
                Debug.LogWarning("Projectile direction not set");
                break;
        }
    }

    public void SetDirection(ProjectileDirection newDirection)
    {
        projectileDirection = newDirection;
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    //debug.log("projectile collision with: " + collision.gameobject.tag);

    //    if (view != null && view.IsMine && (collision.gameObject.CompareTag("Player")))
    //    {
    //        //StartCoroutine(DelayedDestroy(0.1f));
    //    }
    //    else if (view != null && view.IsMine && (collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("Enemy")))
    //    {
    //        //StartCoroutine(DelayedDestroy(0.1f));
    //    }
    //}

    public void OnTriggerEnter(Collider collision)
    {
        //debug.log("projectile collision with: " + collision.gameobject.tag);

        if (view != null && view.IsMine && (collision.gameObject.CompareTag("Player")))
        {
            //StartCoroutine(DelayedDestroy(0.1f));
        }
        else if (view != null && view.IsMine && (collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("Enemy")))
        {
            StartCoroutine(DelayedDestroy(0.1f));
        }
    }

}
