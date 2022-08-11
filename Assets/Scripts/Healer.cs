using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerProxy
{
    Healer target;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public HealerProxy(Healer p)
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
            playHealFX();
            List<Character> inRangeCharacters = checkForInRangeEntities<Character>();
            foreach (Character c in inRangeCharacters)
            {
                try
                {
                    c.currentHealth += healerData.healRate;
                }
                catch (System.Exception e) { }
            }
        }
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

    public void EMP()
    {
        if (!isDisabled)
        {
            List<Entity> inRange = checkForInRangeEntities<Entity>();
            foreach (Entity c in inRange)
            {
                if (c.ownerPlayer != ownerPlayer)
                {
                    c.OnEMPDisable(healerData.EMPStrength);
                }
            }
        }
    }

}
