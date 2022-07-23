using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinStoreProxy
{
    Entity target;

    [MoonSharpHidden]
    public CoinStoreProxy(CoinStore p)
    {
        this.target = p;
    }

}

public class CoinStore : Entity
{    
    public override void takeDamage(int damage, object sender = null)
    {
        Entity attacker = sender as Entity;
        if (attacker)
        {
            ownerPlayer.creditsLeft.value -= damage * 5;
            attacker.ownerPlayer.creditsLeft.value += damage * 5;
            base.takeDamage(damage, sender);
        }
    }
}
