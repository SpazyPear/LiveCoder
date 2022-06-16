using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public PlayerManager belongingPlayer;
    public int towerHealth = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeTowerDamage(int damage)
    {
        towerHealth -= damage;
        if (towerHealth <= 0)
        {
            belongingPlayer.win();
            Destroy(gameObject);
        }
    }

}
