using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;

public class PythonTypeDef
{
    public string typeName;
    public List<string> possibleValues;
}

public class PythonInterpreter : MonoBehaviour
{
    [Header("Editor UI")]
    public TMP_InputField input;
    public CodeContext editingContext;
    public List<CodeContext> codeContexts;
    public static PythonInterpreter instance;

    private void Awake()
    {
        instance = this;

        Microsoft.Scripting.Hosting.ScriptRuntime runtime = IronPython.Hosting.Python.CreateRuntime();

        runtime.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(Python3DMath.vector2)));
        runtime.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(PythonProxies.EntityProxy)));

        pythonEngine = runtime.GetEngine("py");

        ICollection<string> currentSearchPaths = pythonEngine.GetSearchPaths();

        currentSearchPaths.Add(Application.streamingAssetsPath+"/Lib");
        currentSearchPaths.Add(Application.streamingAssetsPath + "/Lib/site-packages");


        pythonEngine.SetSearchPaths(currentSearchPaths);


        input.onValueChanged.AddListener(OnValueChanged);

    }

    public static void AddContext (CodeContext context)
    {
        instance.codeContexts.Add(context);
    }

    public void OnExecuteCode()
    {
        editingContext.source = input.text;
        input.onValueChanged.RemoveAllListeners();
    }

    public void RunCode()
    {
        StopAllCoroutines();
        StartCoroutine(ExecuteScript(true));
    }

    public void OpenEditor(CodeContext context)
    {
        input.text = context.source;
        //headerText.text = context.character.GetType().ToString();
        editingContext = context;

        input.onValueChanged.AddListener(OnValueChanged);

    }


    public void RecievePythonIntellisenseSuggestions (string suggestionsJSON)
    {
        JObject valJson = Newtonsoft.Json.Linq.JObject.Parse(suggestionsJSON);
        var completions = valJson["completions"];

        List<CodeSuggestion> suggestions = new List<CodeSuggestion>();

        foreach (var comp in completions)
        {
            CodeSuggestion s = new CodeSuggestion {
                name = comp.ToString(),
                returnType = ""
            };
            suggestions.Add(s);
        }

        GetComponent<IntellisenseHandler>().LoadNewSuggestions(suggestions);

    }

    void OnValueChanged(string value)
    {

        // GetComponent<IntellisenseEngine>().GetSuggestions(value, input.caretPosition);


        string importDefinitions = "from unit import Unit\n";
        string globals = "current = Unit()\n";


        string substring = value.Substring(0, input.caretPosition);

        //GetComponent<PythonSocketConnection>().SendData(importDefinitions + globals + substring);
    }

    Microsoft.Scripting.Hosting.ScriptEngine pythonEngine;

    public void SetVariable(Microsoft.Scripting.Hosting.ScriptScope scope, string name, object var)
    {
        if (var is Entity)
        {
            object proxy = ((Entity)var).CreateProxy();
            scope.SetVariable(name, proxy);
        }
        else
        {
            scope.SetVariable(name, var);
        }
    }


    public string ProcessSource (CodeContext context)
    {

        string imports = "from Python3DMath import *\nfrom PythonProxies import *\n";

        string source = imports + context.source;

        return source.Trim();
    }

    private void debug (dynamic anything)
    {
        Debug.Log(anything.ToString());
    }

    public void TestCode()
    {
        StopAllCoroutines();
        StartCoroutine(ExecuteScript(false));
    }

    private IEnumerator ExecuteScript(bool runOnStep)
    {
        print("Starting execute step?");
        foreach (CodeContext context in codeContexts)
        {
            try
            {
                print("Found code context with code : " + context.source);
                context.pythonScript = pythonEngine.CreateScriptSourceFromString(ProcessSource(context));
                Microsoft.Scripting.Hosting.ScriptScope scope = pythonEngine.CreateScope();
               
                SetVariable(scope, "current", context.character);
                SetVariable(scope, "world", GameObject.FindObjectOfType <World>());
                SetVariable(scope, "debug", (System.Action<dynamic>)debug);
                
                context.pythonScriptScope = scope;
                context.pythonScript.Execute(scope);
                
                if (context.shouldExecute)
                {
                    if (context.pythonScriptScope.ContainsVariable("OnStart"))
                    {
                        var onStart = context.pythonScriptScope.GetVariable("OnStart");

                        if (onStart != null)
                            onStart();
                    }
                }

            }
            catch (System.Exception e)
            {
               // context.character.selfDestruct();
                Debug.Log($"Doh! An error occured! {e.ToString()}");
                Debug.Log($"Call Stack : {e.ToString()}");
                Debug.Log($"{e.Source}");
                //ErrorBubbleManager.spawnBubble(context.character.gridPos, context.character.name + " threw: " + e.ToString());
            }
        }




        do
        {
            foreach (CodeContext context in codeContexts)
            {

                if (context.shouldExecute)
                {
                    try
                    {
                        if (context.pythonScriptScope.ContainsVariable("OnStep"))
                        {
                            var onStep = context.pythonScriptScope.GetVariable("OnStep");

                            if (onStep != null)
                                onStep();
                        }
                    }
                    catch (System.Exception e)
                    {
                        //context.character.selfDestruct();
                        Debug.Log($"Doh! An error occured! {e.ToString()}");

                    }
                }
            }
            yield return new WaitForSeconds(1);
        } while (Application.isPlaying && runOnStep);
    }
}
