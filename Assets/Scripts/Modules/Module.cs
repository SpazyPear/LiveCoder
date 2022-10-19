using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PythonProxies;
using IronPython.Compiler.Ast;
using System;
using System.Threading.Tasks;
using Photon.Pun;

public abstract class Module : MonoBehaviour, IDamageable
{
    public ModuleData moduleData;

    [HideInInspector]
    public int lane;

    protected Unit owningUnit;

    [HideInInspector]
    public GameObject moduleObj;

    [HideInInspector]
    public BindableValue<int> currentHealth;

    [HideInInspector]
    public float height;

    HealthBar healthBar;

    public abstract string displayName();

    protected virtual void Start()
    {
     
        Transform parent = GameObject.Find("HealthBars").transform;
        healthBar = Instantiate(Resources.Load("UI/HealthBar") as GameObject, parent.position, Quaternion.identity).GetComponent<HealthBar>();
        healthBar.transform.SetParent(parent.transform);

        currentHealth = new BindableValue<int>(x => healthBar.OnHealthChanged(x));
        healthBar.setupHealthBar(moduleObj.transform, moduleData.maxHealth);
        currentHealth.value = moduleData.maxHealth;
        owningUnit = GetComponent<Unit>();
        
    }

    protected virtual void Update() { }

    protected virtual void Awake()
    {
       
    }

    protected async virtual void Initialize()
    {
        while (moduleData == null) await Task.Yield();
    }

    public abstract object CreateProxy();

    public virtual void OnEMPDisable(float strength) { }

    public virtual void EMPRecover() { }

    public async void die(object sender = null)
    {
        float timer = 0;

        while (timer < 1)
        {
            if (moduleObj != null)
            {
                moduleObj.transform.localScale = Vector3.Lerp(moduleObj.transform.localScale, Vector3.zero, timer / 1);
                timer += Time.deltaTime;
                await Task.Yield();
            }
        }

        owningUnit.removeModule(lane);

    }

    public void takeDamage(int damage, object sender = null)
    {
        if (currentHealth.value - damage > 0)
        {
            currentHealth.value -= damage;
            return;
        }
        currentHealth.value = 0;
        die(sender);
    }
}
