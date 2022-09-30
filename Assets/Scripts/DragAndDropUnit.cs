using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public enum ObjectType
{
    Unit,
    Module,
    Entity
}

public class DragAndDropUnit : MonoBehaviour
{
    public Canvas canvas;
    public new string name;
    public ObjectType objectType;
    public Transform dragType; // Duplicates on drag


    Transform _temp;
    bool dragging = false;
    public void OnDragStart()
    {
        if (!dragging)
        {
            _temp = Instantiate(dragType, dragType.position, dragType.rotation, this.transform);
            Color curColor = _temp.GetComponent<Image>().color;
            curColor.a = 0.2f;

            _temp.GetComponent<Image>().color = curColor;

            dragging = true;
            StartCoroutine("DuringDrag");
            //print($"Drag started of {unitType}");
        }
    }


    IEnumerator DuringDrag()
    {
        while (dragging)
        {
            if (_temp != null)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
                _temp.position = canvas.transform.TransformPoint(pos);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnDragEnd()
    {
        if (_temp != null) GameObject.Destroy(_temp.gameObject);
        dragging = false;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, ~7))
        {
            if (hit.transform != null && hit.transform.GetComponent<GridTile>() != null)
            {
                print(hit.transform.gameObject.name);
                Vector2Int pos = hit.transform.GetComponent<GridTile>().gridTile.gridPosition;
                print(pos + "position");

                if ((pos.y > GridManager.GridContents.GetLength(1) / 2 && GameManager.activePlayer.isLeftSide) || (pos.y < GridManager.GridContents.GetLength(1) / 2 && !GameManager.activePlayer.isLeftSide))
                    SpawnObject(pos);
            }
        }
        
    }

    void SpawnObject(Vector2Int pos)
    {
        switch (objectType)
        {
            case ObjectType.Unit:
                GameManager.activePlayer.spawnUnit(name, pos);
                break;
            case ObjectType.Module:
                Unit unit = GridManager.GridContents[pos.x, pos.y].OccupyingObject as Unit;
                if (unit && unit.AmOwner)
                    unit.addModule(name);
                else
                {
                    Unit emptyUnit = GameManager.activePlayer.spawnUnit("Empty", pos);
                    emptyUnit.addModule(name);
                }
                break;
            case ObjectType.Entity:
                GameManager.activePlayer.spawnEntity(name, pos);
                break;
        }
    }

}
