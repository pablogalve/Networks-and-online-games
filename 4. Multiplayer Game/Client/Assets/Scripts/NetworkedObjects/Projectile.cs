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
    PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        StartCoroutine(DelayedDestroy(5.0f));
    }

    IEnumerator DelayedDestroy(float time)
    {
        if (view.IsMine)
        {
            yield return new WaitForSeconds(time);
            PhotonNetwork.Destroy(gameObject);
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

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile collision with: " + collision.gameObject.tag);

        if (view != null && view.IsMine && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")))
        {
            StartCoroutine(DelayedDestroy(0.1f));
            //PhotonNetwork.Destroy(gameObject);
        }
    }

}
