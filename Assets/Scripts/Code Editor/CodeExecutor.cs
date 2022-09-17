using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using MoonSharp.Interpreter;
using UnityEngine.Events;
using System.Linq;

#nullable enable

public enum CodeSuggestionType
{
    Function,
    Property,
    Constructor
}

public struct CodeMethodParameters
{
    public string name;
    public string type;
}

public struct CodeSuggestion
{
    public string name;
    public CodeSuggestionType type;

    public string returnType;
    public CodeMethodParameters[] parameters;
}



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
    public Microsoft.Scripting.Hosting.ScriptSource pythonScript;
    public Microsoft.Scripting.Hosting.ScriptScope pythonScriptScope;

    public Entity character;
    public int ownerPlayer;

    public bool shouldExecute = true;
}


public class CodeExecutor : MonoBehaviour
{
    
    [Header("Editor UI")]
    public TMP_InputField input;
    public TextMeshProUGUI headerText;
    public Transform codeEditor;

    [Header("Context & Script Management")]
    public UnityEvent<Script> preScript;
    public List<CodeContext> codeContexts;


    public static List<UnityEngine.Events.UnityAction> onStepActions = new List<UnityAction>();

    public void RunCode ()
    {
        StopAllCoroutines();
        StartCoroutine("AwakeCoroutineLua");
    }


    // Saves code
    public void OnExecuteCode()
    {
       /* editingContext.source = input.text;
        for (int i = 0; i < otherEditingContexts.Count; i++)
        {
            otherEditingContexts[i].source = input.text;
        }
        GetComponent<IntellisenseHandler>().LoadNewSuggestions(new List<CodeSuggestion>());
        input.onValueChanged.RemoveAllListeners();*/
    }

    public void CloseEditor()
    {
        
       /* codeEditor.gameObject.SetActive(false);
        GetComponent<IntellisenseHandler>().LoadNewSuggestions(new List<CodeSuggestion>());
        input.onValueChanged.RemoveAllListeners();*/
    }

    GlobalManager globalManager = new GlobalManager();

    public CodeContext editingContext;

    List<CodeContext> otherEditingContexts = new List<CodeContext>();

    Dictionary<string, DynValue> currentGlobalsMap = new Dictionary<string, DynValue>();

    string loadedSuggestion = "";
    string lastWord = "";

    public void AddEditingContext (CodeContext newContext)
    {
        otherEditingContexts.Add(newContext);
    }

    public void ClearOtherContexts ()
    {
        otherEditingContexts.Clear();
    }

    public void OpenEditor (CodeContext context)
    {
       /* print("Editor for " + context.character.name);
        codeEditor.gameObject.SetActive(true);
        input.text = context.source;
        //headerText.text = context.character.GetType().ToString();
        editingContext = context;
        editingContext.script.DoString(context.source);
        globalManager.OnScriptStart(editingContext.script, target: context.character);
        Dictionary<string, DynValue> map = setupIntellisense(editingContext.script.Globals,out currentSuggestions);
        currentGlobalsMap = setupIntellisense(editingContext.script.Globals, out currentSuggestions, true);

        input.onValueChanged.AddListener(OnValueChanged);
*/
        
    }

    public void CopyCodeContext (CodeContext context)
    {

    }

    List<CodeSuggestion> currentSuggestions = new List<CodeSuggestion>();

    

