using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject projectile;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
           GameObject projectileInstance = Instantiate(projectile, gameObject.transform.position, Quaternion.identity);
        }
    }
}
