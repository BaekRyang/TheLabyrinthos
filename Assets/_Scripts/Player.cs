using System;
using System.Collections.Generic;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    
    public PlayerStats  PS_playerStats;
    public Weapon       WP_weapon;
    public List<Effect> effectList = new List<Effect>();
    
    public int   defaultExp = 5;
    public float defaultStr = 1.1f;
    void Awake()
    {
        PS_playerStats = new PlayerStats();

        Instance ??= this;
    }

    public ref PlayerStats GetPlayerStats()
    {
        return ref PS_playerStats;
    }

    public void AddEffect(ref Effect originEffect)
    {
        //해당 effectType을 가진 effect를 복사 - 남은 시간 같은 요소를 분리하기 위해
        Effect effect = new Effect(originEffect); 
        
        //아이템을 사용했으므로 Known상태로 만들어준다.
        GameManager.Instance.effectsManager.NowKnown(effect.effectType, effect.isPositive);  

        effectList.Add(effect);
        
        effect.ApplyEffect();
        Debug.Log("Effect Applied : " + effect.effectType);
        
        InventoryManager.Instance.stats.UpdateUI();
    }

    public void ConsumeTurn()
    {
        if (effectList.IsUnityNull())
            return;

        foreach (var effect in effectList)
        {
            //매턴이 끝날때 리스트에 있는 이펙트들의 턴종료 효과를 발동하고, 만료된 이펙트를 지워준다.
            //리스트에 담겨있지 않으면 이펙트는 무효과임
            var remain = effect.ConsumeTurn();
            if (remain == 0)
                effectList.Remove(effect);
        }
    }
}


