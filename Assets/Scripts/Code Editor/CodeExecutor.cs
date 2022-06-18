using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using MoonSharp.Interpreter;
using UnityEngine.Events;

#nullable enable

[System.Serializable]
public class CodeContext
{
    [Multiline]
    public string source = @"

        function OnStart()

        end


        function OnStep()

        end

    ";
    public Script script = new Script();
    public Character character;
}


public class CodeExecutor : MonoBehaviour
{
    
    [Header("Editor UI")]
    public InputField input;
    public TextMeshProUGUI headerText;
    public Transform codeEditor;

    [Header("Context & Script Management")]
    public UnityEvent<Script> preScript;
    public List<CodeContext> codeContexts;


    public void RunCode ()
    {
        StopAllCoroutines();
        StartCoroutine("AwakeCoroutineLua");
    }


    // Saves code
    public void OnExecuteCode()
    {
        editingContext.source = input.text;
    }

    public void CloseEditor()
    {
        codeEditor.gameObject.SetActive(false);
    }

    GlobalManager globalManager = new GlobalManager();

    CodeContext editingContext;

    public void OpenEditor (CodeContext context)
    {
        codeEditor.gameObject.SetActive(true);
        input.text = context.source;
        headerText.text = context.character.GetType().ToString();
        editingContext = context;
        editingContext.script.DoString(context.source);
        globalManager.OnScriptStart(editingContext.script, target: context.character);
        Dictionary<string, DynValue> map = setupIntellisense(editingContext.script.Globals);
        foreach (string key in suggestions("current", map)) print("Key in current" + key);
    }

    public List<string> suggestions (string lastWord, Dictionary<string, DynValue> map)
    {
        if (map.ContainsKey(lastWord))
        {
            DynValue val = map[lastWord];
            
            if (val.Table != null)
            {
                List<string> keys = new List<string>(setupIntellisense(val.Table).Keys);
                return keys;
            }
        }

        return new List<string>();
    }

    public Dictionary<string, DynValue> setupIntellisense(Table inputTable)
    {
        print("Setting up intellisense");
        Dictionary<string, DynValue> keyValues = new Dictionary<string, DynValue>();

        foreach (string key in inputTable.Keys.AsObjects<string>())
        {
            print("Found : " + key + " in " + inputTable);
            keyValues.Add(key, inputTable.Get(key));
        }


        return keyValues;
    }

    private IEnumerator AwakeCoroutineLua()
    {
       


        foreach (CodeContext context in codeContexts)
        {
            context.script.DoString(context.source);
            globalManager.OnScriptStart(context.script, target: context.character);
        }
        
        try
        {
            foreach (CodeContext context in codeContexts)
            {
                context.script.Call(context.script.Globals["OnStart"]);

                print("Calling start for " + context.script);
            }
        }
        catch (ScriptRuntimeException e)
        {
            Debug.Log($"Doh! An error occured! {e.DecoratedMessage}");
            Debug.Log($"Call Stack : {e.CallStack}");
            Debug.Log($"{e.Source}");
        }

        foreach (ControlledMonoBehavour o in GameObject.FindObjectsOfType<ControlledMonoBehavour>())
        {
            o.OnStart();
        }


        while (Application.isPlaying)
        {
            try
            {
                foreach (ControlledMonoBehavour o in GameObject.FindObjectsOfType<ControlledMonoBehavour>())
                {
                    o.OnStep();
                }


                foreach (CodeContext context in codeContexts)
                {
                    context.script.Call(context.script.Globals["OnStep"]);
                }

                foreach (ControlledMonoBehavour o in GameObject.FindObjectsOfType<ControlledMonoBehavour>())
                {
                    o.OnPostStep();
                }
            }
            catch (ScriptRuntimeException e)
            {
                Debug.Log($"Doh! An error occured! {e.DecoratedMessage}");
            }

            yield return new WaitForSeconds(1);
        }

    }



}


