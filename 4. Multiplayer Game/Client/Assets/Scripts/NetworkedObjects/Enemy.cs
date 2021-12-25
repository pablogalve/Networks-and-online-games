using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NetworkedObject
{
    [SerializeField]
    private int points = 5;

    public GameObject destroyParticles;

    EnemyMovement enemyMovement;

    public GameObject projectile;

    void Start()
    {
        base.Init();
        networkedObjectType = NetworkedObjectType.ENEMY;

        enemyMovement = GetComponent<EnemyMovement>();

        if(udpObject.connectionType == ConnectionType.SERVER)
        {
            StartCoroutine(Shoot());

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
        if (udpObject.connectionType == ConnectionType.SERVER)
        {
            if(enemyMovement != null)
            {
                enemyMovement.Move();
                Server server = udpObject as Server;

                VectorMessage positionMessage = new VectorMessage(MessageType.OBJECT_POSITION, id, transform.position);
                server.SendMessageToBothPlayers(positionMessage);
            }
        }
        //Synchronize position from client
        else
        {
            base.Update();
        }
    }

    public IEnumerator Shoot()
    {
        while(gameObject.activeSelf)
        {
            Server server = udpObject as Server;
            server.InstantiateToAll(projectile, InstanceMessage.InstanceType.ENEMY_BULLET, transform.position - new Vector3(-1.0f, 0.0f, 0.0f), Quaternion.identity);

            yield return new WaitForSeconds(1.0f);
        }
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
