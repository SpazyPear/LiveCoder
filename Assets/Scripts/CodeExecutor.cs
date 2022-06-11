using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoslynCSharp;
using RoslynCSharp.Compiler;
using UnityEngine.UI;



public class CodeExecutor : MonoBehaviour
{
    public InputField input;
    public Transform codeEditor;

    public AssemblyReferenceAsset referenceAsset;


    string source = "";
    public void OnExecuteCode()
    {
        source = input.text;
       
        StartCoroutine("AwakeCoroutine");

    }



    private IEnumerator AwakeCoroutine()
    {
        print("Starting compilation");

        source = @"
            using UnityEngine;

            class PlayerRunner {
        " + source + " } ";

            ScriptDomain domain = ScriptDomain.CreateDomain("Example");

            domain.RoslynCompilerService.ReferenceAssemblies.Add(referenceAsset);


        AsyncCompileOperation operation;

        try
        {
            operation = domain.CompileAndLoadSourceAsync(source);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Compiler Error => {e}");
            operation = null;       
        }

        if (operation == null) yield break;

        yield return operation;

        if (domain.CompileResult.Success == true)
        {

            ScriptProxy proxy = operation.CompiledType.CreateInstance();

            proxy.Call("Start");

            codeEditor.gameObject.SetActive(false);

            while (Application.isPlaying)
            {
                proxy.Call("OnStep");
                yield return new WaitForSeconds(1);
            }

            
        }
        else
        {
            foreach (CompilationError error in domain.CompileResult.Errors)
            {
                if (error.IsError)
                {
                    Debug.LogWarning($"Caught Compiler Error => {error}");
                }
            }
        }
    }
    
}


