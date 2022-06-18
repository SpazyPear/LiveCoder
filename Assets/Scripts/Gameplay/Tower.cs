using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Entity
{
    public PlayerManager belongingPlayer;
    public int towerHealth = 5;

    /*public override float takeDamage(int damage)
    {
        float healthLeft = base.takeDamage(damage);
        if (healthLeft == -1)

    }*/
}
