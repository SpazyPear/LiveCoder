using Photon.Pun;
using Python3DMath;
using PythonProxies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretModule : Module
{
    public TurretData turretData { get { return moduleData as TurretData; } private set { moduleData = value; } }

    public GameObject projectile;
    PlaceableObject currentTarget;
    public Transform pivot;
    public Transform barrel;
    public ParticleSystem shootPS;
    public Transform shootPoint;
    bool rotatingBarrel;

    public override object CreateProxy()
    {
        return new TurretModuleProxy(this);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }


    public void targetEntity(PlaceableObject enemy)
    {
        if (GridManager.isInRange(turretData.range, enemy))
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
    public void replicatedShoot()
    {
        GameObject obj = Instantiate(projectile, shootPoint.position, pivot.rotation);
        obj.GetComponentInChildren<ProjectileBehaviour>().ownerPlayer = owningUnit.ownerPlayer;
        obj.GetComponentInChildren<ProjectileBehaviour>().aliveRange = turretData.projectileAliveTime;
        obj.GetComponentInChildren<ProjectileBehaviour>().lane = lane;

        obj.GetComponent<Rigidbody>().AddForce(pivot.forward * 3000f);
        shootPS.Play();
    }

    public void shoot()
    {
        if (!owningUnit.isDisabled && owningUnit.currentEnergy > 0)
        {
            owningUnit.currentEnergy--;
            GameManager.CallRPC(this, "replicatedShoot", RpcTarget.All);
        }
    }

    IEnumerator rotateBarrel()
    {
        while (rotatingBarrel && currentTarget)
        {
            var lookPos = currentTarget.transform.position - pivot.position;
            lookPos += new Vector3(0, 2, 0);
            var rotation = Quaternion.LookRotation(lookPos);
            pivot.rotation = Quaternion.Slerp(pivot.rotation, rotation, Time.deltaTime * 2f);
            yield return null;
        }
    }

    void rotateBarrelTowards(vector2 point)
    {

        while (rotatingBarrel)
        {
            var lookPos = new Vector3(point.x, transform.position.y, point.y) - pivot.position;
            lookPos += new Vector3(0, 2, 0);
            var rotation = Quaternion.LookRotation(lookPos);
            pivot.rotation = Quaternion.Slerp(pivot.rotation, rotation, Time.deltaTime * 2f);
        }
    }

/*    IEnumerator debugShoot()
    {

        while (true)
        {
            yield return new WaitForSeconds(1);
            if (!currentTarget)
                target(GameObject.FindObjectOfType<Character>());

            shoot();
        }
    }*/

    private Vector3 lookatPoint = Vector3.zero;

    public void lookAt(vector2 lookAt)

    {
        this.StopAllCoroutines();
        rotatingBarrel = false;
        currentTarget = null;

        rotatingBarrel = true;
        rotateBarrelTowards(lookAt);
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

    public override string displayName()
    {
        return "turretModule";
    }

}
