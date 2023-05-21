using System;
using System.Collections.Generic;
using TypeDefs;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public List<int> knownEffects;

    public bool IsKnown(int ID)
    {
        if (knownEffects.Contains(ID))
            return true;
        return false;
    }

    public void NowKnown(int ID)
    {
        knownEffects.Add(ID);
    }

    public string GetEffectDesc(int ID)
    {
        if (ID >= Enum.GetValues(typeof(EffectList)).Length)
        {
            switch ((effectStats)ID-1)
            {
                case effectStats.MaxHealth:
                    return "최대 체력 증가";
                case effectStats.Speed:
                    return "속도 증가";
                case effectStats.Defense:
                    return "방어력 증가";
                case effectStats.Accuracy:
                    return "명중률 증가";
                case effectStats.PrepareSpeed:
                    return "기민함 증가";
                case effectStats.Damage:
                    return "공격력 증가";
            }
        }
        else
        {
            switch ((EffectList)ID)
            {
                case EffectList.poision:
                    return "중독";
            }
        }

        return "";
    }
}
