using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyParticles : MonoBehaviour
{
    private new ParticleSystem particleSystem;
    public float destroyTime = 4.5f;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine(AutoDestroy(destroyTime));
    }

    IEnumerator AutoDestroy(float _destroyTime)
    {
        yield return new WaitForSeconds(_destroyTime);
        PhotonNetwork.Destroy(gameObject);
    }
}
