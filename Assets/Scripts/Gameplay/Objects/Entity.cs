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

    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual void die(Character sender = null)
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        codeContext.character = this;
      
        GameObject.FindObjectOfType<CodeExecutor>().codeContexts.Add(codeContext);
    }


    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual void takeDamage(int damage, Character sender = null)
    {
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            return;
        }
            
        die(sender);
    }

}
