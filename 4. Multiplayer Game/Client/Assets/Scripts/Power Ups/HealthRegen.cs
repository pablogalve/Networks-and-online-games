using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegen : PowerUp
{
    public int amountToIncrase = 1;

    public override void Use(Player player)
    {
        base.Use(player);
        player.IncreaseLives(amountToIncrase);
    }
}
