using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

using PythonProxies;
using System;

public class Healer : Character
{
    public override object CreateProxy()
    {
        return new HealerHandlerProxy(this);
    }

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
            int[] inRange = GridManager.checkForInRangeEntities(this, healerData.EMPRange, false, true).Select(x => x.viewID).ToArray();
            photonView.RPC("replicatedEMP", RpcTarget.AllViaServer, inRange);
        }
    }


    [PunRPC]
    public void replicatedEMP(int[] inRange)
    {

        foreach (int c in inRange)
        {

            if (PhotonView.Find(c).GetComponent<Entity>().ownerPlayer != ownerPlayer)
            {
                PhotonView.Find(c).GetComponent<Entity>().OnEMPDisable(healerData.EMPStrength);
            }
        }
    }

}
