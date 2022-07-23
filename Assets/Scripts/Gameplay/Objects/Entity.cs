using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using System;

public class EntityProxy
{
    Entity target;

    [MoonSharpHidden]
    public EntityProxy(Entity p)
    {
        this.target = p;
    }

    public string id => target.ID.ToString();
    public int health => target.currentHealth;
}

public class Entity : ControlledMonoBehavour
{
    public int currentHealth;
    public Vector2Int gridPos;
    public PlayerManager ownerPlayer;
    public int cost;
    public CodeContext codeContext;
    public int ID;
    public bool isDisabled;
    public EntityData entityData;
   

    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual void die(object sender = null)
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        codeContext.character = this;
      
        GameObject.FindObjectOfType<CodeExecutor>().codeContexts.Add(codeContext);
    }
    

    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual void takeDamage(int damage, object sender = null)
    {
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            return;
        }
            
        die(sender);
    }

    public async virtual void EMPRecover(float strength)
    {
        float timer = entityData.empResistance * strength;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            await Task.Yield();
        }
    }

    public virtual void OnEMPDisable(float strength)
    {
        isDisabled = true;
        EMPRecover(strength);
    }

}
