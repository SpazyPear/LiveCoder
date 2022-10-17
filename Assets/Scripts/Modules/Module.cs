using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PythonProxies;
using IronPython.Compiler.Ast;
using System;
using System.Threading.Tasks;

public abstract class Module : MonoBehaviour, IDamageable
{
    public ModuleData moduleData;

    [HideInInspector]
    public int lane;

    protected Unit owningUnit;

    [HideInInspector]
    public GameObject moduleObj;

    [HideInInspector]
    public int currentHealth;

    [HideInInspector]
    public float height;

    public HealthBar healthBar;

    public EventHandler<float> OnHealthChanged;

    public abstract string displayName();

    protected virtual void Start()
    {
        currentHealth = moduleData.maxHealth;
        healthBar = Instantiate(Resources.Load("UI/HealthBar") as GameObject, GameObject.Find("HealthBars").transform).GetComponent<HealthBar>(); 
        healthBar.setupHealthBar(moduleObj.transform);
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
        healthBar.OnHealthChanged(this, (currentHealth - damage) / (float)moduleData.maxHealth);

        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            return;
        }

        die(sender);
    }
}
