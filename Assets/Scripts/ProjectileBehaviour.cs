using System.Collections;
using System.Collections.Generic;
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
        Destroy(gameObject, aliveRange);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Shield shield = collision.GetComponentInParent<Shield>();
        Unit character = collision.gameObject.GetComponentInParent<Unit>();
        if (shield == null && character && character.ownerPlayer != ownerPlayer)
        {
            character.attachedModules[lane].takeDamage(damage, this);
            Destroy(gameObject);
        }
        else if (shield)
        {
            shield.takeShieldDamage(damage);
            Destroy(gameObject);
        }
    }
}
