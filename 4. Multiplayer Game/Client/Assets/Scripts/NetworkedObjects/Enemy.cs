using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NetworkedObject
{
    [SerializeField]
    private int points = 5;

    public GameObject destroyParticles;

    void Start()
    {
        base.Init();
        networkedObjectType = NetworkedObjectType.ENEMY;
    }

    public override void Update()
    {
        base.Update();

        Move();
    }

    private void Move()
    {
        
    }

    private void OnDestroy()
    {
        //Spawn paprticles
        GameObject particlesInstance = Instantiate(destroyParticles, transform.position, Quaternion.identity);
        ParticleSystem particleSystem = particlesInstance.GetComponent<ParticleSystem>();
        particleSystem.Play();
        Destroy(particlesInstance, particleSystem.main.duration * 0.8f);

        GameManager.instance.AddScore(points);
        WaveManager.IsWaveDone();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            base.OnCollisionEnter(collision);
        }
    }
}
