using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flickering : MonoBehaviour
{
    public float minFlickerTime = 0.01f;
    public float maxFlickerTime = 0.02f;
    public float minInterval = 0.1f;
    public float maxInterval = 3.0f;

    private Light lightComponent;

    private void Start()
    {
        lightComponent = GetComponent<Light>();
        StartCoroutine(FlickerLight());
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            lightComponent.enabled = !lightComponent.enabled;
            float flickerDuration = Random.Range(minFlickerTime, maxFlickerTime);
            yield return new WaitForSeconds(flickerDuration);
            lightComponent.enabled = !lightComponent.enabled;
            float interval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(interval);
        }
    }
}
