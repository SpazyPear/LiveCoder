/*using MoonSharp.Interpreter;*/
using System;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerHandlerInterface
{
    void MovePlayer(Vector2Int moveDirection);
}

public class PlayerHandler : MonoBehaviour
{
    public Unit selectedPlayer;

    public List<Unit> multipleSelectedPlayers;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKeyDown(KeyCode.LeftShift) == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
          
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null && hit.transform.GetComponentInParent<Unit>() != null)
                {
                    print($"Player {hit.transform.parent} has been selected");


                    if (hit.transform.GetComponentInParent<Unit>().ownerPlayer && hit.transform.GetComponentInParent<Unit>().ownerPlayer.isLocalPlayer)
                    {
                        this.multipleSelectedPlayers.Clear();
                        this.selectedPlayer = hit.transform.GetComponentInParent<Unit>();

                        PythonInterpreter.instance.OpenEditor(this.selectedPlayer.codeContext);
                        //GameObject.FindObjectOfType<CodeExecutor>().ClearOtherContexts();
                        //GameObject.FindObjectOfType<CodeExecutor>().OpenEditor(this.selectedPlayer.codeContext);
                    }
                }
            }
        }

        if (selectedPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
            {
                print("AA");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

                    //print(hit.transform.parent.name);
                    if (hit.transform != null && hit.transform.GetComponentInChildren<Unit>() != null)
                    {
                        
                        if (hit.transform.GetComponentInChildren<Unit>().ownerPlayer.playerID == GameManager.activePlayer.playerID)
                        {
                            this.multipleSelectedPlayers.Add(hit.transform.GetComponentInChildren<Unit>());
                            //GameObject.FindObjectOfType<CodeExecutor>().AddEditingContext(hit.transform.GetComponentInChildren<Unit>().codeContext);
                        }
                    }
                }
            }
        }
    }

    public List<Unit> getEnemies()
    {
        List<Unit> characters = new List<Unit>();
        foreach (Unit c in GameObject.FindObjectsOfType<Unit>())
        {
            if (!c.ownerPlayer.isLocalPlayer)
            {
                characters.Add(c);
            }
        }

        return characters;
    }
    public List<OreDeposit> getOreDeposits()
    {
        List<OreDeposit> ores = new List<OreDeposit>();
        foreach (OreDeposit c in GameObject.FindObjectsOfType<OreDeposit>())
        {
             ores.Add(c);            
        }

        return ores;
    }

    public Tower getEnemyTower()
    {
        return GameObject.FindObjectOfType<Tower>();
    }

    public List<Unit> getAllUnits(Unit from)
    {
        List<Unit> entities = new List<Unit>();
        foreach (Unit c in GameObject.FindObjectsOfType<Unit>())
        {
            if (c != from)
                entities.Add(c);
        }

        return entities;
    }

    public Unit findClosest(Unit sender, string typeName, bool enemyOnly)
    {
        Unit closest = null;
        float minDistance = Mathf.Infinity;
        Type type = Type.GetType(typeName);

        if (type == null)
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "findClosestEntityOfType", playerId = gameObject.name }, new Error("Incorrect type name."));
            return null;
        }

        foreach (Unit c in GameObject.FindObjectsOfType(type))
        {
            if (c == sender || (enemyOnly && c.ownerPlayer && c.ownerPlayer.playerID == sender.ownerPlayer.playerID))
                continue;

            if (Vector2Int.Distance(c.gridPos, sender.gridPos) < minDistance)
            {
                closest = c;
                minDistance = Vector2Int.Distance(c.gridPos, sender.gridPos);
            }
        }

        return closest;
    }

    public List<Unit> getAllEntitiesOfType(String typeName)
    {
        List<Unit> entities = new List<Unit>();
        Type type = Type.GetType(typeName);

        if (type == null)
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "findClosestEntityOfType", playerId = gameObject.name }, new Error("Incorrect type name."));
            return null;
        }

        foreach (Unit c in GameObject.FindObjectsOfType(type))
        {
           entities.Add(c);
        }

        return entities;
    }

 
}