    void OnValueChanged(string value)
    {
      /*
        string lastWord = getLastWord(input.text);

        if (lastWord.Trim() != "")
        {
            string[] split = lastWord.Split(".");

            if (split.Length == 1)
            {
                List<string> keys = new List<string>(currentGlobalsMap.Keys);

                keys = keys.Where((s) => s.Contains(split[0].Trim())).ToList<string>();

                List<CodeSuggestion> foundSuggestions = currentSuggestions.Where((s) => s.name.Contains(split[0].Trim())).ToList();

                printListCS(foundSuggestions);

                GetComponent<IntellisenseHandler>().LoadNewSuggestions(foundSuggestions);

                if (keys.Count > 0)
                {

                    lastMap = currentGlobalsMap;
                }
            }
            else if (split.Length == 2)
            {
                string splitWord = split[1].Trim();
                print("1 : " + splitWord);
                List<CodeSuggestion> _suggestions = suggestions(split[0].Trim(), lastMap);

                if (_suggestions.Count > 0)
                {
                    _suggestions = _suggestions.Where((s) => s.name.Contains(splitWord)).ToList<CodeSuggestion>();

                    List<string> suggestionKeys = new List<string>();

                    _suggestions.ForEach((s) => { suggestionKeys.Add(s.name); });

                    List<CodeSuggestion> filteredSuggestions = _suggestions.Where((s) => s.name.Contains(splitWord)).ToList();

                    if (filteredSuggestions.Count > 0)
                    {
                        printListCS(filteredSuggestions);

                        GetComponent<IntellisenseHandler>().LoadNewSuggestions(filteredSuggestions);

                    }
                    else
                    {
                        if (suggestionKeys.Count > 0)
                        {
                            GetComponent<IntellisenseHandler>().LoadNewSuggestions(_suggestions);
                        }

                    }
                }


            }
        }
        else
        {
            List<CodeSuggestion> foundSuggestions = currentSuggestions.ToList();
            GetComponent<IntellisenseHandler>().LoadNewSuggestions(foundSuggestions);
        }*/
    }

    void printList(List<string> strings)
    {
        string b = "[";
        foreach (string s in strings) b += s + ",";
        b += "]";
        print(b);
    }

    string giveList(List<string> strings)
    {
        string b = "[";
        foreach (string s in strings) b += s + ",";
        b += "]";
        return b;
    }

    void printListCS(List<CodeSuggestion> strings)
    {
        try
        {

            if (strings == null) return;

            string b = "[";
            foreach (CodeSuggestion s in strings)
            {
                if (s.type == CodeSuggestionType.Function)
                    b += "(" + s.returnType.ToString() + ")" + s.name + " => " + giveList(s.parameters.Select((a) => "{" + a.name + ":" + a.type + "}").ToList()) + " " + ",";
                else
                    b += "(" + s.returnType.ToString() + ")" + s.name + " " + ",";

            }

            b += "]";
            print(b);

        }
        catch (System.Exception e)
        {

        }
    }


    public List<CodeSuggestion> suggestions(string lastWord, Dictionary<string, DynValue> map)
    {
        print("Finding suggestion for :" + lastWord);

        List<CodeSuggestion> temp = new List<CodeSuggestion>();
        if (map.ContainsKey(lastWord))
        {
            DynValue val = map[lastWord];

            if (val.UserData != null)
            {
                System.Type type = GlobalManager.proxyMappings[val.UserData.Object.GetType()];
                MethodInfo[] methods = type.GetMethods();
                FieldInfo[] fields = type.GetFields();

                foreach (MethodInfo info in methods)
                {
                    if (info.IsPublic)
                    {

                        ParameterInfo[] parameters = info.GetParameters();

                        List<CodeMethodParameters> codeParams = parseParameters(parameters);

                        if (info.Name.Contains("get_"))
                        {
                            temp.Add(new CodeSuggestion { name = info.Name.Replace("get_", ""), type = CodeSuggestionType.Property, returnType = parseFullName(info.ReturnType),  parameters=codeParams.ToArray()});

                        }
                        else
                            temp.Add(new CodeSuggestion { name = info.Name, type = CodeSuggestionType.Function, returnType = parseFullName(info.ReturnType), parameters=codeParams.ToArray() });
                    }
                }

                foreach (FieldInfo info in fields)
                {
                    if (info.IsPublic)
                    {
                        temp.Add(new CodeSuggestion { name = info.Name.Replace("get_", ""), type = CodeSuggestionType.Property });
                    }
                }

            }



            if (val.Table != null)
            {
                List<CodeSuggestion> s = new List<CodeSuggestion>();
                foreach (KeyValuePair<string, DynValue> pair in setupIntellisense(val.Table, out s, true))
                {
                   
                    temp.Add(new CodeSuggestion { name = pair.Key.Replace("get_", ""), type = (pair.Value.Function != null) ? CodeSuggestionType.Function : CodeSuggestionType.Property });
                }

            }
        }
        else if (lastWord == "")
        {
            foreach (KeyValuePair<string, DynValue> pair in map)
            {
                temp.Add(new CodeSuggestion { name = pair.Key.Replace("get_", ""), type = (pair.Value.Function != null) ? CodeSuggestionType.Function : CodeSuggestionType.Property });
            }
        }

        return temp;
    }

