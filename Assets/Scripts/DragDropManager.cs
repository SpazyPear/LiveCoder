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
    public List<DragAndDropType> attackChoices = new List<DragAndDropType>();
    public List<DragAndDropType> defenceChoices = new List<DragAndDropType>();
    public Transform dragDropTransform;
    bool attacking;
    public List<DragAndDropType> activeChoices { get { return attacking ? attackChoices : defenceChoices; } }

    private void Start() { 
        updateChoices(false);
    }


    public void updateChoices(bool attacking)
    {
        clearPanel();
        this.attacking = attacking;
        foreach (DragAndDropType type in activeChoices)
        {
            Transform t = Transform.Instantiate(dragDropTransform, this.transform);
            t.GetComponent<DragAndDropUnit>().canvas = canvas;
            t.GetComponent<DragAndDropUnit>().entity = type.nonCharacterEntityPrefab;
            t.GetComponent<DragAndDropUnit>().unitType = type.characterType;
            t.GetComponent<DragAndDropUnit>().isCharacter = type.isCharacter;
        }
    }

    void clearPanel()
    {
        for (int x = transform.childCount - 1; x >=0; x--)
        {
            Destroy(transform.GetChild(x).gameObject);  
        }
    }

}
