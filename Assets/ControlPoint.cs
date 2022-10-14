using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : PlaceableObject
{
    [SerializeField]
    int hackIntegrity = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hack(PlayerManager player, int damage)
    {
        hackIntegrity -= damage;
        if (hackIntegrity <= 0)
        {
            hackIntegrity = 0;
            ownerPlayer = player;
        }
    }

    override public void OnStep()
    {
        
    }
}
