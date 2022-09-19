using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using Photon.Pun;

public class TurretProxy : EntityProxy
{
    Turret target;

    [MoonSharpHidden]
    public TurretProxy(Turret p) : base(p)
    {
        this.target = p;
    }

    public void targetCharacter(Entity e) => target.target(e);
    public void shoot() => target.shoot();
    
    public void lookAt(Vector2Float pos) => target.lookAt(pos);

}

public class Turret : Entity
{

    TurretData turretData { get { return entityData as TurretData; } }
    GameObject projectile;
    Entity currentTarget;
    Transform pivot;
    [SerializeField] Transform barrel;
    ParticleSystem shootPS;
    public Transform shootPoint;
    bool rotatingBarrel;

    public override void Start()
    {
        base.Start();
        projectile = Resources.Load("Prefabs/projectile") as GameObject;
        pivot = transform.GetChild(0);
        shootPS = GetComponentInChildren<ParticleSystem>();
    }

    public override void OnStart()
    {
        base.OnStart();
        //target(GameObject.FindObjectOfType<Character>());
        //StartCoroutine(debugShoot());
    }

    public void target(Entity enemy)
    {
        rotatingBarrel = false;
        this.StopAllCoroutines();
        currentTarget = enemy;
        rotatingBarrel = true;
        StartCoroutine(rotateBarrel());
    }

    [PunRPC]
    IEnumerator replicatedShoot()
    {
        GameObject obj = Instantiate(projectile, shootPoint.position, pivot.rotation);
        obj.GetComponentInChildren<ProjectileBehaviour>().ownerPlayer = ownerPlayer;
        obj.GetComponentInChildren<ProjectileBehaviour>().aliveRange = turretData.prpjectileAliveTime;
        obj.GetComponent<Rigidbody>().AddForce(pivot.forward * 3000f);
        shootPS.Play();
        yield return null;
    }

    public void shoot() //todo action function type that checks if disabled, has energy and depletes energy
    {
        if (!isDisabled)
        {
            photonView.RPC("replicatedShoot", RpcTarget.All);
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

    IEnumerator rotateBarrelTowards(Vector2Float point)
    {

        while (rotatingBarrel )
        {
            if (!isDisabled)
            {
                var lookPos = new Vector3(point.x, transform.position.y, point.y)  - pivot.position;
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

    private Vector3 lookatPoint = Vector3.zero;

    public void lookAt (Vector2Float lookAt)

    {
        this.StopAllCoroutines();
        rotatingBarrel = false;
        currentTarget = null;
        
        rotatingBarrel = true;
        StartCoroutine(rotateBarrelTowards(lookAt));
    }

    public override void OnEMPDisable(float strength)
    {
        base.OnEMPDisable(strength);
        barrel.GetComponent<MeshRenderer>().material.SetFloat("_EMPFactor", 5f);
    }

    public override void EMPRecover()
    {
        base.EMPRecover();
        barrel.GetComponent<MeshRenderer>().material.SetFloat("_EMPFactor", 0);
    }
}
