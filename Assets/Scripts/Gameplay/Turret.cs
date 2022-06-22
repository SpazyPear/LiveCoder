using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Entity
{
    GameObject projectile;
    Character currentTarget;
    Transform barrel;
    bool rotatingBarrel;

    private void Start()
    {
        projectile = Resources.Load("Prefabs/projectile") as GameObject;
        barrel = transform.GetChild(0);
        target(GameObject.FindObjectOfType<Character>());
    }

    public void target(Character enemy)
    {
        rotatingBarrel = false;
        currentTarget = enemy;
        rotatingBarrel = true;
        StartCoroutine(rotateBarrel());
        StartCoroutine(debugShoot());
    }

    public void shoot()
    {
        GameObject obj = Instantiate(projectile, barrel.transform.GetChild(0).position, barrel.rotation);
        obj.GetComponent<Rigidbody>().AddForce(barrel.forward * 3000f);
    }

    IEnumerator rotateBarrel()
    {
        while (rotatingBarrel)
        {
            if (currentTarget)
            {
                var lookPos = currentTarget.transform.position - barrel.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                barrel.rotation = Quaternion.Slerp(barrel.rotation, rotation, Time.deltaTime * 2f);
            }
            rotatingBarrel = false;
            yield return null;
        }
    }

    IEnumerator debugShoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            shoot();
        }
    }
}
