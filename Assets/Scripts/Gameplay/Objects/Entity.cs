using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using Photon.Pun;


using PythonProxies;
using System.Linq;

public class PythonProxyObject {

    public string pythonClassName;
}

public class Entity : PlaceableObject, IDamageable
{

    public EntityData entityData;
    public HealthBar healthBar;
    public int currentHealth;
    public int maxHealth = 5;
    EventHandler<float> OnHealthChanged;
   
    public virtual async void die(object sender = null)
    {
        float timer = 0;

        while (timer < 1)
        {
            if (gameObject != null)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, timer / 1);
                timer += Time.deltaTime;
                await Task.Yield();
            }
        }
        Destroy(gameObject);
    }
    
    public virtual void Start()
    {
        //GameManager.unitInstances.Add(gameObject.GetInstanceID(), this);
    }

    protected override void Awake()
    {
        base.Awake();
        //healthBarObj = Instantiate(Resources.Load("UI/HealthBar") as GameObject, GameObject.FindObjectOfType<Canvas>().transform).GetComponent<RectTransform>();
        currentHealth = maxHealth;
    }
    


    public virtual void takeDamage(int damage, object sender = null)
    {
        OnHealthChanged?.Invoke(this, (currentHealth - damage) / maxHealth);
        
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            return;
        }
            
        die(sender);
    }




    public PythonProxyObject CreateProxy()
    {
        return new EntityProxy(this);
    }
}
