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
    public Material lightMaterial;
    public Color lightColor;

    private void Start()
    {
        lightComponent = GetComponent<Light>();
        lightMaterial = transform.parent.GetChild(0).GetComponent<Renderer>().material;
        lightColor = lightMaterial.GetColor("_EmissionColor");
        StartCoroutine(FlickerLight());
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            lightComponent.intensity = 50;
            float flickerDuration = Random.Range(minFlickerTime, maxFlickerTime);
            transform.parent.GetChild(0).GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.gray);
            yield return new WaitForSeconds(flickerDuration);
            lightComponent.intensity = 200;
            transform.parent.GetChild(0).GetComponent<Renderer>().material.SetColor("_EmissionColor", lightColor);

            float interval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(interval);
        }
    }
}
