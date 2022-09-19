using ExitGames.Client.Photon.StructWrapping;
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
                if (hit.transform != null && hit.transform.GetComponentInParent<Entity>() != null)
                {
                    print($"Player {hit.transform.parent} has been selected");


                    if (hit.transform.GetComponentInParent<Entity>().ownerPlayer.playerID == GameManager.activePlayer.playerID)
                    {
                        this.multipleSelectedPlayers.Clear();
                        this.selectedPlayer = hit.transform.GetComponentInParent<Entity>();


                        GameObject.FindObjectOfType<CodeExecutor>().ClearOtherContexts();
                        GameObject.FindObjectOfType<CodeExecutor>().OpenEditor(this.selectedPlayer.codeContext);
                    }
                }
            }
        }

        if (selectedPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
            {
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

    public Entity findClosest(Entity sender, string typeName, bool enemyOnly)
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

}


public class CharacterHandlerProxy : EntityProxy
{
    Character target;

    [MoonSharpHidden]
    public CharacterHandlerProxy(Character p) : base(p)
    {
        this.target = p;
    }

    public bool isDead() { return (target == null || target.currentHealth <= 0); }
  
    public float attackRange
    {
        get
        {
            return target.characterData.range;
        }
    }

    public void MovePlayer(Vector2Int move) { target.replicatedMove(move.x, move.y); }
    public void SetPath(List<Vector2Int> path) { target.SetPath(path); }
    public bool PathCompleted() { return target.PathCompleted(); }
    public void MoveOnPathNext() { target.MoveOnPathNext(); }

    public bool IsInRange(Entity entity) { return target.checkForInRangeEntities("Entity", true, true).Contains(entity); } // make good

    public void Attack(int x, int y) { target.attack(x, y); }

    public void MoveToCharacter (Character character) { target.MoveToCharacter(character); }
    public void MoveToPos(Vector2Int pos) { target.MoveTo(pos); }

    public void MoveToEntity(Entity entity) { target.MoveTo(entity.gridPos); }

    public void CheckForInRangeEntities(string typeName, bool friendlies, bool enemies) { target.checkForInRangeEntities(typeName, friendlies, enemies); }

}

public class GiantHandlerProxy : CharacterHandlerProxy
{
    Giant target;
    
    [MoonSharpHidden]
    public GiantHandlerProxy(Giant p) : base(p)
    {
        this.target = p;
    }

    public void DeployShield(bool raised) { target.positionShield(raised); }
}

public class HealerHandlerProxy : CharacterHandlerProxy
{
    Healer target;

    [MoonSharpHidden]
    public HealerHandlerProxy(Healer p) : base(p)
    {
        this.target = p;
    }

    public void heal() { target.heal(); }

    public void emp() { target.EMP(); }
}
