using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NetworkedObject
{
    [SerializeField]
    private int points = 5;

    public GameObject particles;

    void Start()
    {
        base.Init();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        
    }

    public override void Die()
    {
        GameManager.instance.InstantiateParticles(particles, transform.position);
        GameManager.instance.AddScore(points);
        WaveManager.IsWaveDone();
        base.Die();
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Projectile":
                Die();
                break;
        }
    }
}
