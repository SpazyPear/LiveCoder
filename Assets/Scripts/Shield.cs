using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum ShieldState
{
    InActive,
    Raised,
    Lowered
}

public class Shield : MonoBehaviour
{
    public float shieldHealth;
    public CancellationTokenSource shieldRegenTokenSource;
    public float maxShieldHealth;
    public float shieldRegenRate;
    public ShieldState shieldState = ShieldState.InActive;
    public ShieldState prevShieldState;

    public void setDefaults(float maxShieldHealth, float shieldRegenRate)
    {
        this.maxShieldHealth = maxShieldHealth;
        this.shieldRegenRate = shieldRegenRate;
        shieldHealth = maxShieldHealth;
    }

    public void takeShieldDamage(float damage)
    {
        shieldHealth -= damage;
        if (shieldHealth <= 0)
        {
            gameObject.SetActive(false);
            shieldState = ShieldState.InActive;
            shieldRegen();
        }
    }

    async void shieldRegen() //probably doesnt work like that with the tokens
    {
        if (shieldRegenTokenSource != null) shieldRegenTokenSource.Cancel();
        shieldRegenTokenSource = new CancellationTokenSource();
        while (!shieldRegenTokenSource.Token.IsCancellationRequested && shieldHealth <= maxShieldHealth && shieldState == ShieldState.InActive)
        {
            await Task.Yield();
            shieldHealth += shieldRegenRate * Time.deltaTime;
        }
        shieldHealth = maxShieldHealth;
    }
}
