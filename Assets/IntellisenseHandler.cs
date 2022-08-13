using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using TMPro.EditorUtilities;

public class IntellisenseHandler : MonoBehaviour
{
    public Transform intellisenseParent;
    public RectTransform intellisenseParentRectTransform;
    public Transform intellisenseSpawned;
    public TextMeshProUGUI intellisenseInputField;
    public TMP_InputField inputField;

    public Vector3 offset;

    string giveList(List<string> strings)
    {
        string b = "(";
        
        for (int i = 0; i < strings.Count; i++)
        {
            b += strings[i];

            if (i < strings.Count - 1) b += ", ";
        }

        b += ")";
        return b;
    }


    int lastSuggestionCount = 0;

    private void Update()
    {
        if (lastSuggestions.Count > 0 && lastSuggestions.Count != lastSuggestionCount)
        {
            intellisenseParent.gameObject.SetActive(true);
            
        }
        else
        {
            intellisenseParent.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            intellisenseParent.gameObject.SetActive(false);
            lastSuggestionCount = lastSuggestions.Count;
        }

        Vector3 bottomLeft = intellisenseInputField.transform.TransformPoint(intellisenseInputField.textInfo.characterInfo[inputField.caretPosition].bottomLeft);


        Vector3 buttonSpacePos = intellisenseParent.transform.parent.InverseTransformPoint(bottomLeft);
        intellisenseParent.transform.localPosition = new Vector3(buttonSpacePos.x, buttonSpacePos.y, 0) + offset;

    }

    List<CodeSuggestion> lastSuggestions = new List<CodeSuggestion>();

    public void LoadNewSuggestions (List<CodeSuggestion> suggestions)
    {

        lastSuggestions = suggestions;

        for (int i = 0; i < intellisenseParent.childCount; i++)
        {
            GameObject.Destroy(intellisenseParent.GetChild(i).gameObject);
        }


        foreach (CodeSuggestion suggestion in suggestions)
        {
            Transform t = Transform.Instantiate(intellisenseSpawned, intellisenseParent);
            TextMeshProUGUI textObject = t.GetComponent<TextMeshProUGUI>();

            string typeString = suggestion.type == CodeSuggestionType.Function ? "[Method] " : "[Property] ";
            string nameString = suggestion.name;
            string returnType = " : " + suggestion.returnType;


            string paramString = (suggestion.type == CodeSuggestionType.Function && suggestion.parameters != null) ? giveList(suggestion.parameters.Select((a) => "" + a.name + " : " + a.type + "").ToList()) : "";


            string finalString = typeString + nameString + " " + paramString + returnType;

            textObject.text = finalString;
        }

    }

}
