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
    public ProjectileLane lane;

    private void Start()
    {
        Destroy(gameObject, aliveRange);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Entity character = collision.gameObject.GetComponentInParent<Entity>();
        Shield shield = collision.GetComponentInChildren<Shield>();
        if (character && character.ownerPlayer != ownerPlayer)
        {
            character.takeDamage(damage, this);
            Destroy(gameObject);
        }
        else if (shield)
        {
            shield.takeShieldDamage(damage);
            Destroy(gameObject);
        }

    }
}
