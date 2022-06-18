using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int GridWidth = 5;
    public int GridBreadth = 5;
    public int GridHeight = 2;
    public float TileSize = 10;
    public GameObject[] tilePrefabs;
    public Transform GridParent;

    // Start is called before the first frame update
    void Awake()
    {
        State.onLevelLoad += generateGrid;
    }

    public void HighlightGrid(int x, int y)
    {

    }


    public void generateGrid(object sender, EventArgs e)
    {
        State.GridContents = new Tile[GridWidth, GridBreadth];

        for (int height = 0; height < GridWidth; height++)
        {
            for (int width = 0; width < GridBreadth; width++)
            {
                GameObject tile = Instantiate(tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Length)], new Vector3(height * TileSize, 0, width * TileSize), Quaternion.identity, GridParent);
                tile.transform.localScale = new Vector3(TileSize, 2, TileSize);
                State.GridContents[height, width] = new Tile(tile);
            }
        }

    }

    public float[,] CostMap()
    {
        float[,] costMap = new float[GridWidth, GridBreadth];

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridBreadth; y++)
            {
                costMap[x, y] = 1f;
            }
        }

        return costMap;

    }


}


/*
 * 
 * function OnStart()
	e = getEnemies()
	
	if len(e) > 0 then
		closest = e[1]
		
		for i,v in e do
			if dist(current.position, v) < dist(current.position, closest) then
				closest = v
			end
		end
	end
end


function OnStep()
	current.MoveToCharacter(closest)
end
*/

/*
 * function OnStart()
	e = getEnemies()
	
	if len(e) > 0 then
		closest = e[1]
		
		for i,v in e do
			if dist(current.position, v) < dist(current.position, closest) then
				closest = v
			end
		end
	end
end


function OnStep()
	current.MoveToCharacter(closest)
end*/