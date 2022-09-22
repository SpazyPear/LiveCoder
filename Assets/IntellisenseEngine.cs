using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;


public class IntellisenseEngine : MonoBehaviour
{

    Microsoft.Scripting.Hosting.ScriptEngine pythonEngine = IronPython.Hosting.Python.CreateEngine();
    Microsoft.Scripting.Hosting.ScriptSource intellisense;
    Microsoft.Scripting.Hosting.ScriptScope scope;


    private void Awake()
    {
        SetupIntellisense();
    }


    public void SetupIntellisense()
    {
        ICollection<string> currentSearchPaths = pythonEngine.GetSearchPaths();

        currentSearchPaths.Add("C:/Users/ajayv/Desktop/LiveCoder/Assets/Plugins/Lib");
        currentSearchPaths.Add("C:/Users/ajayv/Desktop/LiveCoder/Assets/Plugins/Lib/site-packages");


        pythonEngine.SetSearchPaths(currentSearchPaths);

        intellisense = pythonEngine.CreateScriptSourceFromFile(Application.streamingAssetsPath + "/python-completer.py");
        scope = pythonEngine.CreateScope();

        intellisense.Execute(scope);


    }

    string getLastWord(string value, int caretPosition)
    {
        string sub = value.Substring(0, caretPosition).Trim();


        int lastIndex = sub.LastIndexOf(' ');
        int lastNewLineIndex = sub.LastIndexOf('\n');
        int lastBracketIndex = sub.LastIndexOf(':');

        print($"{lastIndex}, {lastNewLineIndex}, {lastBracketIndex}");

        try
        {
            
            if (lastIndex == -1) lastIndex = lastNewLineIndex;
            if (lastIndex < lastNewLineIndex && lastNewLineIndex != -1) lastIndex = lastNewLineIndex;

            if (lastBracketIndex > lastIndex) lastIndex = lastBracketIndex + 1;


            if (lastIndex != -1)
            {
                try
                {
                    return sub.Substring(lastIndex, (caretPosition - lastIndex));
                }
                catch (System.Exception e) { 
                    return e.ToString(); 
                }
            }
        }
        catch (System.Exception e)
        {
            return "";
        }

        return "";
    }


    public Dictionary<string, List<ValueTypePair>> typeMap = new Dictionary<string, List<ValueTypePair>>();


    public struct ValueTypePair
    {
        public string value;
        public string valueType;
    }

    public static bool PropertyExists(dynamic obj, string name)
    {

        if (obj == null) return false;
        if (obj is IDictionary<string, object> dict)
        {
            print(name);
            return dict.ContainsKey(name);
        }
        return obj.GetType().GetProperty(name) != null;
    }


    public void ParseJObjectType (JObject obj, string key = "body")
    {
        foreach (var item in obj[key])
        {
            if (item is JObject)
            {
                if (((JObject)item).ContainsKey("var"))
                {
                    string varName = (string)(((JObject)item)["var"]);
                    string val = (string)(((JObject)item)["value"]);
                    string valType = (string)(((JObject)item)["valueType"]);

                    if (!typeMap.ContainsKey(varName))
                    {
                        typeMap.Add(varName, new List<ValueTypePair>());
                    }

                    typeMap[varName].Add(new ValueTypePair {
                        value = val,
                        valueType = valType,
                    });

                    print($"Added var {varName} as type {val}");
                }
                else if (((JObject)item).ContainsKey("body"))
                {
                    ParseJObjectType((JObject)item);

                }
                else if (((JObject)item).ContainsKey("or"))
                {
                    ParseJObjectType((JObject)item, "or");
                }
            }
        }
    }

    public void InspectType (string input)
    {
        // Ex. 
        // a = "hello"
        // a.capitalize() --- .
        // need to know on the fly the type of 'a.capitalize()'
    }

    public List<string> ParseAttribute (string attributedWord)
    {
        string[] parsed = attributedWord.Split('.');

        if (parsed.Length == 1)
        {
            parsed[0].Replace(".", "");
        }

        List<string> temp = new List<string>();
        temp.AddRange(parsed);

        return temp;
    }

    public void GetSuggestions(string value, int caretPosition)
    {
        var intellisenseFunction = scope.GetVariable("intellisense");
        var val = intellisenseFunction(value);

        if (val != "FAIL")
        {
            JObject valJson = Newtonsoft.Json.Linq.JObject.Parse(val);
            typeMap.Clear();
            ParseJObjectType(valJson);
        }

        var suggestions = scope.GetVariable("produce_suggestions");

        string lastWord = getLastWord(value, caretPosition).Trim();

        List<string> attributeList = ParseAttribute(lastWord);


        if (attributeList.Count > 0 && attributeList[0].Length > 0)
        {
            print($"Last Word: {attributeList[0].Trim()}");

            if (typeMap.ContainsKey(attributeList[0].Trim()))
            {
                string valueType = typeMap[attributeList[0]][0].valueType;
                string parsedValue = typeMap[attributeList[0]][0].value;

                print($"Parsing {parsedValue}, {valueType}");

                if (valueType == "Str")
                {
                    parsedValue = $"\"{parsedValue}\"";
                }

                var outputSuggestions = suggestions(parsedValue, valueType);

                JObject outputJson = Newtonsoft.Json.Linq.JObject.Parse(outputSuggestions);

                print(outputJson.ToString());
            }

        }
    }

}
