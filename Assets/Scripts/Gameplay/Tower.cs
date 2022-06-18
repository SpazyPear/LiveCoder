using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Entity
{
    public PlayerManager belongingPlayer;
    public int towerHealth = 5;

    public override void die(Character sender = null)
    {
        GameManager.findPlayer(sender.ownerPlayer).win();
        base.die(sender);
    }
}
