using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    // private Tween activeTween;
    //public List<Tween> activeTweens;
    Tween activeTween;
    bool tweenActive;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (activeTween != null)
        {
            if (Vector3.Distance(activeTween.Target.position, activeTween.EndPos) > 0.04f)
            {
                tweenActive = true;
                float timeFraction = (Time.time - activeTween.StartTime) / activeTween.Duration;
                float newTime = Mathf.Pow(timeFraction, 2);
                activeTween.Target.position = Vector3.Lerp(activeTween.Target.position, activeTween.EndPos, timeFraction);
            }
            else
            {
                activeTween.Target.position = activeTween.EndPos;
                tweenActive = false;
                activeTween = null;

            }
        }
           
    }

    public async void AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration)
    {
        await waitForComplete();
        activeTween = new Tween(targetObject, startPos, endPos, Time.time, duration);
        
    }

    public async Task waitForComplete()
    {
        while (tweenActive)
        {
            await Task.Yield();
        }
    }


}
