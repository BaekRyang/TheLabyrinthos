using System;
using TypeDefs;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.Log("Settings Instance already exist");
    }

    private void OnDisable()
    {
        SystemObject.Instance.SaveSetting();
    }

    public void SetVolume(Slider slider)
    {
        string target = slider.transform.parent.parent.name;
        SystemObject.Instance.audioMixer.SetFloat(target, Mathf.Log10(slider.value / 100) * 20);
    }

    public void SetResolution(string resolution)
    {
        if (resolution == "HD")
            Screen.SetResolution(1280, 720, Screen.fullScreenMode);
        else if (resolution == "FHD")
            Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
        else if (resolution == "QHD")
            Screen.SetResolution(2560, 1440, Screen.fullScreenMode);
        else if (resolution == "UHD")
            Screen.SetResolution(3840, 2160, Screen.fullScreenMode);
    }

    public void SetWindowMode(string resolution)
    {
        if (resolution == "Fullscreen")
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        else if (resolution == "Borderless")
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        else if (resolution == "Windowed")
            Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    public void SetVsync(bool use)
    {
        QualitySettings.vSyncCount = use ? 1 : 0;
    }
    public void SetFrameLimit(int frame)
    {
        Application.targetFrameRate = frame;
    }

    public void Resume()
    {
        GameManager.Instance.SystemButtonAction("Resume");
    }
}
