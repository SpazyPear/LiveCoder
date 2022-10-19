using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ProjectileLane
{
    Flat,
    Above
}

public class ProjectileBehaviour : MonoBehaviour
{
    public int damage = 1;
    public PlayerManager ownerPlayer;
    public float aliveRange;
    public ProjectileLane projectileLane;
    public int lane;
    PhotonView photonView;

    private void Start()
    {
        Destroy(gameObject, aliveRange);
        photonView = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!PhotonNetwork.IsConnected || photonView.AmOwner)
        {
            Shield shield = collision.GetComponentInParent<Shield>();
            Unit character = collision.gameObject.GetComponentInParent<Unit>();
            if (shield == null && character && character.ownerPlayer != ownerPlayer)
            {
                GameManager.CallRPC(this, "DoDamage", RpcTarget.All, character.ViewID);
                GameManager.projectiles.Remove(photonView.ViewID);
                GridManager.DestroyOnNetwork(gameObject);
            }
            else if (shield)
            {
                GameManager.CallRPC(this, "DoShieldDamage", RpcTarget.All, character.ViewID);
                GameManager.projectiles.Remove(photonView.ViewID);
                GridManager.DestroyOnNetwork(gameObject);
            }
        }
    }

    [PunRPC]
    public void DoDamage(int ViewID)
    {
        GridManager.GetObjectInstance(ViewID).GetComponent<Unit>().takeDamage(lane, damage);
    }

    [PunRPC]
    public void DoShieldDamage(int ViewID)
    {
        GridManager.GetObjectInstance(ViewID).GetComponent<ShieldModule>().shield.takeShieldDamage(damage);
    }
}
