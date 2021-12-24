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

        if(udpObject.connectionType == ConnectionType.SERVER)
        {
            Collider collider = GetComponent<Collider>();
            if(collider != null)
            {
                //TODO: Uncomment
                collider.enabled = false;
            }
        }
    }

    public override void Update()
    {
        //Move on server
        if (GameManager.instance.udpObject.connectionType == ConnectionType.SERVER)
        {
            Move();
        }
        //Synchronize position from client
        else
        {
            base.Update();
        }
    }

    private void Move()
    {
        
    }

    public override void Die()
    {
        base.Die();

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
