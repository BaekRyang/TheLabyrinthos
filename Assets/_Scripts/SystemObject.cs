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

    public bool SaveData()
    {
        SaveData saveData = new SaveData();

        foreach (var (index, roomNode) in GameManager.Instance.GetComponent<RoomCreation>().roomMap)
        {
            if (!saveData.roomVisited.Contains(index)) //방문 안한방은 저장 안해도됨
                continue;

            saveData.interacted.Add(index, new Dictionary<int, bool>()); //방문은 했으니깐 안에 오브젝트들의 상태를 저장해야함
            
            var interactables = roomNode.RoomObject.transform.GetComponentsInChildren<Interactable>(); //Interactable 컴포넌트를 가진 오브젝트들을 가져옴
            
            for (var i = 0; i < interactables.Length; i++) //오브젝트들을 돌면서 상태를 저장
            {
                var  interactable = interactables[i];
                bool interacted   = false;

                if (interactable.type == ObjectType.Door || interactable.type == ObjectType.Item)
                {
                    //Interact를 했던 오브젝트들만 저장 (일반 문과 아이템만 저장)
                    if (interactable.interacted)
                        interacted = true;
                }
                saveData.interacted[index].Add(i, interacted);
            }
        }
        
        try
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/Data.json", JsonConvert.SerializeObject(saveData));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Save Failed");
            return false;
        }
        
        return true;
    }
}
