using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorBubbleManager : MonoBehaviour
{
    public float lerpDuration = 3f;
    public Transform lerpTarget;
    static GameObject errorBubble;

    public static ErrorBubbleManager instance;
    

    private void Awake()
    {
        instance = this;
        errorBubble = Resources.Load("UI/ErrorBubble") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void spawnBubble(Vector2Int pos, string error)
    {
        print(State.gridToWorldPos(pos));
        GameObject text = Instantiate(errorBubble, State.gridToWorldPos(pos) + new Vector3(0, 7f, 0), Quaternion.identity, FindObjectOfType<Canvas>().transform);
        text.GetComponentInChildren<TMPro.TMP_Text>().text = error;
        //text.transform.LookAt(Camera.main.transform.position);
        instance.animateBubble(text.transform);
    }
    
    void animateBubble(Transform bubble)
    {
        StartCoroutine(moveBubble(bubble)); 
    }

    IEnumerator moveBubble(Transform bubble)
    {
        float timer = 0;
        while (timer < lerpDuration)
        {
            bubble.position = Vector3.Lerp(bubble.position, Camera.main.ScreenToWorldPoint(new Vector3(lerpTarget.position.x, lerpTarget.position.y, 10f)), timer / lerpDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        bubble.position = lerpTarget.position;
    }
}
