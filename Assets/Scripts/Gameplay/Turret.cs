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

    TurretData turretData;
    GameObject projectile;
    Character currentTarget;
    Transform pivot;
    [SerializeField] Transform barrel;
    ParticleSystem shootPS;
    public Transform shootPoint;
    bool rotatingBarrel;

    private void Start()
    {
        projectile = Resources.Load("Prefabs/projectile") as GameObject;
        pivot = transform.GetChild(0);
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
        if (!isDisabled)
        {
            GameObject obj = Instantiate(projectile, shootPoint.position, pivot.rotation);
            obj.GetComponent<Rigidbody>().AddForce(pivot.forward * 3000f);
            shootPS.Play();
        }
    }

    IEnumerator rotateBarrel()
    {
        while (rotatingBarrel && currentTarget)
        {
            if (!isDisabled)
            {
                var lookPos = currentTarget.transform.position - pivot.position;
                lookPos += new Vector3(0, 2, 0);
                var rotation = Quaternion.LookRotation(lookPos);
                pivot.rotation = Quaternion.Slerp(pivot.rotation, rotation, Time.deltaTime * 2f);
            }

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

    public override void OnEMPDisable(float strength)
    {
        base.OnEMPDisable(strength);
        barrel.GetComponent<MeshRenderer>().material.SetFloat("_EMPFactor", 5.5f);
    }

    public override void EMPRecover()
    {
        base.EMPRecover();
        barrel.GetComponent<MeshRenderer>().material.SetFloat("_EMPFactor", 0);
    }
}
