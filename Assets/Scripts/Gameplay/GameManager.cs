using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject towerPrefab;
    // Start is called before the first frame update
    void Awake()
    {
        State.initializeLevel();
    }

    private async void Start()
    {
        await Task.Delay(1000);
        spawnTowers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnTowers()
    {
        Instantiate(towerPrefab, State.gridToWorldPos(new Vector2Int(State.GridContents.GetLength(0) / 2, 0)), Quaternion.identity);
    }
}
