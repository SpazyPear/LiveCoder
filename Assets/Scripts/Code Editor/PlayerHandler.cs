using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public interface PlayerHandlerInterface
{
    void MovePlayer(Vector2Int moveDirection);
}

public class PlayerHandler : MonoBehaviour
{
    public Character selectedPlayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            print("Clicking");
            if (Physics.Raycast(ray, out hit))
            {
                print(hit.transform.parent.name);
                if (hit.transform.parent != null && hit.transform.parent.GetComponent<Character>() != null)
                {
                    print($"Player {hit.transform.parent.name} has been selected");


                    if (hit.transform.parent.GetComponent<Character>().ownerPlayer == 0)
                    {
                        this.selectedPlayer = hit.transform.parent.GetComponent<Character>();

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
            if (c.ownerPlayer == 1)
            {
                characters.Add(c);
            }
        }

        return characters;
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

    public void MovePlayer(Vector2Int move) { if (target.ownerPlayer == 0) target.moveUnit(move.x, move.y); }
    public void SetPath(List<Vector2Int> path) { if (target.ownerPlayer == 0) target.SetPath(path); }
    public bool PathCompleted() { if (target.ownerPlayer == 0) return target.PathCompleted(); else return false; }
    public void MoveOnPathNext() { if (target.ownerPlayer == 0) target.MoveOnPathNext(); }
    public List<Character> getNearbyUnits() { if (target.ownerPlayer == 0) return target.checkForInRangeEnemies(); else return new List<Character>(); }
    public void Attack(Character character) { if (target.ownerPlayer == 0) target.attack(character); }
    public void MoveToCharacter (Character character) { if (target.ownerPlayer == 0) target.MoveToCharacter(character); }

}
