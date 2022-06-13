using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCGenerator : MonoBehaviour
{
    public GameObject[] input;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[,] objectGrid = PatternIdentifier.objectArrayToGrid(input);
        int[,] intGrid = PatternIdentifier.tileMapToArray(objectGrid);
        List<Rule> rules = PatternIdentifier.identifyRules(intGrid);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
