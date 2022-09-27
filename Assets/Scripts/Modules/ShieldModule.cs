using Photon.Pun;
using PythonProxies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldModule : Module
{

    public Transform shieldDownPoint;
    public Transform shieldUpPoint;

    Shield shield;
    public ShieldData shieldData { get { return moduleData as ShieldData; } private set { } }

    protected override void Awake()
    {
        shieldData = Resources.Load("ModuleConfig/ShieldScriptableObject") as ShieldData;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        shield = GetComponentInChildren<Shield>();
        shield.transform.position = transform.position;
        shield.transform.rotation = transform.rotation;
        shield.setDefaults(shieldData.maxShieldHealth, shieldData.shieldRegenRate);

    }

    public void positionShield(bool raised)
    {
        if (shield.shieldHealth > 0)
        {
            owningUnit.photonView.RPC("replicatedPositionShield", RpcTarget.AllViaServer, raised);
        }
    }

    public void lowerShield()
    {
        owningUnit.photonView.RPC("replicatedLowerShield", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void replicatedLowerShield()
    {
        shield.gameObject.SetActive(false);
    }

    [PunRPC]
    public void replicatedPositionShield(bool raised)
    {
        shield.gameObject.SetActive(true);
        Transform transform = raised ? shieldUpPoint : shieldDownPoint;
        shield.transform.position = transform.position;
        shield.transform.rotation = transform.rotation;
    }

    public void takeShieldDamage(int damage, object sender = null)
    {
        if (sender is ProjectileBehaviour)
        {
            ProjectileBehaviour projectile = sender as ProjectileBehaviour;
            if (shield.shieldState == ShieldState.Lowered && projectile.projectileLane == ProjectileLane.Flat || shield.shieldState == ShieldState.Raised && projectile.projectileLane == ProjectileLane.Above)
            {
                shield.takeShieldDamage(damage);
                return;
            }
        }
    }

    public override object CreateProxy()
    {
        return new ShieldModuleProxy(this);
    }

    public override void OnEMPDisable(float strength)
    {
        if (shield.shieldState != ShieldState.InActive)
        {
            shield.prevShieldState = shield.shieldState;
            shield.shieldState = ShieldState.InActive;
            shield.gameObject.SetActive(false);
        }
    }

    public override void EMPRecover()
    {
        if (shield.prevShieldState != ShieldState.InActive)
        {
            positionShield(shield.prevShieldState == ShieldState.Raised ? true : false);
        }
    }

    public override string displayName()
    {
        return "shieldModule";
    }

    protected override void AddPrefab()
    {
        moduleObj = GridManager.InstantiateObject("Prefabs/Modules/ShieldModule", transform.position, Quaternion.identity);
        moduleObj.transform.SetParent(transform);
    }
}
