using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : Character
{
    public GiantData giantData;
    // Start is called before the first frame update
    void Start()
    {
        initializePlayer("Giant");
        giantData = initializeCharacterClass<GiantData>(characterData);
    }

}
