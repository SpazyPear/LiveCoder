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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
          
            if (Physics.Raycast(ray, out hit))
            {

                //print(hit.transform.parent.name);
                if (hit.transform != null && hit.transform.GetComponentInChildren<Entity>() != null)
                {
                    print($"Player {hit.transform.parent} has been selected");


                    if (hit.transform.GetComponentInChildren<Entity>().ownerPlayer.playerID == GameManager.activePlayer.playerID)
                    {
                        this.selectedPlayer = hit.transform.GetComponentInChildren<Entity>();

                        GameObject.FindObjectOfType<CodeExecutor>().OpenEditor(this.selectedPlayer.codeContext);
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

        print(closest);
        return closest;
    }

}


public class CharacterHandlerProxy 
{
    Character target;

    [MoonSharpHidden]
    public CharacterHandlerProxy(Character p)
    {
        this.target = p;
    }

    public bool isDead() { return (target == null || target.currentHealth <= 0); }
    public Vector2Int position
    {
        get
        {
            return target.gridPos;
        }
    }

    public double attackRange
    {
        get
        {
            return target.characterData.range;
        }
    }

    public void MovePlayer(Vector2Int move) { target.moveUnit(move.x, move.y); }
    public void SetPath(List<Vector2Int> path) { target.SetPath(path); }
    public bool PathCompleted() { return target.PathCompleted(); }
    public void MoveOnPathNext() { target.MoveOnPathNext(); }

    public bool IsInRange(Entity entity) { return target.checkForInRangeEntities<Entity>().Contains(entity); } // make good

    public void Attack(Entity entity) { target.attack(entity); }

    public void CollectOre(OreDeposit ore) { target.attack(ore); }
    public void MoveToCharacter (Character character) { target.MoveToCharacter(character); }
    public void MoveToPos(Vector2Int pos) { target.MoveTo(pos); }

    public void MoveToEntity(Entity entity) { target.MoveTo(entity.gridPos); }



}
