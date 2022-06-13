using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using MoonSharp.Interpreter;
using UnityEngine.Events;

public class CodeExecutor : MonoBehaviour
{
    public InputField input;
    public Transform codeEditor;

    public UnityEvent<Script> preScript;


    string source = "";
    public void OnExecuteCode()
    {


        source = input.text;
        StopAllCoroutines();
        StartCoroutine("AwakeCoroutineLua");

    }


    private IEnumerator AwakeCoroutineLua()
    {

        Script script = new Script();

        GlobalManager globalManager = new GlobalManager();
        globalManager.OnScriptStart(script);

        script.DoString(source);
        
        

        try
        {
            foreach (ControlledMonoBehavour o in GameObject.FindObjectsOfType<ControlledMonoBehavour>())
            {
                o.OnStart();
            }
            script.Call(script.Globals["OnStart"], 4);
        }
        catch (ScriptRuntimeException e)
        {
            Debug.Log($"Doh! An error occured! {e.DecoratedMessage}");
        }

        while (Application.isPlaying)
        {
            try
            {
                foreach (ControlledMonoBehavour o in GameObject.FindObjectsOfType<ControlledMonoBehavour>())
                {
                    o.OnStep();
                }
                script.Call(script.Globals["OnStep"]);
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


