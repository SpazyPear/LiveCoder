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

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void generateGrid(object sender, EventArgs e)
    {
        State.GridContents = new Tile[GridWidth, GridBreadth];

        for (int height = 0; height < GridBreadth; height++)
        {
            for (int width = 0; width < GridWidth; width++)
            {
                GameObject tile = Instantiate(tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Length)], new Vector3(height * TileSize, 0, width * TileSize), Quaternion.identity, GridParent);
                tile.transform.localScale = new Vector3(TileSize, 2, TileSize);
                State.GridContents[height, width] = new Tile(tile);
            }
        }

        foreach (Character playerManager in GameObject.FindObjectsOfType(typeof(Character)))
        {
            playerManager.initializePlayer(playerManager.GetType().Name + "ScriptableObject");
        }
    }


}
