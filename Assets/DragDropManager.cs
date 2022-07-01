using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DragAndDropType
{
    public bool isCharacter;
    public string characterType; // If Character
    public GameObject nonCharacterEntityPrefab; // If Not Character
}

public class DragDropManager : MonoBehaviour
{
    public Canvas canvas;
    public List<DragAndDropType> dragAndDropTypes = new List<DragAndDropType>();
    public Transform dragDropTransform;

    private void Start() { 
        foreach (DragAndDropType type in dragAndDropTypes)
        {
            Transform t = Transform.Instantiate(dragDropTransform, this.transform);
            t.GetComponent<DragAndDropUnit>().canvas = canvas;
            t.GetComponent<DragAndDropUnit>().entity = type.nonCharacterEntityPrefab;
            t.GetComponent<DragAndDropUnit>().unitType = type.characterType;
            t.GetComponent<DragAndDropUnit>().isCharacter = type.isCharacter;

        }
    }

}
