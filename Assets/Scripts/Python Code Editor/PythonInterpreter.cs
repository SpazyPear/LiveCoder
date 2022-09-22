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
        print("Intellisense removed");
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

        globalAssigns = "";
        SetVariable("current", context.character);
        SetVariable("world", GameObject.FindObjectOfType<World>());
        SetVariable("debug", (System.Action<dynamic>)debug);


        print("Starting intellisense");

        input.onValueChanged.AddListener(OnValueChanged);

    }


    public void RecievePythonIntellisenseSuggestions (string suggestionsJSON)
    {
        print(suggestionsJSON);
        JObject valJson = Newtonsoft.Json.Linq.JObject.Parse(suggestionsJSON);
        var completions = valJson["completions"];

        List<CodeSuggestion> suggestions = new List<CodeSuggestion>();

        foreach (var comp in completions)
        {
            if (comp != null && comp is JObject)
            {

                CodeSuggestionType sugType = ((comp["typeHint"].ToString()).Contains("(") && (comp["typeHint"].ToString()).Contains(")")) ? CodeSuggestionType.Function : CodeSuggestionType.Property;

                if (comp["typeHint"].ToString().Contains("Type["))
                {
                    sugType = CodeSuggestionType.Constructor;
                }

                List<CodeMethodParameters> methodParameters = new List<CodeMethodParameters>();

                foreach (var param in comp["params"])
                {
                    methodParameters.Add(new CodeMethodParameters { name = param.ToString().Replace("param","").Trim() });
                }

                CodeSuggestion s = new CodeSuggestion
                {
                    name = comp["name"].ToString(),
                    returnType = comp["typeHint"].ToString(),
                    type = sugType,
                    parameters = methodParameters.ToArray()
                };

                suggestions.Add(s);
            }

            GetComponent<IntellisenseHandler>().LoadNewSuggestions(suggestions);
        }
    }

    string globalAssigns = "";

    void OnValueChanged(string value)
    {


        string importDefinitions = "from game_stubs import *\n";


        if (value.Length > 0)
        {
            string substring = value.Substring(0, Mathf.Clamp(input.caretPosition, 0, value.Length));

            print("Sending");
            print(importDefinitions + globalAssigns + substring);

            GetComponent<PythonSocketConnection>().SendData(importDefinitions + globalAssigns + substring);
        }

    }

    Microsoft.Scripting.Hosting.ScriptEngine pythonEngine;

    public string TypeToPythonTypeString(System.Type type)
    {

        PythonProxies.PythonClass[] attrs = type.GetCustomAttributes(typeof(PythonProxies.PythonClass), true) as PythonProxies.PythonClass[];
        if (attrs.Length > 0)
        {
            return attrs[0].className;
        }
        else
        {

            if (type == typeof(int))
            {
                return "int";
            }
            else if (type == typeof(string))
            {
                return "str";
            }
            else if (type == typeof(float))
            {
                return "float";
            }
            else if (type == typeof(bool))
            {
                return "bool";
            }

            return "None";
        }
    }


    public void SetVariable( string name, object var, Microsoft.Scripting.Hosting.ScriptScope scope = null)
    {
        
        if (var is Entity)
        {
            object proxy = ((Entity)var).CreateProxy();

            globalAssigns += $"{name} = {TypeToPythonTypeString(proxy.GetType())}()\n";

            if (scope != null)
            scope.SetVariable(name, proxy);

        }
        else
        {
            if (var != null && scope != null)
            {
                globalAssigns += $"{name} = {TypeToPythonTypeString(var.GetType())}()\n";
                scope.SetVariable(name, var);
            }
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

                globalAssigns = "";
               
                SetVariable("current", context.character, scope);
                SetVariable("world", GameObject.FindObjectOfType <World>(), scope);
                SetVariable("debug", (System.Action<dynamic>)debug, scope);
                
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
