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
           
        }
    }
}
