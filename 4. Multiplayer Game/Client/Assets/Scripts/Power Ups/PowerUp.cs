using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PowerUp : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 2.0f;

    private float autoDestroyTime = 25.0f;

    public float activeTime = 5.0f;
    private float deactivationTimer = 0.0f;

    private Player player;
    private PhotonView view;

    private AudioSource audioSource;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(AutoDestroy(autoDestroyTime));
    }

    public void Update()
    {
        if (view != null && view.IsMine)
        {
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime, Space.World);
        }

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        if (deactivationTimer > 0.0f)
        {
            deactivationTimer -= Time.deltaTime;

            if (deactivationTimer <= 0.0f)
            {
                EndEffect(player);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                Use(player);
                deactivationTimer = activeTime;
            }
        }
    }

    public virtual void Use(Player player)
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        audioSource.Play();
    }

    IEnumerator AutoDestroy(float time)
    {
        if (view != null && view.IsMine)
        {
            yield return new WaitForSeconds(time);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public virtual void EndEffect(Player player)
    {
        if (view != null && view.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
