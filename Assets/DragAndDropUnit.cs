using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropUnit : MonoBehaviour
{
    public Canvas canvas;
    public string unitType;
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
            print($"Drag started of {unitType}");
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

        print(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            print("Transform " + hit.transform.name);
            if (hit.transform != null && hit.transform.GetComponent<GridTile>() != null)
            {
                print("Spawning player");
                Vector2Int pos = hit.transform.GetComponent<GridTile>().gridTile.gridPosition;
                print(pos);
                GameObject.FindObjectOfType<UnitSpawner>().spawnUnit(unitType,pos);       
            }
        }
        
        print($"Drag ended of {unitType} at {Input.mousePosition}");

    }

}
