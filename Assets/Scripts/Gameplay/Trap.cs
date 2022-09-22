using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PythonProxies;
public class Trap : Entity
{

    public override object CreateProxy()
    {
        return new TrapProxy(this);
    }

    private void OnTriggerEnter(Collider collider)
    {
        print(collider.name);
        Character character = collider.GetComponentInChildren<Character>();
        if (character)
        {
            //character.codeContext.script = this.codeContext.script;
            //GameObject.FindObjectOfType<CodeExecutor>().StartCoroutine(GameObject.FindObjectOfType<CodeExecutor>().ResetScript(codeContext.source, character));
            //print("Recieved character hit -- resetting to \n " + this.codeContext.script);
        }
    }
}
