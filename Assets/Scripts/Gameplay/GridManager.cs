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
    public static int GridHeight = 2;
    public float TileSize = 10;
    public GameObject[] tilePrefabs;
    public Transform GridParent;
    public static Tile[,] GridContents;
    public static Material[,] tileMaterials;
    public static PhotonView photonView;
    public static GridManager gridInstance;

    // Start is called before the first frame update
    void Awake()
    {
        gridInstance = this;
        photonView = GetComponent<PhotonView>();
        State.onLevelLoad += generateGrid;
    }

    public void HighlightGrid(int x, int y)
    {

    }

    public static PlaceableObject entityOnTile(int x, int y)
    {
        return GridContents[x, y].OccupyingObject ? GridContents[x, y].OccupyingObject.GetComponent<PlaceableObject>() : null;
    }

    public void generateGrid(object sender, EventArgs e)
    {
        GridContents = new Tile[GridWidth, GridBreadth];
        tileMaterials = new Material[GridWidth, GridBreadth];
        for (int height = 0; height < GridWidth; height++)
        {
            for (int width = 0; width < GridBreadth; width++)
            {
                GameObject tile = Instantiate(tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Length)], new Vector3(height * TileSize, 0, width * TileSize), Quaternion.identity, GridParent);
                tile.transform.localScale = new Vector3(TileSize, GridHeight, TileSize);

                tileMaterials[height, width] = tile.GetComponentInChildren<MeshRenderer>().material;

                GridContents[height, width] = new Tile(tile, new Vector2Int(height, width));

                if (tile.transform.GetComponentInChildren<GridTile>() != null)
                {
                    tile.transform.GetComponentInChildren<GridTile>().gridTile = GridContents[height, width];
                }
            }
        }
    }

    List<Material> findTilesToToggle(int PlayerID)
    {
        List<Material> materials = new List<Material>();
        for (int x = 0; x < GridContents.GetLength(0); x++)
        {
            for (int y = 0; y < GridContents.GetLength(1); y++)
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

        if ( GridContents != null)
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridBreadth; y++)
            {

                if (GridContents[x, y] != null && GridContents[x, y].OccupyingObject != null && GridContents[x,y].OccupyingObject.GetComponentInChildren<Wall>() != null)
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

    public static GameObject spawnOnGrid(string objName, Vector2Int pos, bool ignorePosClash = false, bool isLeftSide = false)
    {
        if (!validMovePosition(pos) && !ignorePosClash)
        {
            return null;
        }

        GameObject instance = InstantiateObject("Prefabs/" + objName, Vector3.zero, Quaternion.identity);
        PlaceableObject placeableObject = instance.GetComponentInChildren<PlaceableObject>();
        GameManager.objectInstances.Add(placeableObject.ViewID, placeableObject);
        GameManager.CallRPC(gridInstance, "placeOnGrid", RpcTarget.All, placeableObject.ViewID, pos.x, pos.y, isLeftSide);
        return instance;
    }

    [PunRPC]
    public void placeOnGrid(int ViewID, int x, int y, bool isLeftSide)
    {
        if ((y > GridContents.GetLength(1) / 2 && isLeftSide) || (y < GridContents.GetLength(1) / 2 && !isLeftSide))
        {
            GameObject obj = GetObjectInstance(ViewID).gameObject;
            Vector2Int pos = new Vector2Int(x, y);
            obj.transform.position = gridToWorldPos(pos);
            try
            {
                Transform mesh = obj.GetComponentInChildren<Renderer>().transform;
                float hieght = (GridHeight / 2);
                obj.transform.position += new Vector3(0, 1, 0);
            }
            catch { }
            GridContents[pos.x, pos.y].OccupyingObject = GetObjectInstance(ViewID);
            if (isLeftSide) obj.transform.rotation = Quaternion.Euler(0, 180, 0);

            obj.GetComponentInChildren<PlaceableObject>().gridPos = pos;
        }
    }

    public static bool isInRange(int range, PlaceableObject enemy)
    {
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                if (isPosInBounds(new Vector2Int(x, y)))
                {

                    if (entityOnTile(x, y) == enemy)
                        return true;
                }
            }
        }
        return false;
    }

    public static T findClosest<T>(T sender, bool ignoreOwn) where T : Unit
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

    public static Unit findClosestEnemy(Unit sender)
    {
        float min = Mathf.Infinity;
        Unit closest = null;
        foreach (Unit character in GameObject.FindObjectsOfType<Unit>())
        {
            if (Vector2Int.Distance(sender.gridPos, character.gridPos) < min && character.ownerPlayer != sender.ownerPlayer)
            {
                min = Vector2Int.Distance(sender.gridPos, character.gridPos);
                closest = character;
            }
        }
        return closest;
    }

    public static Unit getEntityAtPos(Vector2Int pos)
    {
        if (GridContents != null)
        {
            if (isPosInBounds(pos) && GridContents[pos.x, pos.y].OccupyingObject)
            {
                return GridContents[pos.x, pos.y].OccupyingObject.GetComponentInChildren<Unit>();
            }
        }
        return null;
    }

    public void spawnOreDeposits()
    {
        /*for (int x = 0; x < numOfOreToSpawn / 2; x++)
        {
            spawnOnGrid(orePrefab, new Vector2Int(UnityEngine.Random.Range(0, Mathf.CeilToInt(GridManager.GridContents.GetLength(0) / 2)), UnityEngine.Random.Range(0, Mathf.CeilToInt(GridManager.GridContents.GetLength(1)))));
        }

        for (int x = 0; x < numOfOreToSpawn / 2; x++)
        {
            spawnOnGrid(orePrefab, new Vector2Int(UnityEngine.Random.Range(Mathf.CeilToInt(GridManager.GridContents.GetLength(0) / 2), 0), UnityEngine.Random.Range(0, Mathf.CeilToInt(GridManager.GridContents.GetLength(1)))));
        }*/
    }

    public static bool isPosInBounds(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.y >= 0 && pos.x < GridContents.GetLength(0) && pos.y < GridContents.GetLength(1))
        {
            return true;
        }
        return false;
    }

    public static bool validMovePosition(Vector2Int pos)
    {
        if (isPosInBounds(pos) && GridContents[pos.x, pos.y].OccupyingObject == null)
        {
            return true;
        }
        return false;
    }

    public static Vector3 gridToWorldPos(Vector2Int gridPoint)
    {
        return GridContents[gridPoint.x, gridPoint.y].TileObject.transform.position;
    }
    public static List<Unit> checkForInRangeEntities(Unit sender, int range, bool friendlies, bool enemies)
    {
        List<Unit> foundEntities = new List<Unit>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                try
                {
                    if (GridManager.GridContents[sender.gridPos.x + x, sender.gridPos.y + y].OccupyingObject && GridManager.GridContents[sender.gridPos.x + x, sender.gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>() && GridManager.GridContents[sender.gridPos.x + x, sender.gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>() != sender)
                    {
                        if (!friendlies && (GridManager.GridContents[sender.gridPos.x + x, sender.gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>()).ownerPlayer == sender.ownerPlayer) continue;

                        if (!enemies && (GridManager.GridContents[sender.gridPos.x + x, sender.gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>()).ownerPlayer != sender.ownerPlayer) continue;

                        foundEntities.Add(GridManager.GridContents[sender.gridPos.x + x, sender.gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>());
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        return foundEntities;
    }

    public static void DestroyObject(GameObject obj)
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Destroy(obj);
        else
            Destroy(obj);
    }

    public static GameObject InstantiateObject(string obj, Vector3 pos, Quaternion rot)
    {
        if (PhotonNetwork.IsConnected)
            return PhotonNetwork.Instantiate(obj, pos, rot);
        else
            return Instantiate(Resources.Load(obj) as GameObject, pos, rot);
    }

    public static PlaceableObject GetObjectInstance(int id)
    {
        if (PhotonNetwork.IsConnected)
            return PhotonView.Find(id).GetComponent<PlaceableObject>();
        else
            return GameManager.objectInstances[id];
    }

    public static void DestroyComponent(Component comp)
    {

    }

    [PunRPC]
    static void ReplicatedDestroyComponent(int ViewID, string componentName)
    {

    }

}
