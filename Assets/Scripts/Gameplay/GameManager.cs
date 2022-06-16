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
        State.gameManager = this;
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
        GameObject instance = spawnOnGrid(towerPrefab, new Vector2Int(State.GridContents.GetLength(0) / 2, 0));
        instance.AddComponent<Tower>().belongingPlayer = findPlayer(0);
        GameObject instance2 = spawnOnGrid(towerPrefab, new Vector2Int(State.GridContents.GetLength(0) / 2, State.GridContents.GetLength(1) - 1));
        instance.AddComponent<Tower>().belongingPlayer = findPlayer(2);
    }

    public static GameObject spawnOnGrid(GameObject obj, Vector2Int pos)
    {
        if (State.validMovePosition(pos))
        {
            GameObject instance = Instantiate(obj, State.gridToWorldPos(pos), Quaternion.identity);
            float y = obj.transform.GetChild(0).GetComponent<Renderer>().localBounds.extents.y * instance.transform.GetChild(0).localScale.y; // needs to be recursive search for the first renderer
            instance.transform.position += new Vector3(0, y, 0);
            State.GridContents[pos.x, pos.y].Entity = instance;
            return instance;
        }
        return null;
    }

    public static PlayerManager findPlayer(int playerID)
    {
        foreach (PlayerManager player in GameObject.FindObjectsOfType<PlayerManager>())
        {
            if (player.playerID == playerID)
                return player;
        }
        return null;
    }
}
