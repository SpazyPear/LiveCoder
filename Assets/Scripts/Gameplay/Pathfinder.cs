using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class PathfindingTarget
{
    public Vector2Int target;
}

public class Pathfinder : ControlledMonoBehavour
{
    public GridManager grid;
    private PathfindingTarget target;

    private PathFind.Grid pathGrid;

    public override void OnStart()
    {
        
        pathGrid = new PathFind.Grid(grid.GridWidth, grid.GridHeight, grid.CostMap());
    }

    public override void OnPostStep()
    {
        pathGrid = new PathFind.Grid(grid.GridWidth, grid.GridHeight, grid.CostMap());
    }

    private void Awake()
    {
        
    }

    // Returns a move set of how to get to this path
    public List<Vector2Int> FindPath(Vector2Int from, Vector2Int to)
    {
        PathFind.Point _from = new PathFind.Point(from.x, from.y);
        PathFind.Point _to = new PathFind.Point(to.x, to.y);

        List<PathFind.Point> path = PathFind.Pathfinding.FindPath(pathGrid, _from, _to);

        List<Vector2Int> moves = new List<Vector2Int>();
        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0)
            {
                moves.Add(new Vector2Int(path[0].x - from.x, path[0].y - from.y));
                continue;
            }
            Vector2Int next = new Vector2Int(path[i].x, path[i].y);
            Vector2Int cur = new Vector2Int(path[i-1].x, path[i-1].y);

            Vector2Int move = new Vector2Int(next.x - cur.x, next.y - cur.y);
            moves.Add(move);
        }

        moves.Add(new Vector2Int(to.x - path[path.Count-1].x, to.y - path[path.Count-1].y));

        foreach (Vector2Int moveset in moves) print($"C# added : {moveset.x},{moveset.y}");

        return moves;
    }

    public List<Vector2Int> FindPath (Character from, Character to)
    {
        return FindPath(from.gridPos, to.gridPos);
    }

}

/*
 * 
function OnStart()
	path = FindPath(vec2(0,0), vec2(0,2))
	i = 1
	max = len(path)
end

function OnStep()
	if i <= max then
		currentPlayer.MovePlayer(path[i])
		i = i + 1
	end	
end*/