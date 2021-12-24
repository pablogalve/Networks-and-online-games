using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float defaultTimeBetweenShots;

    private Player player;

    [HideInInspector]
    public float timeBetweenShots;

    [HideInInspector]
    public float shotsTimer = 0.0f;

    public GameObject projectile;

    [SerializeField]
    private Client client;

    void Start()
    {
        timeBetweenShots = defaultTimeBetweenShots;
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (shotsTimer > 0.0f)
        {
            shotsTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && shotsTimer <= 0.0f)
        {
            Projectile projectileInstance = Instantiate(projectile, gameObject.transform.position + new Vector3(0.0f, 0.0f, 1.0f), Quaternion.identity).GetComponent<Projectile>();
            projectileInstance.udpObject = player.udpObject;

            shotsTimer = timeBetweenShots;
            
            if(client != null)
            {
                InstanceMessage projectileInstanceMessage = new InstanceMessage(MessageType.INSTANTIATE, projectileInstance.id, InstanceMessage.InstanceType.PLAYER_BULLET, projectileInstance.transform.position + new Vector3(0.0f, -10.0f, 0.0f), projectileInstance.speed);
                client.Send(projectileInstanceMessage);
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), 0.0f);
            InstanceMessage enemyInstanceMessage = new InstanceMessage(MessageType.INSTANTIATE, "-1", InstanceMessage.InstanceType.ENEMY, position, 0.0f);
            client.Send(enemyInstanceMessage);
        }
    }
}
