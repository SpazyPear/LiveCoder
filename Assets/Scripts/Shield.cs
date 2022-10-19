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

public class Shield : ControlledMonoBehavour
{
    public BindableValue<float> shieldHealth;
    public float maxShieldHealth;
    public float shieldRegenRate;
    public ShieldState shieldState = ShieldState.InActive;
    public ShieldState prevShieldState;
    public bool shieldRegenerating;
    public Material shieldMat;
    public Color shieldFullColour;
    public Color shieldDeadColour;

    public void setDefaults(float maxShieldHealth, float shieldRegenRate)
    {
        this.maxShieldHealth = maxShieldHealth;
        this.shieldRegenRate = shieldRegenRate;
        shieldHealth = new BindableValue<float>(x => shieldMat.SetColor("_TintColor", Color.Lerp(shieldDeadColour, shieldFullColour, x / maxShieldHealth)));
        shieldHealth.value = maxShieldHealth;
    }

    public override void OnStep()
    {
        shieldRegen();
    }

    public void takeShieldDamage(float damage)
    {
        shieldHealth.value -= damage;
        if (shieldHealth.value <= 0)
        {
            gameObject.SetActive(false);
            shieldState = ShieldState.InActive;
            shieldRegen();
        }
    }

    void shieldRegen() //probably doesnt work like that with the tokens
    {
        if (shieldRegenerating && shieldHealth.value <= maxShieldHealth && shieldState == ShieldState.InActive)
        {
            shieldHealth.value = Mathf.Clamp(shieldRegenRate + shieldHealth.value, 0, maxShieldHealth);
        }
        else
        {
            shieldRegenerating = false;
        }
    }
}
