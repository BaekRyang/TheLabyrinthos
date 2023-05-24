using System;
using System.Collections.Generic;
using TypeDefs;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public List<EffectTypes> knownEffects;

    public bool IsKnown(EffectTypes ID)
    {
        if (knownEffects.Contains(ID))
            return true;
        return false;
    }

    public void NowKnown(EffectTypes ID)
    {
        //ID가 knownEffects에 없다면 추가
        if (!knownEffects.Contains(ID))
            knownEffects.Add(ID);

    }

    public string GetEffectDesc(Effect effect)
    {
        //ID에 해당하는 Effect의 설명을 반환한다.
        //EffectType에 따라서 설명을 출력하고, isPositive에 따라 증가인지 감소인지 표시
        //두 문자열을 더해서 더한 문자열을 반환한다.
        string desc = "";
        switch (effect.effectType)
        {
            case EffectTypes.MaxHealth:
                desc = "최대 체력 ";
                break;
            case EffectTypes.Speed:
                desc = "이동속도 ";
                break;
            case EffectTypes.Defense:
                desc = "방어력 ";
                break;
            case EffectTypes.Accuracy:
                desc = "명중률 ";
                break;
            case EffectTypes.PrepareSpeed:
                desc = "기민함 ";
                break;
            case EffectTypes.Damage:
                desc = "공격력 ";
                break;
            case EffectTypes.Poison:
                desc = "중독";
                return desc; //중독은 언제나 부정이므로 isPositive를 체크하지 않는다.
        }
        
        desc += effect.isPositive ? "증가 " : "감소 ";

        return desc;
    }

    public Sprite GetEffectIcon(EffectTypes effType)
    {
        return null;
    }
}
