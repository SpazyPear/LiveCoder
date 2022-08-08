using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Entity
{
    public int towerHealth = 5;

    public override void die(object sender = null)
    {
        (sender as Entity).ownerPlayer.win();
        base.die(sender);
    }
}
