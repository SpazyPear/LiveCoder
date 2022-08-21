using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class GiantProxy : CharacterHandlerProxy
{
    Giant target;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public GiantProxy(Giant p) : base(p)
    {
        this.target = p;
    }
}

public enum ShieldState
{
    InActive,
    Raised,
    Lowered
}

public class Giant : Character
{
    public GameObject shield;
    public Transform shieldDownPoint;
    public Transform shieldUpPoint;
    public ShieldState shieldState = ShieldState.InActive;
    public ShieldState prevShieldState;
    public float shieldHealth;
 
    public GiantData giantData
    {
       get { return characterData as GiantData; }
    }
    // Start is called before the first frame update
    
    public void deployShield(bool raised)
    {
        if (shieldHealth > 0)
        {
            shield.SetActive(true);
            Transform transform = raised ? shieldUpPoint : shieldDownPoint;
            shield.transform.position = transform.position;
            shield.transform.rotation = transform.rotation;
        }
    }
    
    void takeShieldDamage(float damage)
    {
        shieldHealth -= damage;
        if (shieldHealth <= 0)
        {
            shield.SetActive(false);
            shieldState = ShieldState.InActive;
            shieldRegen();
        }
    }

    async void shieldRegen()
    {
        while (shieldHealth <= giantData.maxShieldHealth && shieldState == ShieldState.InActive)
        {
            await Task.Yield();
            shieldHealth += giantData.shieldRegenRate * Time.deltaTime;
        }
        shieldHealth = giantData.maxShieldHealth;
    }

    public override void takeDamage(int damage, object sender = null)
    {
        if (sender is ProjectileBehaviour)
        {
            ProjectileBehaviour projectile = sender as ProjectileBehaviour;
            if (shieldState == ShieldState.Lowered && projectile.lane == ProjectileLane.Flat || shieldState == ShieldState.Raised && projectile.lane == ProjectileLane.Above)
            {
                takeShieldDamage(damage);
                return;
            }
        }

        base.takeDamage(damage, sender);

    }

    public override void OnEMPDisable(float strength)
    {
        base.OnEMPDisable(strength);
        if (shieldState != ShieldState.InActive)
        {
            prevShieldState = shieldState;
            shieldState = ShieldState.InActive;
            shield.SetActive(false);
        }
        
    }

    public override void EMPRecover()
    {
        base.EMPRecover();
        if (prevShieldState != ShieldState.InActive)
        {
            deployShield(prevShieldState == ShieldState.Raised ? true : false);
        }
    }

}
