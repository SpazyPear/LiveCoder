using Photon.Pun;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class HealthBar : MonoBehaviour
{
    RectTransform CanvasRect;
    RectTransform healthBarObj;
    public Slider healthBar;
    public const float yOffset = 2;
    Transform target;
    [HideInInspector]
    public int currentHealth;
    public PhotonView photonView;
    
    protected virtual void Awake()
    {
        
    }

    public void setupHealthBar(Transform target)
    {
        this.target = target;
        //photonView = target.parent.GetComponent<PhotonView>();
        healthBarObj = GetComponent<RectTransform>();
        CanvasRect = GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        //transform.parent = target;
       // transform.parent = GameObject.Find("HealthBars").transform;
    }

    void Update()
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.position + new Vector3(0, yOffset, 0));
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

    public void OnHealthChanged(object sender, float value)
    {
        if (!healthBarObj.gameObject.activeInHierarchy)
            healthBarObj.gameObject.SetActive(true);

        StopCoroutine(shakeHealthBar());
        StartCoroutine(shakeHealthBar());
        healthBar.value = value;

        if (value <= 0)
        {
            GridManager.DestroyOnNetwork(gameObject);
        }
    }
}
