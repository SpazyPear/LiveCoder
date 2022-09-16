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
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if (GUILayout.Button("Generate Python Stubs"))
        {
            var types = System.Reflection.Assembly.GetAssembly(typeof(EntityProxy)).GetTypes();

            foreach (var type in types)
            {

            }

        }    
    }
}
