using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyParticles : MonoBehaviour
{
    private AudioSource explosionSource;
    private ParticleSystem particleSystem;
    public float destroyTime = 4.5f;

    void Start()
    {
        explosionSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();

        StartCoroutine(AutoDestroy(destroyTime));
    }

    IEnumerator AutoDestroy(float _destroyTime)
    {
        yield return new WaitForSeconds(_destroyTime);
        PhotonNetwork.Destroy(gameObject);
    }
}
