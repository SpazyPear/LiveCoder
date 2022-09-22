using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

public class HealerProxy : CharacterHandlerProxy
{
    Healer target;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public HealerProxy(Healer p) : base(p)
    {
        this.target = p;
    }
}

public class Healer : Character
{

    public ParticleSystem healRing;
    public ParticleSystem healCrosses;

    public HealerData healerData
    {
        get { return characterData as HealerData; }
    }

    public void heal()
    {
        if (!isDisabled)
        {
            photonView.RPC("replicatedHeal", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public IEnumerator replicatedHeal() 
    {
        playHealFX();
        List<Entity> inRangeEntities = checkForInRangeEntities("Entity", true, false);
        foreach (Character c in inRangeEntities)
        {
            try
            {
                c.currentHealth += healerData.healRate;
            }
            catch (System.Exception e) { }
        }
        yield return null;
    }

    void playHealFX()
    {
        ParticleSystem.MainModule main = healRing.main;
        main.startSizeX = healerData.healRange * 2;
        main.startSizeZ = healerData.healRange * 2;
        healRing.Play();

        ParticleSystem.ShapeModule shape = healCrosses.shape;
        shape.scale = new Vector3(healerData.healRange * 8, healerData.healRange * 8, 1);
        healCrosses.Play();
    }

    public void OnEMP()
    {
        if (!isDisabled)
        {
            List<Entity> inRange = checkForInRangeEntities("Entity", false, true);
            photonView.RPC("replicatedEMP", RpcTarget.AllViaServer, inRange.Select(x => x as object).ToArray());
        }
    }


    [PunRPC]
    public IEnumerator replicatedEMP(List<Entity> inRange)
    {
        foreach (Entity c in inRange)
        {
            if (c.ownerPlayer != ownerPlayer)
            {
                c.OnEMPDisable(healerData.EMPStrength);
            }
        }
        yield return null;
    }

}
