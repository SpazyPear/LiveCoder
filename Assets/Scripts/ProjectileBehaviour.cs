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
    bool isColliding;
    public ProjectileLane lane;

    private void Start()
    {
        Destroy(gameObject, 4);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!isColliding)
        {
            isColliding = true;
            Character character = collision.gameObject.GetComponentInChildren<Character>();
            if (character)
            {
                character.takeDamage(damage, this);
            }
        }
        Destroy(gameObject);
    }
}
