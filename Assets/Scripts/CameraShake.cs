using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CameraShake : MonoBehaviour
{
    public float shakeMagnitude = 0.6f;
    public float shakeDuration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void shakeCamera()
    {
        Vector3 orignalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = orignalPosition.x + UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = orignalPosition.y + UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(x, y, transform.position.z);
            elapsed += Time.deltaTime;
            await Task.Yield();
        }
        transform.position = orignalPosition;
    }
}
