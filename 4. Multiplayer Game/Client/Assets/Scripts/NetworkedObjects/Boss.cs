using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int lives = 5;

    public float timerBetweenAttacks = 5.0f;
    public float timerBetweenShots = 1.0f;
    public int totalShotsPerAttack = 5;

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
        for(int i = 0; i < totalShotsPerAttack; ++i)
        {
            Attack1();
            yield return new WaitForSeconds(timerBetweenShots);
        }

        // Wait between attacks
        yield return new WaitForSeconds(timerBetweenAttacks);

        // Attack 2
        for (int i = 0; i < totalShotsPerAttack; ++i)
        {
            Attack2();
            yield return new WaitForSeconds(timerBetweenShots);
        }

        // Wait between attacks
        yield return new WaitForSeconds(timerBetweenAttacks);

        // Attack 3
        for (int i = 0; i < totalShotsPerAttack; ++i)
        {
            Attack3();
            yield return new WaitForSeconds(timerBetweenShots);
        }

        // Wait between attacks
        yield return new WaitForSeconds(timerBetweenAttacks);

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
        Debug.Log("Enemy died");
    }
}
