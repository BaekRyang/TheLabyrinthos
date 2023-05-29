using System;
using System.Collections.Generic;
using TypeDefs;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public  List<int>                  knownEffects;
    public  Sprite[]                   effectIconsArray;
    private Dictionary<string, Sprite> effectIcons = new Dictionary<string, Sprite>();
    public  Sprite                     effectIconsNegative;
    public  Sprite                     effectIconsPositive;


    private void Start()
    {
        //KeyValue로 쓰기위해 변환해준다.
        foreach (var sprite in effectIconsArray)
            effectIcons.Add(sprite.name, sprite);
        
    }
    

    public bool IsKnown(EffectTypes ID, bool isPositive)
    {
        if (knownEffects.Contains((int)ID * 10 + (isPositive ? 1 : 0)))
            return true;
        return false;
    }

    public void NowKnown(EffectTypes ID, bool isPositive)
    {
        //ID가 knownEffects에 없다면 추가
        if (!knownEffects.Contains((int)ID * 10 + (isPositive ? 1 : 0)))
            knownEffects.Add((int)ID * 10 + (isPositive ? 1 : 0));
    }

    public string GetEffectDesc(Effect effect, bool showLevel = false)
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

        desc += effect.isPositive ? "증가" : "감소";
        desc += showLevel ? " " + effect.effectStrength : "";

        return desc;
    }

    public string GetEffectDetails(Effect effect)
    {
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
                desc = "매 턴마다 체력을 " + effect.effectStrength + "만큼 감소시킨다.";
                return desc;
        }

        desc += effect.isPositive ? "+ " : "- ";
        desc += effect.effectStrength;

        if (effect.effectType == EffectTypes.Accuracy || effect.effectType == EffectTypes.Speed)
            desc += "%";

        return desc;
    }

    public Sprite GetEffectIcon(EffectTypes effType)
    {
        return effectIcons[effType.ToString()];
    }
}