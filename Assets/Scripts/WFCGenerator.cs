using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WFCGenerator : MonoBehaviour
{
    public GameObject[,] input;
    public Transform inputContainer;
    List<Rule> rules;

    // Start is called before the first frame update
    void Awake()
    {
        fillInput();
    }

    void fillInput()
    {
        input = new GameObject[(int)Mathf.Sqrt(inputContainer.childCount), (int)Mathf.Sqrt(inputContainer.childCount)];
        int width = (int)Mathf.Sqrt(inputContainer.childCount);
        for (int i = 0; i < inputContainer.childCount; i++)
            input[i / width, i % width] = findObjAtPos(inputContainer.transform, i / width, i % width);
    }

    GameObject findObjAtPos(Transform parent, int x, int y)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).transform.localPosition == new Vector3(x, 0, y))
                return parent.GetChild(i).gameObject;
        }
        return null;
    }

    List<GameObject>[,] fillGridEntropy(int width, int height)
    {
        List<GameObject> uniqueTiles = PatternIdentifier.tileObjects.Values.ToList().OrderByDescending(x => PatternIdentifier.objectWeights[x]).ToList();
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

    List<GameObject> collapseState(List<GameObject> tile)
    {
        List<GameObject> newTileEntropy = new List<GameObject>(tile);
        float result = Random.Range(0, 1f);
        int index = 0;
        while (index < newTileEntropy.Count && result > 0)
        {
            if (result - PatternIdentifier.objectWeights[newTileEntropy.ElementAt(index)] > 0)
            {

                result -= PatternIdentifier.objectWeights[newTileEntropy.ElementAt(index)];
                index++;
          
            }
            else
                break;
        }

        for (int i = tile.Count - 1; i >= 0; i--)
        {
            if (i != index)
            {
                newTileEntropy.RemoveAt(i);
            }
        }
        return newTileEntropy;
    }


    public List<GameObject>[,] generateGrid(int width, int height)
    {
        GameObject[,] objectGrid = input;
        int[,] intGrid = PatternIdentifier.tileMapToArray(objectGrid);
        rules = PatternIdentifier.identifyRules(intGrid);
        List<GameObject>[,] generatedGrid = fillGridEntropy(width, height);
        int lowestX = 0;
        int lowestY = 0;
        while (!isFullyCollapsed(generatedGrid))
        {
            getLowestEntropyTile(generatedGrid, ref lowestX, ref lowestY);
            generatedGrid[lowestX, lowestY] = collapseState(generatedGrid[lowestX, lowestY]);
            // Loop Neighbours
            for (int i = lowestX - 1; i <= lowestX + 1; i++)
            {
                for (int j = lowestY - 1; j <= lowestY + 1; j++)
                {
                    if (i >= 0 && j >= 0 && i < generatedGrid.GetLength(0) && j < generatedGrid.GetLength(1) && (i == lowestX || j == lowestY) && !(i == lowestX && j == lowestY)) // no diaganols
                    {
                        for (int e = generatedGrid[i, j].Count - 1; e >= 0; e--)
                        {
                            List<Rule> matchingRules = rules.Where(x => x.Tile == PatternIdentifier.tileObjects.First(x => x.Value.tag == generatedGrid[lowestX, lowestY][0].tag).Key && x.Adjacent == PatternIdentifier.tileObjects.First(x => x.Value.tag == generatedGrid[i, j][e].tag).Key && x.Direction == new Vector2Int(i - lowestX, j - lowestY)).ToList();
                            if (matchingRules.Count == 0 && generatedGrid[i, j].Count != 1)
                            {
                                //Debug.Log(generatedGrid[lowestX, lowestY][0].tag + " can't be next to " + generatedGrid[i, j][e].tag + " in direction " + new Vector2Int(i - lowestX, j - lowestY));
                                generatedGrid[i, j].RemoveAt(e);
                                if (generatedGrid[i, j].Count == 0)
                                    Debug.Log("now empty");
                                
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
