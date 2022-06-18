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

public enum CodeSuggestionType
{
    Function, 
    Property
}

public struct CodeSuggestion
{
    public string name;
    public CodeSuggestionType type;
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
    Dictionary<string, DynValue> currentGlobalsMap = new Dictionary<string, DynValue>();

    public void OpenEditor (CodeContext context)
    {
        currentGlobalsMap.Clear();
        codeEditor.gameObject.SetActive(true);
        input.text = context.source;
        headerText.text = context.character.GetType().ToString();
        editingContext = context;
        editingContext.script.DoString(context.source);
        globalManager.OnScriptStart(editingContext.script, target: context.character);
        currentGlobalsMap = setupIntellisense(editingContext.script.Globals);

        input.onValueChanged.AddListener(OnValueChanged);
        
    }

    string loadedSuggestion = "";
    string lastWord = "";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            print(loadedSuggestion);
            input.text = input.text.Insert(input.caretPosition, loadedSuggestion);
        }
    }

    string getLastWord (string value)
    {
        string sub = value.Substring(0, input.caretPosition+1).Trim();
        
        int lastIndex = sub.LastIndexOf(' ');
        int lastNewLineIndex = sub.LastIndexOf('\n');

        if (lastIndex == -1) lastIndex = lastNewLineIndex;
        if (lastIndex < lastNewLineIndex && lastNewLineIndex != -1) lastIndex = lastNewLineIndex;
        

        if (lastIndex != -1) {
            try
            {
                return sub.Substring(lastIndex, (input.caretPosition - lastIndex));
            }
            catch (System.Exception e) { return ""; }
        }

        return "";
    }


    void printList(List<string> strings)
    {
        string b = "[";
        foreach (string s in strings) b += s + ",";
        b += "]";
        print(b);
    }



    void OnValueChanged (string value) {
        string lastWord = getLastWord(value);
        

        string[] split = lastWord.Split(".");

        print(split.Length);
        if (split.Length == 1)
        {

            List<string> keys = new List<string>(currentGlobalsMap.Keys);

            keys = keys.Where((s) => s.Contains(split[0].Trim())).ToList<string>();

            printList(keys);

            if (keys.Count > 0)
                loadedSuggestion = keys[0];
            else
                loadedSuggestion = "";
        }
        else
        {

        }

    }

    public List<CodeSuggestion> suggestions (string lastWord, Dictionary<string, DynValue> map)
    {

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
                    if (info.IsPublic) {
                        temp.Add(new CodeSuggestion { name = info.Name, type = CodeSuggestionType.Function });
                    }
                }

                foreach (FieldInfo info in fields)
                {
                    if (info.IsPublic)
                    {
                        temp.Add(new CodeSuggestion { name = info.Name, type = CodeSuggestionType.Property });
                    }
                }

            }


            
            if (val.Table != null)
            {
                foreach (KeyValuePair<string, DynValue> pair in setupIntellisense(val.Table, true))
                {
                    temp.Add(new CodeSuggestion { name = pair.Key, type = (pair.Value.Function != null) ? CodeSuggestionType.Function : CodeSuggestionType.Property });
                }

            }
        }
        else if (lastWord == "")
        {
            foreach (KeyValuePair<string, DynValue> pair in map)
            {
                temp.Add(new CodeSuggestion { name = pair.Key, type = (pair.Value.Function != null) ? CodeSuggestionType.Function : CodeSuggestionType.Property });
            }
        }

        return temp;
    }

    public Dictionary<string, DynValue> setupIntellisense(Table inputTable, bool printLog = false)
    {
        print("Setting up intellisense");
        Dictionary<string, DynValue> keyValues = new Dictionary<string, DynValue>();

        foreach (string key in inputTable.Keys.AsObjects<string>())
        {
            if (printLog)
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


