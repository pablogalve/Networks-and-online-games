using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float defaultTimeBetweenShots;
    
    [HideInInspector]
    public float timeBetweenShots;
    
    [HideInInspector]
    public float shotsTimer = 0.0f;

    public GameObject projectile;

    void Start()
    {
        timeBetweenShots = defaultTimeBetweenShots;
    }

    void Update()
    {
        if(shotsTimer > 0.0f)
        {
            shotsTimer -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Space) && shotsTimer <= 0.0f)
        {
           GameObject projectileInstance = Instantiate(projectile, gameObject.transform.position + new Vector3(0.0f, 0.0f, 1.0f), Quaternion.identity);
            shotsTimer = timeBetweenShots;
        }
    }
}
