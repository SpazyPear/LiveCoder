using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Entity
{
    public CodeContext codeContext = new CodeContext();


    private void OnTriggerEnter(Collider collider)
    {
        Character character = collider.GetComponent<Character>();
        if (character)
            character.codeContext = this.codeContext;
    }
}
