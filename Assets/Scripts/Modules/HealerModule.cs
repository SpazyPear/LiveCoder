using Photon.Pun;
using PythonProxies;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealerModule : Module
{
    public override object CreateProxy()
    {
        return new HealerModuleProxy(this);
    }

    public ParticleSystem healRing;
    public ParticleSystem healCrosses;
    public HealerData healerData { get { return moduleData as HealerData; } private set { moduleData = value; } }

    protected override void Awake()
    {
        base.Awake();
    }

    public void heal()
    {
        GameManager.CallRPC(this, "replicatedHeal", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void replicatedHeal()
    {
        playHealFX();
        List<PlaceableObject> inRangeEntities = GridManager.checkForInRangeEntities(owningUnit, healerData.range, true, false);
        foreach (Unit c in inRangeEntities)
        {
            if (c == null) continue;
            try
            {
                c.attachedModules.ForEach(x => x.currentHealth.value += healerData.healRate);
            }
            catch (System.Exception e) { }
        }
    }

    void playHealFX()
    {
        ParticleSystem.MainModule main = healRing.main;
        main.startSizeX = healerData.range * 2;
        main.startSizeZ = healerData.range * 2;
        healRing.Play();

        ParticleSystem.ShapeModule shape = healCrosses.shape;
        shape.scale = new Vector3(healerData.range * 8, healerData.range * 8, 1);
        healCrosses.Play();
    }
    public override string displayName()
    {
        return "healerModule";
    }


}

