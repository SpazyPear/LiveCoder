using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinStore : Entity
{
    public int coinLeft { get { return coinLeft; }  set { ownerPlayer.goldLeft = value; } }

    public override void takeDamage(int damage, Character sender = null)
    {
        coinLeft -= damage * 5;
        sender.ownerPlayer.goldLeft += damage * 5;
        base.takeDamage(damage, sender);
    }
}
