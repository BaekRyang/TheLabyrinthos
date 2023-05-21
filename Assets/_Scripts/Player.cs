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
        Effect effect = null;
        if (effectType is EffectList)
        {
            effect = new Effect(duration, (EffectList)effectType, strength);
            GameManager.Instance.effectsManager.NowKnown((int)effectType);
        }
        else if (effectType is effectStats)
        {
            effect = new Effect(duration, (effectStats)effectType, strength);
            GameManager.Instance.effectsManager.NowKnown((int)effectType + Enum.GetValues(typeof(EffectList)).Length);
        }
        else
            throw new ArgumentException("Invalid Effect Type");

        effectList.Add(effect);
        
        effect.ApplyEffect();
        Debug.Log("Effct Applyed : " + effectType + "-" + duration + "turn");
        
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


