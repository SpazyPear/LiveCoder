using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoslynCSharp;
using RoslynCSharp.Compiler;
using UnityEngine.UI;
using System.Reflection;
using MoonSharp.Interpreter;
using UnityEngine.Events;

public class CodeExecutor : MonoBehaviour
{
    public InputField input;
    public Transform codeEditor;

    public UnityEvent<Script> preScript;

    public AssemblyReferenceAsset referenceAsset;


    string source = "";
    public void OnExecuteCode()
    {


        source = input.text;

        StartCoroutine("AwakeCoroutineLua");

    }


    private IEnumerator AwakeCoroutineLua()
    {

        Script script = new Script();

        this.preScript.Invoke(script);

        script.DoString(source);


        try
        {

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
                script.Call(script.Globals["OnStep"]);
            }
            catch (ScriptRuntimeException e)
            {
                Debug.Log($"Doh! An error occured! {e.DecoratedMessage}");
            }

            yield return new WaitForSeconds(1);
        }

    }



}


