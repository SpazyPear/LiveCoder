using MoonSharp.Interpreter;
using Photon.Pun;
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
        if (attacker.ownerPlayer)
        {
            attacker.ownerPlayer.creditsLeft.value += damage * 5;
            photonView.RPC("deductCredits", RpcTarget.Others, damage * 5);
        }
        base.takeDamage(damage, sender);
    }

    [PunRPC]
    public IEnumerator deductCredits(int toDeduct)
    {
        if (ownerPlayer)
        {
            if (ownerPlayer.creditsLeft.value > 0)
            {
                ownerPlayer.creditsLeft.value = Mathf.Clamp(ownerPlayer.creditsLeft.value - toDeduct, 0, ownerPlayer.creditsLeft.value - toDeduct);
            }
        }
        yield break;
    }
}
