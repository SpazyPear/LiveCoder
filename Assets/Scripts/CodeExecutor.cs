using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoslynCSharp;


public class CodeExecutor : MonoBehaviour
{
    [SerializeField]
    [Multiline]
    private string source = @"
    
    class ScriptTest {
        static void RunMethod() {
            Debug.Log(\u0022dadada\u0022);
        }
    }
    
    ";

    public AssemblyReferenceAsset referenceAsset;


    public static void CallMe()
    {
        Debug.Log("called from general source");

    }

    private void Start()
    {
        StartCoroutine("AwakeCoroutine");
    }

    private IEnumerator AwakeCoroutine()
    {
        print("Starting compilation");

        ScriptDomain domain = ScriptDomain.CreateDomain("Example");

        domain.RoslynCompilerService.ReferenceAssemblies.Add(referenceAsset);


        AsyncCompileOperation operation = domain.CompileAndLoadSourceAsync(source);

        yield return operation;

        operation.CompiledType.CreateInstance().Call("RunMethod");
    }

}
