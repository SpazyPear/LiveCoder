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
                    this.selectedPlayer = hit.transform.parent.GetComponent<Character>();
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

    public void MovePlayer(Vector2Int move) { target.moveUnit(move.x, move.y); }
    public void SetPath(List<Vector2Int> path) { target.SetPath(path); }
    public bool PathCompleted() { return target.PathCompleted(); }
    public void MoveOnPathNext() { target.MoveOnPathNext(); }
    public List<Character> getNearbyUnits() { return target.checkForInRangeEnemies(); }
    public void Attack(Character character) { target.attack(character); }
    public void MoveToCharacter (Character character) { target.MoveToCharacter(character); }

}
