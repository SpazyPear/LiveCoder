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

    static Dictionary<int, GameObject> tileObjects = new Dictionary<int, GameObject>();
    static readonly char[] trimCharacters = { '(', 'C', 'l', 'o', 'n', 'e', ')' };

    public static GameObject[,] objectArrayToGrid(GameObject[] gameObjects)
    {
        GameObject[,] grid = new GameObject[5, 5]; //change
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Debug.Log(i / 5);
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
                int ID = tileObjects.FirstOrDefault(pair => Regex.Match(pair.Value.name.TrimEnd(trimCharacters), @"^[^\d]+").Value == Regex.Match(tileMap[x, y].name.TrimEnd(trimCharacters), @"^[^\d]+").Value).Key;
                if (ID == 0 && x + y == 0)
                {
                    tileObjects.Add(highestTileID++, tileMap[x, y]);
                    ID = highestTileID;
                }
                tileMapArray[x, y] = ID;
            }
        }
        return tileMapArray;
    }

    public static List<Rule> identifyRules(int[,] inputTiles)
    {
        List<Rule> rules = new List<Rule>();
        for (int x = 0; x < inputTiles.GetLength(0); x++)
        {
            for (int y = 0; y < inputTiles.GetLength(1); y++)
            {
                // Loop Neighbours
                for (int i = -x; i <= x; i++)
                {
                    for (int j = -y; j <= y; j++)
                    {
                        if (i > 0 && j > 0 && i < inputTiles.GetLength(0) && j < inputTiles.GetLength(1) && (i == 0 || j == 0)) // no diaganols
                        {
                            AddRule(ref rules, new Rule(inputTiles[x, y], inputTiles[i + x, j + y], new Vector2Int(i, j)));
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
            rules.Add(newRule);
        }

    }


}
