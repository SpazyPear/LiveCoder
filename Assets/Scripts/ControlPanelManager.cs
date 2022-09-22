using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelManager : MonoBehaviour
{
    public Transform parentTransform;

    public Transform sliderPrefab;
    public Transform checkboxPrefab;

    public TMPro.TMP_InputField nameInputField;

    public Dictionary<string, double> sliderValues = new Dictionary<string, double>();
    public Dictionary<string, bool> checkboxValues = new Dictionary<string, bool>();



    public void OnSliderAdd ()
    {
        Transform t = Instantiate(sliderPrefab, parentTransform);
        t.SetAsFirstSibling();
        t.Find("VarName").GetComponent<TMPro.TextMeshProUGUI>().text = nameInputField.text + " [0]";

        t.name = nameInputField.text;

        t.GetComponentInChildren<UnityEngine.UI.Slider>().onValueChanged.AddListener((f) =>
        {
            OnSliderValueChanged(t.name, f, t.Find("VarName").GetComponent<TMPro.TextMeshProUGUI>());
        });
        sliderValues.Add(nameInputField.text, 0);
    }
    


    public void OnCheckboxAdd()
    {

        Transform t = Instantiate(checkboxPrefab, parentTransform);
        t.SetAsFirstSibling();

        t.Find("VarName").GetComponent<TMPro.TextMeshProUGUI>().text = nameInputField.text + " [false]";

        checkboxValues.Add(nameInputField.text, false);

        t.name = nameInputField.text;

        t.GetComponentInChildren<UnityEngine.UI.Toggle>().onValueChanged.AddListener((f) =>
        {
            OnCheckboxValueChanged(t.name, f, t.Find("VarName").GetComponent<TMPro.TextMeshProUGUI>()); 
        });
       
    }

    public void OnSliderValueChanged (string key, float value, TMPro.TextMeshProUGUI name)
    {
        if (sliderValues.ContainsKey(key))
            sliderValues[key] = value;
        else
            sliderValues.Add(key, value);

        name.text = key + " ["+ value.ToString() + "]";

        print("New slider value for " + key + " is " + value.ToString());
    }

    public void OnCheckboxValueChanged (string key, bool value, TMPro.TextMeshProUGUI name)
    {
        if (checkboxValues.ContainsKey(key))
            checkboxValues[key] = value;
        else
            checkboxValues.Add(key, value);

        name.text = key + " [" + value.ToString().ToLower() + "]";

        print("New checkbox value for " + key + " is " + value.ToString());
    }

    public void UpdateGlobals (CodeContext context)
    {

        foreach (string e in checkboxValues.Keys)
        {/*
            context.script.Globals[e] = checkboxValues[e];*/

        }

        foreach (string e in sliderValues.Keys)
        {/*
            context.script.Globals[e] = sliderValues[e];*/

        }
    }

}