    Dictionary<string, DynValue> lastMap = new Dictionary<string, DynValue>();

    int lastCharacterPosition;

    List<CodeSuggestion> filteredCodeSuggestios(List<CodeSuggestion> suggestions, string contains)
    {
        List<CodeSuggestion> _suggestion = suggestions;

        List<CodeSuggestion> tempSuggestions = new List<CodeSuggestion>();

        _suggestion.ForEach((e) =>
        {
            if (e.name.Contains(contains))
            {
                tempSuggestions.Add(e);
            }
        });

        return tempSuggestions;
    }

    string parseFullName (System.Type returnType)
    {
        if (returnType.FullName.Contains("System.Collections"))
        {
            string type = returnType.GenericTypeArguments[0].Name;
            string paramName = "List<" + type + ">";
            return paramName;
        }
        else
        {
            return returnType.Name;
        }
    }

    

    string getLastWord(string value)
    {
        try
        {
            string sub = value.Substring(0, input.caretPosition + 1).Trim();

            int lastIndex = sub.LastIndexOf(' ');
            int lastNewLineIndex = sub.LastIndexOf('\n');
            int lastBracketIndex = sub.LastIndexOf('(');


            if (lastIndex == -1) lastIndex = lastNewLineIndex;
            if (lastIndex < lastNewLineIndex && lastNewLineIndex != -1) lastIndex = lastNewLineIndex;

            if (lastBracketIndex > lastIndex) lastIndex = lastBracketIndex + 1;


            if (lastIndex != -1)
            {
                try
                {
                    return sub.Substring(lastIndex, (input.caretPosition - lastIndex));
                }
                catch (System.Exception e) { return ""; }
            }
        }
        catch (System.Exception e)
        {
            return "";
        }

        return "";
    }


    List<CodeMethodParameters> parseParameters (ParameterInfo[] parameters)
    {

        List<CodeMethodParameters> codeParams = new List<CodeMethodParameters>();

        foreach (ParameterInfo param in parameters)
        {
            if (param.ParameterType.FullName.Contains("System.Collections")) {
                // Handle Generic Types
                string type = param.ParameterType.GenericTypeArguments[0].Name;
                string paramName = "List<"+type+">";
                codeParams.Add(new CodeMethodParameters { name = param.Name, type = paramName, });
            }
            else {
                codeParams.Add(new CodeMethodParameters { name = param.Name, type = param.ParameterType.Name, });
            }
        }

        return codeParams;
    }

