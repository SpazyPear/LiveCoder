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
    public int selfDestructRange = 2;
    public int selfDestructDamage = 2;
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

    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual void selfDestruct()
    {
        for (int x = -selfDestructRange; x <= selfDestructRange; x++)
        {
            for (int y = -selfDestructRange; y <= selfDestructRange; y++)
            {
                try
                {
                    if (State.GridContents[gridPos.x + x, gridPos.y + y].Entity && State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren<Character>() != gameObject.GetComponentInChildren<Character>())
                    {

                        State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren<Entity>().takeDamage(2);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        Camera.main.gameObject.GetComponent<CameraShake>().shakeCamera();
        Instantiate(Resources.Load("PS/PS_Explosion_Rocket") as GameObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
