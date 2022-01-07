using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Missile : MonoBehaviour
{
    private int lives = 10;
    private float speed = 2.0f;
    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        StartCoroutine(DelayedDestroy(20.0f));
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
        transform.Translate(new Vector3(1.0f, 0.0f, 0.0f) * speed * Time.deltaTime, Space.World);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (view != null && view.IsMine && collision.gameObject.CompareTag("PlayerProjectile"))
        {
            lives--;
            if(lives <= 0)
            {
                StartCoroutine(DelayedDestroy(0.1f));
                //PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
