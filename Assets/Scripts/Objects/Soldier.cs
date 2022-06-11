using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Character
{
   
    // Start is called before the first frame update
    void Start()
    {
        initializePlayer("ScriptableObjects/SoldierScriptableObject");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float attack()
    {
        return 0f;
    }
}
