using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WFCGenerator : MonoBehaviour
{
    public GameObject[] input;
    List<Rule> rules;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    List<GameObject> getUniqueTiles(GameObject[] input)
    {
        List<GameObject> uniqueTiles = new List<GameObject>();
        foreach (GameObject tile in input)
        {
            if (!uniqueTiles.Contains(tile))
                uniqueTiles.Add(tile);
        }
        return uniqueTiles;
    }

    List<GameObject>[,] fillGridEntropy(int width, int height)
    {
        List<GameObject> uniqueTiles = getUniqueTiles(input);
        List<GameObject>[,] grid = new List<GameObject>[width, height];
        int maxEntropy = PatternIdentifier.tileObjects.Max(x => x.Key) + 1;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new List<GameObject>(uniqueTiles);
            }
        }
        return grid;
    }

    void getLowestEntropyTile(List<GameObject>[,] grid, ref int x, ref int y)
    {
        int lowestEntropy = int.MaxValue;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j].Count < lowestEntropy && grid[i, j].Count > 1)
                {
                    lowestEntropy = grid[i, j].Count;
                    x = i;
                    y = j;
                }
            }
        }
    }

    List<GameObject> assignRandomState(List<GameObject> tile)
    {
        List<GameObject> newTileEntropy = new List<GameObject>(tile);
        int index = Random.Range(0, newTileEntropy.Count);
        for (int i = 0; i < newTileEntropy.Count; i++)
        {
            if (i != index)
                newTileEntropy.RemoveAt(i);
        }
        return newTileEntropy;
    }


    public List<GameObject>[,] generateGrid(int width, int height)
    {
        GameObject[,] objectGrid = PatternIdentifier.objectArrayToGrid(input);
        int[,] intGrid = PatternIdentifier.tileMapToArray(objectGrid);
        rules = PatternIdentifier.identifyRules(intGrid);
        List<GameObject>[,] generatedGrid = fillGridEntropy(width, height);
        int lowestX = 0;
        int lowestY = 0;
        while (!isFullyCollapsed(generatedGrid))
        {
            getLowestEntropyTile(generatedGrid, ref lowestX, ref lowestY);
            generatedGrid[lowestX, lowestY] = assignRandomState(generatedGrid[lowestX, lowestY]);
            // Loop Neighbours
            for (int i = lowestX - 1; i <= lowestX + 1; i++)
            {
                for (int j = lowestY - 1; j <= lowestY + 1; j++)
                {
                    if (i >= 0 && j >= 0 && i < generatedGrid.GetLength(0) && j < generatedGrid.GetLength(1) && (i == lowestX || j == lowestY) && !(i == lowestX && j == lowestY)) // no diaganols
                    {
                        for (int e = generatedGrid[i, j].Count - 1; e >= 0; e--)
                        {
                            List<Rule> matchingRules = rules.Where(x => x.Tile == PatternIdentifier.tileObjects.First(x => x.Value == generatedGrid[lowestX, lowestY][0]).Key && x.Adjacent == PatternIdentifier.tileObjects.First(x => x.Value == generatedGrid[i, j][e]).Key && x.Direction == new Vector2Int(i - lowestX, j - lowestY)).ToList();
                            if (matchingRules.Count == 0)
                            {
                                generatedGrid[i, j].RemoveAt(e);
                            }
                        }
                    }
                }
            }
        }
        return generatedGrid;
    }

    bool isFullyCollapsed(List<GameObject>[,] grid)
    {
        foreach (List<GameObject> list in grid)
        {
            if (list.Count > 1)
                return false;
        }
        return true;
    }

    
}
