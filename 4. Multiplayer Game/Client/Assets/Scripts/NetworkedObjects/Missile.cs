using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Missile : MonoBehaviour
{
    public int lives = 10;
    public float speed = 10.0f;
    private PhotonView view;

    public List<GameObject> powerUps;
    public MeshRenderer meshRenderer;
    Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        originalColor = meshRenderer.material.color;
        view = GetComponent<PhotonView>();
        StartCoroutine(DelayedDestroy(10.0f));
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
        transform.Translate(new Vector3(-1.0f, 0.0f, 0.0f) * speed * Time.deltaTime, Space.World);
    }

    public void OnTriggerEnter(Collider collision)
    {
        StartCoroutine("FlashRed");
        if (view != null && view.IsMine && collision.gameObject.CompareTag("PlayerProjectile"))
        {
            lives--;
            if (lives <= 0)
            {
                StartCoroutine(DelayedDestroy(0.1f));
            }
        }
    }

    public IEnumerator FlashRed()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine("FlashWhite");
    }
    public IEnumerator FlashWhite()
    {
        meshRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine("ResetColor");
    }
    public IEnumerator ResetColor()
    {
        meshRenderer.material.color = originalColor;
        yield return new WaitForSeconds(0.1f);
    }
}
