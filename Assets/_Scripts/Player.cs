using System;
using System.Collections.Generic;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStats  PS_playerStats;
    public Weapon       WP_weapon;
    public List<Effect> effectList = new List<Effect>();
    
    public int   defaultExp = 5;
    public float defaultStr = 1.1f;
    void Awake()
    {
        PS_playerStats = new PlayerStats();
    }

    public ref PlayerStats GetPlayerStats()
    {
        return ref PS_playerStats;
    }

    public void AddEffect(object effectType, int duration, float strength)
    {
        Effect effect;
        if (effectType is EffectList list) //effectType이 EffectList라면 EffectList인 list에 저장
        {
            effect = new Effect(duration, list, strength); //해당 effectType을 가진 effect를 생성
            GameManager.Instance.effectsManager.NowKnown((int)list); //아이템을 사용했으므로 Known상태로 만들어준다.
        }
        else if (effectType is effectStats type) //마찬가지로 effectType이 effectStats라면 effectStats인 type에 저장
        {
            effect = new Effect(duration, type, strength);
            GameManager.Instance.effectsManager.NowKnown((int)type + Enum.GetValues(typeof(EffectList)).Length);
        }
        else
            throw new ArgumentException("Invalid Effect Type");

        effectList.Add(effect);
        
        effect.ApplyEffect();
        Debug.Log("Effect Applied : " + effectType + "-" + duration + "turn");
        
        InventoryManager.Instance.stats.UpdateUI();
    }
    
    public void AddEffect(object effectType)
    {
        AddEffect(effectType, defaultExp, defaultStr);
    }


    public void ConsumeTurn()
    {
        if (effectList.IsUnityNull())
            return;

        foreach (var effect in effectList)
        {
            var remain = effect.ConsumeTurn();
            if (remain == 0)
                effectList.Remove(effect);
        }
    }
}


