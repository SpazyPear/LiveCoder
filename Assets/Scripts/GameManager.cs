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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
