using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Photon.Pun;

public class GiantProxy : CharacterHandlerProxy
{
    Giant target;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public GiantProxy(Giant p) : base(p)
    {
        this.target = p;
    }
}



public class Giant : Character
{
    public Transform shieldDownPoint;
    public Transform shieldUpPoint;

    public Shield shield;
 
    public GiantData giantData
    {
       get { return characterData as GiantData; }
    }
    // Start is called before the first frame update

    public override void Start()
    {
        base.Start();
        shield.setDefaults(giantData.maxShieldHealth, giantData.shieldRegenRate);
    }
    
    public void positionShield(bool raised)
    {
        if (shield.shieldHealth > 0)
        {
            photonView.RPC("replicatedPositionShield", RpcTarget.AllViaServer, raised);
        }
    }

    [PunRPC]
    public IEnumerator replicatedPositionShield(bool raised)
    {
        shield.gameObject.SetActive(true);
        Transform transform = raised ? shieldUpPoint : shieldDownPoint;
        shield.transform.position = transform.position;
        shield.transform.rotation = transform.rotation;
        yield return null;
    }

    public override void takeDamage(int damage, object sender = null)
    {
        if (sender is ProjectileBehaviour)
        {
            ProjectileBehaviour projectile = sender as ProjectileBehaviour;
            if (shield.shieldState == ShieldState.Lowered && projectile.lane == ProjectileLane.Flat || shield.shieldState == ShieldState.Raised && projectile.lane == ProjectileLane.Above)
            {
                shield.takeShieldDamage(damage);
                return;
            }
        }
        base.takeDamage(damage, sender);
    }

    public override void OnEMPDisable(float strength)
    {
        base.OnEMPDisable(strength);
        if (shield.shieldState != ShieldState.InActive)
        {
            shield.prevShieldState = shield.shieldState;
            shield.shieldState = ShieldState.InActive;
            shield.gameObject.SetActive(false);
        }
    }

    public override void EMPRecover()
    {
        base.EMPRecover();
        if (shield.prevShieldState != ShieldState.InActive)
        {
            positionShield(shield.prevShieldState == ShieldState.Raised ? true : false);
        }
    }

}
