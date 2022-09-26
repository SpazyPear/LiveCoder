using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using Photon.Pun;


using PythonProxies;
using System.Linq;

public class PythonProxyObject {

    public string pythonClassName;

}

public class Entity : PlaceableObject
{
    public int selfDestructRange = 2;
    public int selfDestructDamage = 2;
    public bool isDisabled;
    public UnitData entityData;
    RectTransform CanvasRect;
    RectTransform healthBarObj;
    protected Slider healthBar;
    public int currentHealth;
    public int maxHealth = 5;
    public int viewID => photonView.ViewID;
    public bool executing = false;
   
    public virtual async void die(object sender = null)
    {
        float timer = 0;

        if (healthBar != null)
            Destroy(healthBar.gameObject);

        while (timer < 1)
        {
            if (gameObject != null)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, timer / 1);
                timer += Time.deltaTime;
                await Task.Yield();
            }
        }
        Destroy(gameObject);
    }
    
    public virtual void Start()
    {
        //GameManager.unitInstances.Add(gameObject.GetInstanceID(), this);
    }

    protected virtual void Awake()
    {
           
        
        CanvasRect = GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        healthBarObj = Instantiate(Resources.Load("UI/HealthBar") as GameObject, GameObject.FindObjectOfType<Canvas>().transform).GetComponent<RectTransform>();
        healthBar = healthBarObj.GetComponentInChildren<Slider>();
        photonView = GetComponentInParent<PhotonView>();
        currentHealth = maxHealth;
        //PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "unitInstances", GameManager.unitInstances } });
        // healthBar.value = currentHealth;
    }

    void setupHealthBar()
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(transform.position + new Vector3(0, 13f, 0));
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        if (healthBarObj)
            healthBarObj.anchoredPosition = WorldObject_ScreenPosition;
    }

    IEnumerator shakeHealthBar()
    {
        Vector3 orignalPosition = healthBar.transform.position;
        float elapsed = 0f;

        while (elapsed < 0.3)
        {
            float x = orignalPosition.x + UnityEngine.Random.Range(-1f, 1f);
            float y = orignalPosition.y + UnityEngine.Random.Range(-1f, 1f);
            healthBar.transform.position = new Vector3(x, y, healthBar.transform.position.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        healthBar.transform.position = orignalPosition;
    }

    public virtual void takeDamage(int damage, object sender = null)
    {
        if (currentHealth - damage > 0)
        {
            if (!healthBarObj.gameObject.activeInHierarchy)
                healthBarObj.gameObject.SetActive(true);

            currentHealth -= damage;
            StopCoroutine(shakeHealthBar());
            StartCoroutine(shakeHealthBar());
            healthBar.value = (float)currentHealth / maxHealth;
            return;
        }
            
        die(sender);
    }

    public virtual void selfDestruct()
    {
        photonView.RPC("replicatedSelfDestruct", RpcTarget.All);
    }

    [PunRPC]
    public IEnumerator replicatedSelfDestruct()
    {
        for (int x = -selfDestructRange; x <= selfDestructRange; x++)
        {
            for (int y = -selfDestructRange; y <= selfDestructRange; y++)
            {
                try
                {
                    if (GridManager.GridContents[gridPos.x + x, gridPos.y + y].OccupyingObject && GridManager.GridContents[gridPos.x + x, gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>() != gameObject.GetComponentInChildren<Unit>())
                    {
                        if (GridManager.GridContents[gridPos.x + x, gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>() != null)
                            GridManager.GridContents[gridPos.x + x, gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>().attachedModules.ForEach(x => x.takeDamage(2));
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        Camera.main.gameObject.GetComponent<CameraShake>().shakeCamera();
        Instantiate(Resources.Load("PS/PS_Explosion_Rocket") as GameObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return null;
    }
    


    [PunRPC]
    public IEnumerator replicatedTeleport(float x, float y, float z, float pitch, float yaw, float roll, int gridX, int gridY)
    {
        transform.position = new Vector3(x, y, z);
        transform.rotation = Quaternion.Euler(pitch, yaw, roll);
        gridPos = new Vector2Int(gridX, gridY);
        yield return null;
    }

    public PythonProxyObject CreateProxy()
    {
        return new EntityProxy(this);
    }
}
