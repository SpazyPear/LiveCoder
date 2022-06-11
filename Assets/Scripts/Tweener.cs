using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Tweener : MonoBehaviour
{
   // private Tween activeTween;
    public List<Tween> activeTweens;
    private List<Tween> toBeRemoved;


    // Start is called before the first frame update
    void Start()
    {
        activeTweens = new List<Tween>();
        toBeRemoved = new List<Tween>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Tween activeTween in activeTweens)
        {
            try
            {
                if (activeTween.Target != null)
                {
                    if (Vector3.Distance(activeTween.Target.position, activeTween.EndPos) > 0.04f)
                    {
                        float timeFraction = (Time.time - activeTween.StartTime) / activeTween.Duration;
                        float newTime = Mathf.Pow(timeFraction, 2);
                        activeTween.Target.position = Vector3.Lerp(activeTween.Target.position, activeTween.EndPos, timeFraction);

                    }
                    else
                    {
                        activeTween.Target.position = activeTween.EndPos;
                        toBeRemoved.Add(activeTween);

                    }
                }
            }
            


            catch (Exception e)
            {

            }
        }

        for (int i = toBeRemoved.Count - 1; i > 0; i--)
        {
            GameObject obj = toBeRemoved[i].Target.gameObject;
            activeTweens.Remove(toBeRemoved.ElementAt(i));

            toBeRemoved.RemoveAt(i);

        } 
    }

    public void AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration)
    {
       
        activeTweens.Add(new Tween(targetObject, startPos, endPos, Time.time, duration - UnityEngine.Random.Range(-9f, 0.5f)));
        
    }

    public async Task waitForComplete()
    {
        while (activeTweens.Count > 0)
        {
            await Task.Yield();
        }
    }

   

}
