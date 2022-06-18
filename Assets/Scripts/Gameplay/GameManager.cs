using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    // Start is called before the first frame update
    void Start()
    {
        //gridManager.generateGrid(null, null);
        //playerManager.initializePlayer(null, null);
        State.initializeLevel();
        //GameObject.FindObjectOfType<UnitSpawner>().spawnUnit("Soldier", new Vector2Int(2, 2)).attack(State.GridContents[0, 4].Entity.GetComponent<Character>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
