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



}


public class CharacterHandlerProxy 
{
    Character target;

    [MoonSharpHidden]
    public CharacterHandlerProxy(Character p)
    {
        this.target = p;
    }

    public void MovePlayer(Vector2Int move) { target.movePlayer(move.x, move.y); }
    public void SetPath(List<Vector2Int> path) { target.SetPath(path); }
    public bool PathCompleted() { return target.PathCompleted(); }
    public void MoveOnPathNext() { target.MoveOnPathNext(); }
    public List<Character> getNearbyUnits() { return target.checkForInRangeEnemies(); }
    public void Attack(Character character) { target.attack(character); }

}
