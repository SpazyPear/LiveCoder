using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : ControlledMonoBehavour
{
    public int currentHealth;
    public Vector2Int gridPos;
    public int ownerPlayer;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public void die()
    {
        Destroy(gameObject);
    }


    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual float takeDamage(int damage)
    {
        if (currentHealth - damage > 0)
            return currentHealth -= damage;

        return -1;
    }

}
