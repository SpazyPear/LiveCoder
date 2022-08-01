using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class IntellisenseHandler : MonoBehaviour
{
    public Transform intellisenseParent;
    public Transform intellisenseSpawned;

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

    public void LoadNewSuggestions (List<CodeSuggestion> suggestions)
    {
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
