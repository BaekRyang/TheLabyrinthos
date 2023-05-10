using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetVolume(Slider slider)
    {
        string target = slider.transform.parent.parent.name;
        audioMixer.SetFloat(target, Mathf.Log10(slider.value/100)*20+10);
        Debug.Log(Mathf.Log10(slider.value / 100) * 20+10);
    }
}
