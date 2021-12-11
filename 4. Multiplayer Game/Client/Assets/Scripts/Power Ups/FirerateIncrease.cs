using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirerateIncrease : PowerUp
{
    public float newTimeBetweenShots;

    public override void Use(Player player)
    {
        base.Use(player);

        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
        playerAttack.timeBetweenShots = newTimeBetweenShots;
        playerAttack.shotsTimer = 0.0f;
    }

    public override void EndEffect(Player player)
    {
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
        playerAttack.timeBetweenShots = playerAttack.defaultTimeBetweenShots;

        base.EndEffect(player);
    }
}
