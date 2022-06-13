using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public interface PlayerHandlerInterface
{
    void MovePlayer(Vector2Int moveDirection);
}

public class PlayerHandler : MonoBehaviour, PlayerHandlerInterface
{
    Character selectedPlayer;

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


    public void MovePlayer(Vector2Int vector2)
    {
        if (selectedPlayer != null)
        {
            selectedPlayer.moveUnit(vector2.x, vector2.y);
            print($"New player position : {selectedPlayer.gridPos}");
        }
    }

    public void SetPath(List<Vector2Int> path)
    {
        if (selectedPlayer != null) selectedPlayer.SetPath(path);
    }

    public bool PathCompleted()
    {
        if (selectedPlayer != null) return selectedPlayer.PathCompleted();
        else return true;
    }

    public void MoveOnPathNext()
    {

        if (selectedPlayer != null) selectedPlayer.MoveOnPathNext();
    }

}


public class PlayerHandlerProxy : PlayerHandlerInterface
{
    PlayerHandler target;

    [MoonSharpHidden]
    public PlayerHandlerProxy(PlayerHandler p)
    {
        this.target = p;
    }

    public void MovePlayer(Vector2Int move) { target.MovePlayer(move); }
    public void SetPath(List<Vector2Int> path) { target.SetPath(path); }
    public bool PathCompleted() { return target.PathCompleted(); }
    public void MoveOnPathNext() { target.MoveOnPathNext(); }

}
