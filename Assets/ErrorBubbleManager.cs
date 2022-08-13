using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorBubbleManager : MonoBehaviour
{
    static GameObject errorBubble;

    private void Awake()
    {
        errorBubble = Resources.Load("UI/ErrorBubble") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void spawnBubble(Vector2Int pos, string error)
    {
        print(State.gridToWorldPos(pos));
        TMPro.TMP_Text text = Instantiate(errorBubble, Camera.main.WorldToScreenPoint(State.gridToWorldPos(pos)), Quaternion.identity).GetComponentInChildren<TMPro.TMP_Text>();
        text.text = error;
    }
}