    public Dictionary<string, DynValue> setupIntellisense(Table inputTable, out List<CodeSuggestion> codeSuggestions, bool printLog = false)
    {
        print("Setting up intellisense");
        Dictionary<string, DynValue> keyValues = new Dictionary<string, DynValue>();

        List<CodeSuggestion> currentSuggestions = new List<CodeSuggestion>();

        foreach (string key in inputTable.Keys.AsObjects<string>())
        {
            if (printLog)
                print("Found : " + key + " in " + inputTable);
            keyValues.Add(key, inputTable.Get(key));

            DynValue val = inputTable.Get(key);

            if (val.UserData != null)
            {
                System.Type type = GlobalManager.proxyMappings[val.UserData.Object.GetType()];
                MethodInfo[] methods = type.GetMethods();
                FieldInfo[] fields = type.GetFields();
                print("OF Type : " + type.FullName);
                currentSuggestions.Add(new CodeSuggestion { name = key, type = CodeSuggestionType.Property, returnType = parseFullName(val.UserData.Object.GetType()) });
            }
            else if (val.Function != null)
            {
                print("Function of Type : " + val.Function.GetType().Name);
                print("Is of value : " + val.Function.GetDelegate().GetMethodInfo().Name);
            }
            else
            {
                if (val.Type == DataType.ClrFunction)
                {
                    if (GlobalManager.globalFunctionMappings.ContainsKey(key))
                    {
                        ParameterInfo[] parameters = GlobalManager.globalFunctionMappings[key].GetParameters();

                        List<CodeMethodParameters> codeParams = parseParameters(parameters);

                        string paramString = giveList(codeParams.Select((a) => "{" + a.name + ":" + a.type + "}").ToList());

                        print("Function of type : " + GlobalManager.globalFunctionMappings[key].ReturnType.FullName + " :: PARAMS :: " + paramString);

                        currentSuggestions.Add(new CodeSuggestion { name = key, type = CodeSuggestionType.Function, returnType = parseFullName(GlobalManager.globalFunctionMappings[key].ReturnType), parameters=codeParams.ToArray() });
                    }
                }    
;           }

           
        }

        codeSuggestions = currentSuggestions;

        return keyValues;
    }

    public ControlPanelManager controlPanelManager;


    public IEnumerator ResetScript (string newSource, Entity sender)
    {
        
        for (int i = 0; i < codeContexts.Count; i++)
        {
            if (codeContexts[i].character == sender)
            {

                codeContexts[i].source = newSource;
                codeContexts[i].script = new Script();
                codeContexts[i].script.DoString(newSource);
                globalManager.OnScriptStart(codeContexts[i].script, target: codeContexts[i].character);
                controlPanelManager.UpdateGlobals(codeContexts[i]);

                print("Calling start on " + i. ToString() + " ::: " + newSource);
                codeContexts[i].script.Call(codeContexts[i].script.Globals["OnStart"]);

            }
        }

        yield break;
     
    }

    

    private IEnumerator AwakeCoroutineLua()
    {

        /*

                foreach (CodeContext context in codeContexts)
                {
                    try
                    {
                        context.script.DoString(context.source);
                        globalManager.OnScriptStart(context.script, target: context.character);
                    }
                    catch (MoonSharp.Interpreter.InterpreterException e)
                    {
                        context.character.selfDestruct();
                        Debug.Log($"Doh! An error occured! {e.DecoratedMessage}");
                        Debug.Log($"Call Stack : {e.CallStack}");
                        Debug.Log($"{e.Source}");
                        ErrorBubbleManager.spawnBubble(context.character.gridPos, context.character.name + " threw: " + e.DecoratedMessage);
                    }
                }


                foreach (CodeContext context in codeContexts)
                {
                    if (context.shouldExecute)
                    {
                        try
                        {


                            controlPanelManager.UpdateGlobals(context);

                            context.script.Call(context.script.Globals["OnStart"]);

                            print("Calling start for " + context.script);
                        }

                        catch (MoonSharp.Interpreter.InterpreterException e)
                        {
                            context.character.selfDestruct();
                            Debug.Log($"Doh! An error occured! {e.DecoratedMessage}");
                            Debug.Log($"Call Stack : {e.CallStack}");
                            Debug.Log($"{e.Source}");
                        }
                    }

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

                        foreach (UnityAction action in CodeExecutor.onStepActions)
                        {
                            action.Invoke();
                        }
                        foreach (CodeContext context in codeContexts)
                        {

                            if (context.shouldExecute)
                            {
                                try
                                {
                                    controlPanelManager.UpdateGlobals(context);
                                    context.script.Call(context.script.Globals["OnStep"]);
                                }
                                catch (MoonSharp.Interpreter.InterpreterException e)
                                {
                                    context.character.selfDestruct();
                                    Debug.Log($"Doh! An error occured! {e.DecoratedMessage}");
                                    Debug.Log($"Call Stack : {e.CallStack}");
                                    Debug.Log($"{e.Source}");
                                }
                            }
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
        */

        yield return null;
    }



}


