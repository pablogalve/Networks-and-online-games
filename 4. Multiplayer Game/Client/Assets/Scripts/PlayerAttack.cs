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
            
            shotsTimer = timeBetweenShots;
            
            if(client != null)
            {
                InstanceMessage projectileInstanceMessage = new InstanceMessage(MessageType.INSTATIATION, "id", InstanceMessage.InstanceType.PLAYER_BULLET, projectileInstance.transform.position + new Vector3(0.0f, -10.0f, 0.0f), projectileInstance.speed);
                client.Send(projectileInstanceMessage);
            }
        }
    }
}
