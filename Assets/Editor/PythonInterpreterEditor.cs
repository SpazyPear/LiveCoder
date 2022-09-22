using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using PythonProxies;

[CustomEditor(typeof(PythonInterpreter))]
[CanEditMultipleObjects]
public class PythonInterpreterEditor : Editor
{

    public string TypeToPythonTypeString (System.Type type)
    {

        PythonClass[] attrs = type.GetCustomAttributes(typeof(PythonClass), true) as PythonClass[];
        if (attrs.Length > 0)
        {
            return attrs[0].className;
        }
        else
        {

            if (type == typeof(int)) {
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

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if (GUILayout.Button("Generate Python Stubs"))
        {
            List<System.Type> types = new List<System.Type>();

            types.AddRange(System.Reflection.Assembly.GetAssembly(typeof(EntityProxy)).GetTypes());
            types.AddRange(System.Reflection.Assembly.GetAssembly(typeof(Vector2Int)).GetTypes());

            Dictionary<System.Type, PythonClass> typeToClassName = new Dictionary<System.Type, PythonClass>();


            foreach (var type in types)
            {
                PythonClass[] attrs = type.GetCustomAttributes(typeof(PythonClass), true) as PythonClass[];
                if (attrs.Length > 0)
                {
                    Debug.Log($"Found {type.Name} has matching python class with name {attrs[0].className}");
                    typeToClassName.Add(type, attrs[0]);
                }
            }

            string stubs = "";

            foreach (KeyValuePair<System.Type, PythonClass> keyValuePair in typeToClassName)
            {
                string imports = "";
                /*
                                string imports = "";

                                foreach (KeyValuePair<System.Type, string> importsKeyValuePair in typeToClassName)
                                {
                                    if (importsKeyValuePair.Value == keyValuePair.Value) continue;
                                    imports += $"from {importsKeyValuePair.Value} import *\n";
                                }*/

                Debug.Log("L " + keyValuePair.Value.className);
                string classDefinition = imports+$"class {keyValuePair.Value.className}():\n";
                string indent = "    ";

                System.Reflection.PropertyInfo[] properties = keyValuePair.Key.GetProperties(System.Reflection.BindingFlags.Public);


                System.Reflection.ConstructorInfo[] constructors = keyValuePair.Key.GetConstructors();

                if (constructors.Length > 0)
                {
                    System.Reflection.ConstructorInfo constructor = constructors[0];

                    string parameterString = "self";

                    System.Reflection.ParameterInfo[] paramInfo = constructor.GetParameters();

                    for (int i = 0; i < paramInfo.Length; i++)
                    {
                        if (i == 0) parameterString += ",";
                        parameterString += $"{paramInfo[i].Name} : {TypeToPythonTypeString(paramInfo[i].ParameterType)}";
                        if (i != paramInfo.Length - 1) parameterString += ",";
                    }

                    string initMethod = indent + $"def __init__ ({parameterString}):\n" + (indent + indent) + $"print (\"Constructing Method\")";

                    classDefinition += initMethod + "\n";

                }

                foreach (System.Reflection.PropertyInfo prop in properties)
                {
                    //Debug.Log("Found Property : " + prop.Name + " with type " + prop.PropertyType.Name);
                }

                System.Reflection.MethodInfo[] methodCalls = keyValuePair.Key.GetMethods();

                

                foreach (System.Reflection.MethodInfo method in methodCalls)
                {
                    if (keyValuePair.Value.pythonAcceptedMethodsOnly)
                    {
                        PythonMethod[] attrs = method.GetCustomAttributes(typeof(PythonMethod), true) as PythonMethod[];
                        if (attrs.Length == 0) continue;
                    }

                    // Leave out default method calls of C# to reduce confusion
                    if (method.Name == "Equals" || method.Name == "GetHashCode" || method.Name == "GetType" || method.Name == "ToString") continue;

                    if (method.Name.Contains("get_") || method.Name.Contains("set_"))
                    {

                        string methodName = method.Name;
                        methodName = methodName.Replace("get_", "");
                        methodName = methodName.Replace("set_", "");

                        classDefinition += indent + $"{methodName} = {TypeToPythonTypeString(method.ReturnType)}()\n";

                        continue;
                    }
                    //Debug.Log("Found Method : " + method.Name + " with type " + method.ReturnType.Name);

                    string parameterString = "self";

                    System.Reflection.ParameterInfo[] paramInfo = method.GetParameters();

                    for (int i = 0; i < paramInfo.Length; i++)
                    {
                        if (i == 0) parameterString += ",";
                        parameterString += $"{paramInfo[i].Name} : {TypeToPythonTypeString(paramInfo[i].ParameterType)}";
                        if (i != paramInfo.Length - 1) parameterString += ",";
                    }

                    // 
                    classDefinition += indent + $"def {method.Name}({parameterString}) -> {TypeToPythonTypeString(method.ReturnType)}:\n" + (indent+indent) + $"print('called {method.Name} on {keyValuePair.Value.className}')\n";
                }

                stubs += classDefinition +"\n";

                string path = Application.streamingAssetsPath + "/output/socket-python-completer/game_stubs.py";

                System.IO.File.WriteAllText(path, stubs);
            }


            Debug.Log(stubs);
        }    
    }
}
