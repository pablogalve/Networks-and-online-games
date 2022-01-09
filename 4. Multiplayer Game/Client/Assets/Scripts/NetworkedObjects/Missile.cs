using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Missile : MonoBehaviour
{
    public int lives = 10;
    public float speed = 10.0f;
    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        StartCoroutine(DelayedDestroy(10.0f));
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
        transform.Translate(new Vector3(-1.0f, 0.0f, 0.0f) * speed * Time.deltaTime, Space.World);
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (view != null && view.IsMine && collision.gameObject.CompareTag("PlayerProjectile"))
    //    {
    //        lives--;
    //        if(lives <= 0)
    //        {
    //            StartCoroutine(DelayedDestroy(0.1f));
    //        }
    //    }
    //}
    public void OnTriggerEnter(Collider collision)
    {
        if (view != null && view.IsMine && collision.gameObject.CompareTag("PlayerProjectile"))
        {
            lives--;
            if (lives <= 0)
            {
                StartCoroutine(DelayedDestroy(0.1f));
            }
        }
    }
}
