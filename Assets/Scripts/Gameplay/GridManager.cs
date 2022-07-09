using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public int GridWidth = 5;
    public int GridBreadth = 5;
    public int GridHeight = 2;
    public float TileSize = 10;
    public GameObject[] tilePrefabs;
    public Transform GridParent;
    public static Material[,] tileMaterials;


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
        tileMaterials = new Material[GridWidth, GridBreadth];
        for (int height = 0; height < GridWidth; height++)
        {
            for (int width = 0; width < GridBreadth; width++)
            {
                GameObject tile = Instantiate(tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Length)], new Vector3(height * TileSize, 0, width * TileSize), Quaternion.identity, GridParent);
                tile.transform.localScale = new Vector3(TileSize, GridHeight, TileSize);

                tileMaterials[height, width] = tile.GetComponentInChildren<MeshRenderer>().material;

                State.GridContents[height, width] = new Tile(tile, new Vector2Int(height, width));


                if (tile.transform.GetComponentInChildren<GridTile>() != null)
                {
                    tile.transform.GetComponentInChildren<GridTile>().gridTile = State.GridContents[height, width];
                }
            }
        }
    }

    List<Material> findTilesToToggle(int PlayerID)
    {
        List<Material> materials = new List<Material>();
        for (int x = 0; x < State.GridContents.GetLength(0); x++)
        {
            for (int y = 0; y < State.GridContents.GetLength(1); y++)
            {
                //if ()
            }
        }
        return materials;
    }

    async void hideTiles()
    {

        float progress = 1;
        while (progress > 0) {
            progress -= Time.deltaTime / 3f;
            for (int x = 0; x < tileMaterials.GetLength(0); x++)
            {
                for (int y = 0; y < tileMaterials.GetLength(1); y++)
                {
                    tileMaterials[x, y].SetFloat("_IsVisible", progress);
                }
            }
            await Task.Yield();
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