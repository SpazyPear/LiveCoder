using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    int health;
    int damage = 1;
    private void OnCollisionEnter(Collision collision)
    {
        Character character = collision.gameObject.GetComponent<Character>();
        if (character)
        {
            character.takeDamage(damage);
        }
        Destroy(gameObject);
    }
}
