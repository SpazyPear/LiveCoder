using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using Photon.Pun;

public class GridManager : MonoBehaviour
{
    public int GridWidth = 5;
    public int GridBreadth = 5;
    public int GridHeight = 2;
    public float TileSize = 10;
    public GameObject[] tilePrefabs;
    public Transform GridParent;
    public static Material[,] tileMaterials;
    public static PhotonView photonView;

    // Start is called before the first frame update
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        State.onLevelLoad += generateGrid;
    }

    public void HighlightGrid(int x, int y)
    {

    }


    public void generateGrid(object sender, EventArgs e)
    {
        State.GridContents = new Tile[GridWidth, GridBreadth];
        tileMaterials = new Material[GridWidth, GridBreadth];
        for (int height = 0; height < GridWidth; height++)
        {
            for (int width = 0; width < GridBreadth; width++)
            {
                GameObject tile = Instantiate(tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Length)], new Vector3(height * TileSize, 0, width * TileSize), Quaternion.identity, GridParent);
                tile.transform.localScale = new Vector3(TileSize, GridHeight, TileSize);

                tileMaterials[height, width] = tile.GetComponentInChildren<MeshRenderer>().material;

                State.GridContents[height, width] = new Tile(tile, new Vector2Int(height, width));

                if (tile.transform.GetComponentInChildren<GridTile>() != null)
                {
                    tile.transform.GetComponentInChildren<GridTile>().gridTile = State.GridContents[height, width];
                }
            }
        }
    }

    List<Material> findTilesToToggle(int PlayerID)
    {
        List<Material> materials = new List<Material>();
        for (int x = 0; x < State.GridContents.GetLength(0); x++)
        {
            for (int y = 0; y < State.GridContents.GetLength(1); y++)
            {
                //if ()
            }
        }
        return materials;
    }

    async void hideTiles()
    {

        float progress = 1;
        while (progress > 0) {
            progress -= Time.deltaTime / 3f;
            for (int x = 0; x < tileMaterials.GetLength(0); x++)
            {
                for (int y = 0; y < tileMaterials.GetLength(1); y++)
                {
                    tileMaterials[x, y].SetFloat("_IsVisible", progress);
                }
            }
            await Task.Yield();
        }
    }

    public float[,] CostMap()
    {
        float[,] costMap = new float[GridWidth, GridBreadth];

        if ( State.GridContents != null)
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridBreadth; y++)
            {

                if (State.GridContents[x, y] != null && State.GridContents[x, y].Entity != null && State.GridContents[x,y].Entity.GetComponentInChildren<Wall>() != null)
                {
                    costMap[x, y] = 0f;
                }
                else
                {
                    costMap[x, y] = 1f;
                }
                
            }
        }

        return costMap;

    }

    public static GameObject spawnOnGrid(GameObject obj, Vector2Int pos, bool ignorePosClash = false, bool isLeftSide = false)
    {
        if (!State.validMovePosition(pos) && !ignorePosClash)
        {
            return null;
        }

        GameObject instance = PhotonNetwork.Instantiate("Prefabs/" + obj.name, Vector3.zero, Quaternion.identity);
        photonView.RPC("placeOnGrid", RpcTarget.AllViaServer, instance.GetComponentInChildren<Entity>().viewID, pos.x, pos.y, isLeftSide);

        return instance;
    }

    [PunRPC]
    public IEnumerator placeOnGrid(int viewID, int x, int y, bool isLeftSide)
    {
        GameObject obj = PhotonView.Find(viewID).gameObject;
        Vector2Int pos = new Vector2Int(x, y);
        obj.transform.position = State.gridToWorldPos(pos);
        Transform mesh = obj.GetComponentInChildren<Renderer>().transform;
        float hieght = (GridHeight / 2);
        obj.transform.position += new Vector3(0, 1, 0);
        State.GridContents[pos.x, pos.y].Entity = obj;
        if (isLeftSide) obj.transform.Rotate(0, 180, 0);

        obj.GetComponentInChildren<Entity>().gridPos = pos;
        yield return null;
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

    public static T findClosest<T>(T sender, bool ignoreOwn) where T : Entity
    {
        float min = Mathf.Infinity;
        T closest = null;
        foreach (T entity in GameObject.FindObjectsOfType<T>())
        {
            if (Vector2Int.Distance(sender.gridPos, entity.gridPos) < min)
            {
                if (ignoreOwn && entity.ownerPlayer != sender.ownerPlayer)
                    continue;

                min = Vector2Int.Distance(sender.gridPos, entity.gridPos);
                closest = entity;
            }
        }
        return closest;
    }

    public static List<T> checkForInRangeEntities<T>(Vector2Int pos, int range, Entity sender, bool ignoreOwnTeam) where T : Entity
    {
        List<T> foundEntities = new List<T>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                try
                {
                    if (State.GridContents[pos.x + x, pos.y + y].Entity && State.GridContents[pos.x + x, pos.y + y].Entity.GetComponentInChildren<Entity>() != sender)
                    {
                        if (ignoreOwnTeam && State.GridContents[pos.x + x, pos.y + y].Entity.GetComponentInChildren<Entity>().ownerPlayer == sender.ownerPlayer) continue;

                        foundEntities.Add(State.GridContents[pos.x + x, pos.y + y].Entity.GetComponentInChildren(typeof(T)) as T);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        return foundEntities;
    }

    public static Character findClosestEnemy(Character sender)
    {
        float min = Mathf.Infinity;
        Character closest = null;
        foreach (Character character in GameObject.FindObjectsOfType<Character>())
        {
            if (Vector2Int.Distance(sender.gridPos, character.gridPos) < min && character.ownerPlayer != sender.ownerPlayer)
            {
                min = Vector2Int.Distance(sender.gridPos, character.gridPos);
                closest = character;
            }
        }
        return closest;
    }

    public void spawnOreDeposits()
    {
        /*for (int x = 0; x < numOfOreToSpawn / 2; x++)
        {
            spawnOnGrid(orePrefab, new Vector2Int(UnityEngine.Random.Range(0, Mathf.CeilToInt(State.GridContents.GetLength(0) / 2)), UnityEngine.Random.Range(0, Mathf.CeilToInt(State.GridContents.GetLength(1)))));
        }

        for (int x = 0; x < numOfOreToSpawn / 2; x++)
        {
            spawnOnGrid(orePrefab, new Vector2Int(UnityEngine.Random.Range(Mathf.CeilToInt(State.GridContents.GetLength(0) / 2), 0), UnityEngine.Random.Range(0, Mathf.CeilToInt(State.GridContents.GetLength(1)))));
        }*/
    }

}


/*
 * 
 * function OnStart()
	e = getEnemies()
	
	if len(e) > 0 then
		closest = e[1]
		
		for i,v in e do
			if dist(current.position, v) < dist(current.position, closest) then
				closest = v
			end
		end
	end
end


function OnStep()
	current.MoveToCharacter(closest)
end
*/

/*
 * function OnStart()
	e = getEnemies()
	
	if len(e) > 0 then
		closest = e[1]
		
		for i,v in e do
			if dist(current.position, v) < dist(current.position, closest) then
				closest = v
			end
		end
	end
end


function OnStep()
	current.MoveToCharacter(closest)
end*/