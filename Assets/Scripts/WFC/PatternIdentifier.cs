using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class Rule
{
    public int Tile;
    public int Adjacent;
    public Vector2Int Direction;
    public int weight;

    public Rule(int Tile, int Adjacent, Vector2Int direction)
    {
        this.Tile = Tile;
        this.Adjacent = Adjacent;
        this.Direction = direction;
    }
}

public static class PatternIdentifier
{

    public static Dictionary<int, GameObject> tileObjects = new Dictionary<int, GameObject>();
    public static Dictionary<GameObject, float> objectWeights = new Dictionary<GameObject, float>();

    public static GameObject[,] objectArrayToGrid(GameObject[] gameObjects)
    {
        GameObject[,] grid = new GameObject[5, 5]; //change
        for (int i = 0; i < gameObjects.Length; i++)
        {
            grid[i / 5, i % 5] = gameObjects[i];
        }
        return grid;
    }

    public static int[,] tileMapToArray(GameObject[,] tileMap)
    {
        int[,] tileMapArray = new int[tileMap.GetLength(0), tileMap.GetLength(1)];
        int highestTileID = -1;
        for (int x = 0; x < tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < tileMap.GetLength(1); y++)
            {
                int ID = containsTag(tileMap[x, y].tag);
                if (ID == -1)
                {
                    tileObjects.Add(++highestTileID, tileMap[x, y]);
                    objectWeights.Add(tileMap[x, y], 0);
                    ID = highestTileID;
                }
                tileMapArray[x, y] = ID;
                objectWeights[tileObjects[ID]]++;
            }
        }
        objectWeights[objectWeights.ElementAt(1).Key] += 3; 
        float sum = objectWeights.Sum(x => x.Value);
        List<GameObject> keys = objectWeights.Keys.ToList();
        for (int x = objectWeights.Count - 1; x >= 0; x--)  
            objectWeights[keys[x]] /= sum;
        return tileMapArray;
    }

    public static int containsTag(string tag)
    {
        foreach (KeyValuePair<int, GameObject> pair in tileObjects)
        {
            if (pair.Value.tag.Equals(tag))
                return pair.Key;
        }
        return -1;
    }

    public static List<Rule> identifyRules(int[,] inputTiles)
    {
        List<Rule> rules = new List<Rule>();
        for (int x = 0; x < inputTiles.GetLength(0); x++)
        {
            for (int y = 0; y < inputTiles.GetLength(1); y++)
            {
                // Loop Neighbours
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (i >= 0 && j >= 0 && i < inputTiles.GetLength(0) && j < inputTiles.GetLength(1) && (i == x || j == y) && !(i == x && j == y)) // no diaganals
                        {
                            AddRule(ref rules, new Rule(inputTiles[x, y], inputTiles[i, j], new Vector2Int(i - x, j - y)));
                        }
                    }
                }
                
            }
        }
        return rules;
    }

    static void AddRule (ref List<Rule> rules, Rule newRule)
    {

        foreach (Rule rule in rules)
        {
            if (rule.Tile == newRule.Tile && rule.Adjacent == newRule.Adjacent && rule.Direction == newRule.Direction)
            {
                rule.weight++;
                return;
            }
        }
        rules.Add(newRule);

    }


}
