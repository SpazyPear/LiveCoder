using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Entity
{
    public int towerHealth = 5;

    public override void die(Character sender = null)
    {
        sender.ownerPlayer.win();
        base.die(sender);
    }
}
