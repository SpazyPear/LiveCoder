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

    public int lane;

    protected Unit owningUnit;

    public GameObject moduleObj;

    public int currentHealth;

    public float height;

    public HealthBar healthBar;

    public EventHandler<float> OnHealthChanged;

    public abstract string displayName();

    protected virtual void Start()
    {
        currentHealth = moduleData.maxHealth;
    }

    protected virtual void Update() { }

    protected virtual void Awake()
    {
        //healthBar.setupHealthBar(transform, OnHealthChanged);
        owningUnit = GetComponent<Unit>();
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
            if (gameObject != null)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, timer / 1);
                timer += Time.deltaTime;
                await Task.Yield();
            }
        }

        owningUnit.removeModule(lane);

    }

    public void takeDamage(int damage, object sender = null)
    {
        OnHealthChanged?.Invoke(this, (currentHealth - damage) / moduleData.maxHealth);

        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            return;
        }

        die(sender);
    }
}
