using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TypeDefs;
using UnityEngine;
using UnityEngine.Audio;
using Resolution = TypeDefs.Resolution;

public class SystemObject : MonoBehaviour
{
    public static SystemObject Instance;
    public        OptionData   optionData;
    public        AudioMixer   audioMixer;
    private void Awake()
    {
        Instance ??= this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveSetting()
    {
        //볼륨값은 Linear값에서 db으로 변환된 값이 들어있으므로 다시 Linear값으로 변환해준다.
        optionData.volume.master = audioMixer.GetFloat("Master", out float master) ? 100 * Mathf.Pow(10, master / 20) : 100f;
        optionData.volume.music  = audioMixer.GetFloat("Music",  out float music) ? 100  * Mathf.Pow(10, music  / 20) : 100f;
        optionData.volume.sfx    = audioMixer.GetFloat("SFX",    out float sfx) ? 100    * Mathf.Pow(10, sfx    / 20) : 100f;
        optionData.resolution    = new Resolution(Screen.width, Screen.height);
        optionData.screenMode    = (int)Screen.fullScreenMode;
        optionData.refreshRate   = Application.targetFrameRate;
        optionData.vsync         = QualitySettings.vSyncCount;
        
        System.IO.File.WriteAllText(Application.persistentDataPath + "/Settings.json", JsonConvert.SerializeObject(optionData));
    }

    public bool LoadSetting()
    {
        bool firstPlay = false;
        if (System.IO.File.Exists(Application.persistentDataPath + "/Settings.json"))
        {
            //만약 Serialize에 실패했다면 새로 생성
            try
            {
                optionData = JsonConvert.DeserializeObject<OptionData>(System.IO.File.ReadAllText(Application.persistentDataPath + "/Settings.json"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                optionData = new OptionData();
                SaveSetting();
            }

            firstPlay = true;
        }
        else
        {
            optionData = new OptionData();
            SaveSetting();
        }
        
        optionData.ApplySetting();
        
        return firstPlay;
    }
}
