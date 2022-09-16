using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerHandlerInterface
{
    void MovePlayer(Vector2Int moveDirection);
}

public class PlayerHandler : MonoBehaviour
{
    public Entity selectedPlayer;

    public List<Entity> multipleSelectedPlayers;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKeyDown(KeyCode.LeftShift) == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
          
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null && hit.transform.GetComponentInChildren<Entity>() != null)
                {
                    print($"Player {hit.transform.parent} has been selected");


                    if (hit.transform.GetComponentInChildren<Entity>().ownerPlayer.playerID == GameManager.activePlayer.playerID)
                    {
                        this.multipleSelectedPlayers.Clear();
                        this.selectedPlayer = hit.transform.GetComponentInChildren<Entity>();

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
                    if (hit.transform != null && hit.transform.GetComponentInChildren<Entity>() != null)
                    {
                        
                        if (hit.transform.GetComponentInChildren<Entity>().ownerPlayer.playerID == GameManager.activePlayer.playerID)
                        {
                            this.multipleSelectedPlayers.Add(hit.transform.GetComponentInChildren<Entity>());
                            GameObject.FindObjectOfType<CodeExecutor>().AddEditingContext(hit.transform.GetComponentInChildren<Entity>().codeContext);
                        }
                    }
                }
            }
        }
    }

    public List<Character> getEnemies()
    {
        List<Character> characters = new List<Character>();
        foreach (Character c in GameObject.FindObjectsOfType<Character>())
        {
            if (c.ownerPlayer.playerID == 1)
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

    public List<Entity> getAllEntities(Character from)
    {
        List<Entity> entities = new List<Entity>();
        foreach (Entity c in GameObject.FindObjectsOfType<Entity>())
        {
            if (c != from)
                entities.Add(c);
        }

        return entities;
    }

    public Entity findClosestEntityOfType(Entity sender, string typeName)
    {
        Entity closest = null;
        float minDistance = Mathf.Infinity;
        Type type = Type.GetType(typeName);

        if (type == null)
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "findClosestEntityOfType", playerId = gameObject.name }, new Error("Incorrect type name."));
            return null;
        }

        foreach (Entity c in GameObject.FindObjectsOfType(type))
        {
            if (c == sender)
                continue;

            if (Vector2Int.Distance(c.gridPos, sender.gridPos) < minDistance)
            {
                closest = c;
                minDistance = Vector2Int.Distance(c.gridPos, sender.gridPos);
            }
        }

        return closest;
    }

    public List<Entity> getAllEntitiesOfType(String typeName)
    {
        List<Entity> entities = new List<Entity>();
        Type type = Type.GetType(typeName);

        if (type == null)
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "findClosestEntityOfType", playerId = gameObject.name }, new Error("Incorrect type name."));
            return null;
        }

        foreach (Entity c in GameObject.FindObjectsOfType(type))
        {
           entities.Add(c);
        }

        return entities;
    }

}