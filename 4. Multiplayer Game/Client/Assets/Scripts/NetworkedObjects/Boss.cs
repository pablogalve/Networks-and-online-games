using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int lives = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("AttackCycle");
    }

    // Update is called once per frame
    void Update()
    {
        if (lives <= 0)
        {
            StopAllCoroutines();
            Die();
        }            
    }

    IEnumerator AttackCycle()
    {
        // Attack 1
        for(int i = 0; i < 5; ++i)
        {
            Attack1();
            yield return new WaitForSeconds(1.0f);
        }

        // Wait between attacks
        yield return new WaitForSeconds(5.0f);

        // Attack 2
        for (int i = 0; i < 5; ++i)
        {
            Attack2();
            yield return new WaitForSeconds(1.0f);
        }

        // Wait between attacks
        yield return new WaitForSeconds(5.0f);

        // Attack 3
        for (int i = 0; i < 5; ++i)
        {
            Attack3();
            yield return new WaitForSeconds(1.0f);
        }

        // Wait between attacks
        yield return new WaitForSeconds(5.0f);

        // Start the attack cycle again recursively
        StopAllCoroutines();
        StartCoroutine("AttackCycle");
    }

    void Attack1()
    {
        Debug.Log("Attack 1");
        // Shoot
    }

    void Attack2()
    {
        Debug.Log("Attack 2");
        // Shoot
    }

    void Attack3()
    {
        Debug.Log("Attack 3");
        // Shoot
    }

    void Die()
    {

    }
}
