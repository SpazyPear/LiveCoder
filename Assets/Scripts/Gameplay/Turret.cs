using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Python3DMath;
using PythonProxies;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class Turret : Unit
{
/*
    TurretData turretData { get { return entityData as TurretData; } }
    GameObject projectile;
    Unit currentTarget;
    Transform pivot;
    [SerializeField] Transform barrel;
    ParticleSystem shootPS;
    public Transform shootPoint;
    bool rotatingBarrel;

    public override object CreateProxy()
    {
        return new TurretProxy(this);
    }

    private void Start()
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

    public void target(Unit enemy)
    {
        if (isInRange(enemy))
        {
            rotatingBarrel = false;
            this.StopAllCoroutines();
            currentTarget = enemy;
            rotatingBarrel = true;
            StartCoroutine(rotateBarrel());
        }
        else
            ErrorManager.instance.PushError(new ErrorSource { function = "target", playerId = gameObject.name }, new Error("Target is out of range"));

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

    IEnumerator rotateBarrelTowards(vector2 point)
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

    public void lookAt (vector2 lookAt)

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
    }*/
}
