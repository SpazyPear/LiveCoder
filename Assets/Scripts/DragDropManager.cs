using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DragAndDropType
{
    public string name; 
    public ObjectType objectType;
}

public class DragDropManager : MonoBehaviour
{
    public Canvas canvas;
    public List<DragAndDropType> unitChoices = new List<DragAndDropType>();
    public Transform dragDropTransform;
    public UIManager uiManager;
    public PlayerManager playerManager;
    PlaceableObject draggingEntity;

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
            t.GetComponent<DragAndDropUnit>().name = type.name;
            t.GetComponent<DragAndDropUnit>().objectType = type.objectType;
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
            PlaceableObject hitObject = Physics.Raycast(ray, out hit) ? hit.collider.gameObject.GetComponentInParent<PlaceableObject>() : null;
            
            if (hitObject is Unit && (hitObject as Unit).executing) return;
            
            if (hitObject && hitObject.ownerPlayer && hitObject.ownerPlayer.isLocalPlayer)
            {
                draggingEntity = hitObject;
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
            else if (!hit.transform && draggingEntity is Unit)
            {
                playerManager.deleteUnit(draggingEntity as Unit);
            }

            draggingEntity = null;
        }
    }
}
