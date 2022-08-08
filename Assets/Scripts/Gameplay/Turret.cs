using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class TurretProxy : EntityProxy
{
    Turret target;

    [MoonSharpHidden]
    public TurretProxy(Turret p) : base(p)
    {
        this.target = p;
    }

    public void targetCharacter(Character enemy) => target.target(enemy);
    public void shootCharacter() => target.shoot();

}

public class Turret : Entity
{
 

    GameObject projectile;
    Character currentTarget;
    Transform barrel;
    ParticleSystem shootPS;
    public Transform shootPoint;
    bool rotatingBarrel;

    private void Start()
    {
        projectile = Resources.Load("Prefabs/projectile") as GameObject;
        barrel = transform.GetChild(0);
        shootPS = GetComponentInChildren<ParticleSystem>();
        
    }

    public override void OnStart()
    {
        base.OnStart();
        target(GameObject.FindObjectOfType<Character>());
        StartCoroutine(debugShoot());
    }

    public void target(Character enemy)
    {
        rotatingBarrel = false;
        currentTarget = enemy;
        rotatingBarrel = true;
        StartCoroutine(rotateBarrel());
    }

    public void shoot()
    {
        GameObject obj = Instantiate(projectile, shootPoint.position, barrel.rotation);
        obj.GetComponent<Rigidbody>().AddForce(barrel.forward * 4000f);
        shootPS.Play();
    }

    IEnumerator rotateBarrel()
    {
        while (rotatingBarrel)
        {
            if (currentTarget)
            {
                var lookPos = currentTarget.transform.position - barrel.position;
                lookPos += new Vector3(0, 2, 0);
                var rotation = Quaternion.LookRotation(lookPos);
                barrel.rotation = Quaternion.Slerp(barrel.rotation, rotation, Time.deltaTime * 2f);
            }
            else 
                rotatingBarrel = false;

            yield return null;
        }
    }

    IEnumerator debugShoot()
    {

        while (true)
        {
            yield return new WaitForSeconds(1);
            if (!currentTarget)
                target(GameObject.FindObjectOfType<Character>());

            shoot();
        }
    }
}
