using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CoinStoreProxy
{
    CoinStore target;

    public CoinStoreProxy(CoinStore p)
    {
        this.target = p;
    }
    
}

public class CoinStore : Entity
{
    public PlayerManager ownerPlayer;
    public PhotonView photonView;

    private void Awake()
    {
        ownerPlayer = GameManager.activePlayer;
        photonView = GetComponent<PhotonView>();
    }
    
    public override void takeDamage(int damage, object sender = null)
    {
        if (currentHealth != 0)
        {
            handleSteal(damage, sender);
        }
        
        base.takeDamage(damage, sender);
    }
    
    public void handleSteal(int damage, object sender = null)
    {
        Unit attacker = sender as Unit;
        if (attacker.ownerPlayer)
        {
            attacker.ownerPlayer.creditsLeft.value += damage * 5;
            photonView.RPC("deductCredits", RpcTarget.Others, damage * 5);
        }
    }

    [PunRPC]
    public void deductCredits(int toDeduct)
    {
        if (ownerPlayer)
        {
            if (ownerPlayer.creditsLeft.value > 0)
            {
                ownerPlayer.creditsLeft.value = Mathf.Clamp(ownerPlayer.creditsLeft.value - toDeduct, 0, ownerPlayer.creditsLeft.value - toDeduct);
            }
        }
    }
}
