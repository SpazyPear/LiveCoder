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
    public WFCGenerator wfcGenerator;
    public bool showInput;

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
        List<GameObject>[,] tiles = wfcGenerator.generateGrid(GridBreadth, GridWidth);
        State.GridContents = new Tile[GridWidth, GridBreadth];

        for (int height = 0; height < GridWidth; height++)
        {
            for (int width = 0; width < GridBreadth; width++)
            {
                try
                {
                    GameObject tile = Instantiate(tiles[height, width][0], new Vector3(height * TileSize, 0, width * TileSize), Quaternion.identity, GridParent);
                    tile.transform.localScale = new Vector3(TileSize, 2, TileSize);
                    State.GridContents[height, width] = new Tile(tile);
                }
                catch (ArgumentOutOfRangeException) { continue; }

            }
        }

        /*if (showInput)
        {
            for (int height = 0; height < Mathf.Sqrt(wfcGenerator.input.Length); height++)
            {
                for (int width = 0; width < Mathf.Sqrt(wfcGenerator.input.Length); width++)
                {
                    
                }
            }
        }*/

    }

    public float[,] CostMap()
    {
        float[,] costMap = new float[GridWidth, GridHeight];

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                if (State.GridContents[x, y].Entity != null) costMap[x, y] = 0f;
                else costMap[x, y] = 1f;
            }
        }

        return costMap;

    }


}
