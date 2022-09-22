using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PythonProxies;

[PythonClass("World", true)]
public class world : MonoBehaviour
{

    [PythonMethod]
    public List<CharacterHandlerProxy> getEnemies()
    {
        List<CharacterHandlerProxy> characters = new List<CharacterHandlerProxy>();
        foreach (Character c in GameObject.FindObjectsOfType<Character>())
        {
            if (c.ownerPlayer.playerID == GameObject.FindObjectOfType<PlayerManager>().playerID)
            {
                characters.Add(c.CreateProxy() as CharacterHandlerProxy);
            }
        }

        return characters;
    }

    [PythonMethod]
    public List<OreDepositProxy> getOreDeposits()
    {
        List<OreDepositProxy> ores = new List<OreDepositProxy>();
        foreach (OreDeposit c in GameObject.FindObjectsOfType<OreDeposit>())
        {
            ores.Add(c.CreateProxy() as OreDepositProxy);
        }

        return ores;
    }

    [PythonMethod]
    public EntityProxy getEnemyTower()
    {
        return GameObject.FindObjectOfType<Tower>().CreateProxy() as EntityProxy;
    }

    [PythonMethod]
    public List<object> getAllEntities()
    {
        List<object> entities = new List<object>();
        foreach (Entity c in GameObject.FindObjectsOfType<Entity>())
        {
            entities.Add(c.CreateProxy());
        }

        return entities;
    }



    [PythonMethod]
    public List<EntityProxy> getAllEntitiesOfType(String typeName)
    {
        List<EntityProxy> entities = new List<EntityProxy>();
        Type type = Type.GetType(typeName);

        if (type == null)
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "findClosestEntityOfType", playerId = gameObject.name }, new Error("Incorrect type name."));
            return null;
        }

        foreach (Entity c in GameObject.FindObjectsOfType(type))
        {
            entities.Add(c.CreateProxy() as EntityProxy);
        }

        return entities;
    }
}
