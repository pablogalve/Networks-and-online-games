using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyParticles : MonoBehaviour
{
    private AudioSource explosionSource;
    private ParticleSystem particleSystem;
    public float destroyTime = 4.5f;

    private PhotonView view;

    void Start()
    {
        explosionSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();

        view = GetComponent<PhotonView>();

        if(view != null && view.IsMine)
        {
            StartCoroutine(AutoDestroy(destroyTime));
        }
    }

    IEnumerator AutoDestroy(float _destroyTime)
    {
        if (view != null && view.IsMine)
        {
            yield return new WaitForSeconds(_destroyTime);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
