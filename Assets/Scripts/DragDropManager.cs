using System.Collections;
using System.Collections.Generic;
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

    private void Start() { 
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

}
