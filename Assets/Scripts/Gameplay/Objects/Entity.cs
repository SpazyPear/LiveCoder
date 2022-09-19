using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using System;
using UnityEngine.UI;
using Photon.Pun;

public class EntityProxy
{
    Entity target;

    [MoonSharpHidden]
    public EntityProxy(Entity p)
    {
        this.target = p;
    }

    public Vector2Int gridPos
    {
        get
        {
            return target.gridPos;
        }
    }

    
    public string id => target.ID.ToString();
    public int health => target.currentHealth;

    public bool friendly => target.ownerPlayer.playerID == PhotonNetwork.LocalPlayer.UserId;

    public Vector2Float worldPos()
    {
        return Vector2Float.fromVec2(new Vector2(target.transform.position.x, target.transform.position.z));
    }
}

public class Entity : ControlledMonoBehavour
{
    public int currentHealth;
    public int selfDestructRange = 2;
    public int selfDestructDamage = 2;
    public Vector2Int gridPos;
    public PlayerManager ownerPlayer;
    public int cost;
    public CodeContext codeContext;
    public int ID;
    public bool isDisabled;
    public EntityData entityData;
    RectTransform CanvasRect;
    RectTransform healthBarObj;
    protected Slider healthBar;
    public PhotonView photonView;
    public int viewID => photonView.ViewID;
   

    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual async void die(object sender = null)
    {
        if (ownerPlayer)
            ownerPlayer.units.Remove(this);
     
        
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

    public virtual void Awake()
    {
        
        codeContext.entity = this;
      
        GameObject.FindObjectOfType<CodeExecutor>().codeContexts.Add(codeContext);
        CanvasRect = GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        healthBarObj = Instantiate(Resources.Load("UI/HealthBar") as GameObject, GameObject.FindObjectOfType<Canvas>().transform).GetComponent<RectTransform>();
        currentHealth = entityData.maxHealth;
        healthBar = healthBarObj.GetComponentInChildren<Slider>();
        photonView = GetComponentInParent<PhotonView>();
        GameManager.unitInstances.Add(viewID, this);
        //PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "unitInstances", GameManager.unitInstances } });
        // healthBar.value = currentHealth;
    }

    public virtual void Update()
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

    [MoonSharp.Interpreter.MoonSharpHidden]
    public virtual void takeDamage(int damage, object sender = null)
    {
        if (currentHealth - damage > 0)
        {
            if (!healthBarObj.gameObject.activeInHierarchy)
                healthBarObj.gameObject.SetActive(true);

            currentHealth -= damage;
            StopCoroutine(shakeHealthBar());
            StartCoroutine(shakeHealthBar());
            healthBar.value = (float)currentHealth / entityData.maxHealth;


            return;
        }
            
        die(sender);
    }

    [MoonSharp.Interpreter.MoonSharpHidden]
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
                    if (GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity && GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren<Character>() != gameObject.GetComponentInChildren<Character>())
                    {
                        if (GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren<Entity>() != null)
                            GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren<Entity>().takeDamage(2);
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
    
    async void EMPTimer(float strength)
    {
        if (!entityData) return;

        float timer = entityData.empResistance * strength;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            await Task.Yield();
        }
        EMPRecover();
    }

    public virtual void EMPRecover()
    {
        isDisabled = false;
    }

    public virtual void OnEMPDisable(float strength)
    {
        if (isDisabled)
        {
            return;
        }
        isDisabled = true;
        EMPTimer(strength);
    }

    [PunRPC]
    public IEnumerator replicatedTeleport(float x, float y, float z, float pitch, float yaw, float roll, int gridX, int gridY)
    {
        transform.position = new Vector3(x, y, z);
        transform.rotation = Quaternion.Euler(pitch, yaw, roll);
        gridPos = new Vector2Int(gridX, gridY);
        yield return null;
    }

    public List<Entity> checkForInRangeEntities(string typeName, bool friendlies, bool enemies)
    {
        Type type = Type.GetType(typeName);
        List<Entity> foundEntities = new List<Entity>();
        for (int x = -entityData.range; x <= entityData.range; x++)
        {
            for (int y = -entityData.range; y <= entityData.range; y++)
            {
                try
                {
                    if (GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity && GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren(type) && GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren(type) != this)
                    {
                        if (!friendlies && (GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren(type) as Entity).ownerPlayer == this.ownerPlayer) continue;

                        if (!enemies && (GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren(type) as Entity).ownerPlayer != this.ownerPlayer) continue;

                        foundEntities.Add(GridManager.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren(type) as Entity);
    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        return foundEntities;
    }
    
    private void OnDestroy()
    {
        
    }
}
