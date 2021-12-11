using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float activeTime = 5.0f;
    private float deactivationTimer = 0.0f;

    private Player player;

    private void Update()
    {
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
    }

    public virtual void EndEffect(Player player)
    {
        Destroy(gameObject);
    }
}
