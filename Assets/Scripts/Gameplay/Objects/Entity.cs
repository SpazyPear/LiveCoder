using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using System;
using UnityEngine.UI;


using PythonProxies;
public class PythonProxyObject {

    public string pythonClassName;

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
   

    [MoonSharp.Interpreter.MoonSharpHidden]
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

    private void Awake()
    {
        codeContext.character = this;
      
        GameObject.FindObjectOfType<CodeExecutor>().codeContexts.Add(codeContext);
        PythonInterpreter.AddContext(codeContext);
        CanvasRect = GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        healthBarObj = Instantiate(Resources.Load("UI/HealthBar") as GameObject, GameObject.FindObjectOfType<Canvas>().transform).GetComponent<RectTransform>();
        currentHealth = entityData.maxHealth;
        healthBar = healthBarObj.GetComponentInChildren<Slider>();
       // healthBar.value = currentHealth;
    }

    private void Start()
    {
        
    }

    private void Update()
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
        for (int x = -selfDestructRange; x <= selfDestructRange; x++)
        {
            for (int y = -selfDestructRange; y <= selfDestructRange; y++)
            {
                try
                {
                    if (State.GridContents[gridPos.x + x, gridPos.y + y].Entity && State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren<Character>() != gameObject.GetComponentInChildren<Character>())
                    {
                        if (State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren<Entity>() != null)
                            State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren<Entity>().takeDamage(2);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        Camera.main.gameObject.GetComponent<CameraShake>().shakeCamera();
        Instantiate(Resources.Load("PS/PS_Explosion_Rocket") as GameObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
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

    private void OnDestroy()
    {
        
    }

    // TODO Change TypeName to enum for ease of use
    public Entity findClosestEntityOfType(Entity sender, string typeName)
    {
        Entity closest = null;
        float minDistance = Mathf.Infinity;
        Type type = Type.GetType(typeName);

        if (type == null)
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "findClosestEntityOfType", playerId = gameObject.name }, new Error("Incorrect type name."));
            return null;
        }

        foreach (Entity c in GameObject.FindObjectsOfType(type))
        {
            if (c == sender)
                continue;

            if (Vector2Int.Distance(c.gridPos, sender.gridPos) < minDistance)
            {
                closest = c;
                minDistance = Vector2Int.Distance(c.gridPos, sender.gridPos);
            }
        }

        return closest;
    }


    public virtual object CreateProxy()
    {
        return new EntityProxy(this);
    }
}
