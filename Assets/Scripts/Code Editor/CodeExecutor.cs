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

    CodeContext editingContext;

    public void OpenEditor (CodeContext context)
    {
        codeEditor.gameObject.SetActive(true);
        input.text = context.source;
        headerText.text = context.character.GetType().ToString();
        editingContext = context;
    }


    private IEnumerator AwakeCoroutineLua()
    {
        GlobalManager globalManager = new GlobalManager();


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


