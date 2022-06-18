using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemySoldier : Soldier
{
    bool chasePlayer = true;
    Animator stateMachine;
    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponent<Animator>();
        //stateMachine.SetTimeUpdateMode(DirectorUpdateMode.Manual);

    }



    // Update is called once per frame
    public override void OnStep()
    {
        base.OnStep();
        
    }
}
