using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public PlayerManager playerManager;
    // Start is called before the first frame update
    void Start()
    {
        State.onLevelLoad += gridManager.generateGrid;
        State.onLevelLoad += playerManager.initializePlayer;
        State.initializeLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
