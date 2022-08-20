using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Entity
{
    

    private void OnTriggerEnter(Collider collider)
    {
        print(collider.name);
        Character character = collider.GetComponentInChildren<Character>();
        if (character)
        {
            //character.codeContext.script = this.codeContext.script;
            GameObject.FindObjectOfType<CodeExecutor>().StartCoroutine(GameObject.FindObjectOfType<CodeExecutor>().ResetScript(codeContext.source, character));
            print("Recieved character hit -- resetting to \n " + this.codeContext.script);
        }
    }
}

public class TrapProxy : EntityProxy
{
    Trap target;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public TrapProxy(Trap p) : base(p)
    {
        this.target = p;
    }

}
