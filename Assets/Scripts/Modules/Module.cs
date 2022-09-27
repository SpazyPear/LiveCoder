using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PythonProxies;


public abstract class Module : MonoBehaviour, IDamageable
{
    public ModuleData moduleData;

    public int lane;

    protected Unit owningUnit;

    public GameObject moduleObj;

    public int currentHealth;
    
    public abstract string displayName();

    protected virtual void Start() { }

    protected virtual void Update() { }


    protected virtual void Awake()
    {
        currentHealth = moduleData.maxHealth;
        owningUnit = GetComponent<Unit>();
        AddPrefab();
    }

    public abstract object CreateProxy();

    public virtual void OnEMPDisable(float strength) { }

    public virtual void EMPRecover() { }

    protected abstract void AddPrefab();

    public void die(object sender = null)
    {
        throw new System.NotImplementedException();
    }

    public void takeDamage(int damage, object sender = null)
    {
        throw new System.NotImplementedException();
    }
}
