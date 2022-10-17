using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ProjectileLane
{
    Flat,
    Above
}

public class ProjectileBehaviour : MonoBehaviour
{
    public int damage = 1;
    public PlayerManager ownerPlayer;
    public float aliveRange;
    public ProjectileLane projectileLane;
    public int lane;

    private void Start()
    {
        //Destroy(gameObject, aliveRange);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Shield shield = collision.GetComponentInParent<Shield>();
        Unit character = collision.gameObject.GetComponentInParent<Unit>();
        if (shield == null && character && character.ownerPlayer != ownerPlayer)
        {
            character.takeDamage(lane, damage);
            GridManager.DestroyOnNetwork(gameObject);
        }
        else if (shield)
        {
            shield.takeShieldDamage(damage);
            GridManager.DestroyOnNetwork(gameObject);
        }
    }
}
