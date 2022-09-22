using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DragAndDropType
{
    public string characterType; // If Character
}

public class DragDropManager : MonoBehaviour
{
    public Canvas canvas;
    public List<DragAndDropType> unitChoices = new List<DragAndDropType>();
    public Transform dragDropTransform;
    public UIManager uiManager;
    public PlayerManager playerManager;
    Entity draggingEntity;

    private void Start() 
    { 
        updateChoices();
    }

    public void updateChoices()
    {
        clearPanel();
        foreach (DragAndDropType type in unitChoices)
        {
            Transform t = Transform.Instantiate(dragDropTransform, transform);
            t.GetComponent<DragAndDropUnit>().canvas = canvas;
            t.GetComponent<DragAndDropUnit>().unitType = type.characterType;
        }
    }

    void clearPanel()
    {
        for (int x = transform.childCount - 1; x >= 0; x--)
        {
            Destroy(transform.GetChild(x).gameObject);  
        }
    }

    void Update()
    {
        dragMoveExistingUnit();    
    }

    void dragMoveExistingUnit()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Entity hitEntity = Physics.Raycast(ray, out hit) ? hit.collider.gameObject.GetComponentInParent<Entity>() : null;
            if (hitEntity && !hitEntity.executing && hitEntity.ownerPlayer && hitEntity.ownerPlayer.isLocalPlayer)
            {
                draggingEntity = hitEntity;
            }
        }
        if (Input.GetMouseButtonUp(0) && draggingEntity)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            GridTile hitTile = Physics.Raycast(ray, out hit) ? hit.collider.gameObject.GetComponentInParent<GridTile>() : null;

            if (hitTile)
            {
                GameManager.photonView.RPC("placeOnGrid", Photon.Pun.RpcTarget.All, draggingEntity.photonView.ViewID, hitTile.gridTile.gridPosition.x, hitTile.gridTile.gridPosition.y, playerManager.isLeftSide);
            }
            else if (!hit.transform)
            {
                playerManager.deleteUnit(draggingEntity);
            }

            draggingEntity = null;
        }
    }
}
