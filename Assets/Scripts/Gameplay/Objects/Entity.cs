using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using MoonSharp.Interpreter;

public class EntityProxy
{
    Entity target;

    [MoonSharpHidden]
    public EntityProxy(Entity p)
    {
        this.target = p;
    }



}

public class Entity : ControlledMonoBehavour
{
    public int currentHealth;
    public Vector2Int gridPos;
    public int ownerPlayer;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual void die(Character sender = null)
    {
        Destroy(gameObject);
    }


    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual void takeDamage(Character sender, int damage)
    {
        if (currentHealth - damage > 0)
            currentHealth -= damage;

        die(sender);
    }

}
