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

        networkedObjectType = NetworkedObjectType.ENEMY;
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
        GameManager.instance.AddScore(points);
        WaveManager.IsWaveDone();
        base.Die();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        switch (collision.gameObject.tag)
        {
            case "Projectile":
                //Die();
                break;
        }
    }
}
