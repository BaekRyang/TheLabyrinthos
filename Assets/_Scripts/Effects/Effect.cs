using System;
using TypeDefs;

public class Effect
{
    public  EffectTypes effectType;
    public  bool        isPositive;
    private int         expirationRemain;
    private float       effectStrength;
    private Dele        endturnEffect;

    public Effect(int exp, EffectTypes paramEffectType, float strength, bool isPositive)
    {
        expirationRemain = exp;
        effectStrength   = strength;
        effectType       = paramEffectType;
        this.isPositive  = isPositive;

        if (effectType == EffectTypes.Poison)
        {
            endturnEffect = () =>
            {
                var playerStats = Player.Instance.GetPlayerStats();
                playerStats.health -= (int)effectStrength;
            };
        }
    }

    public Effect(Effect other)
    {
        expirationRemain = other.expirationRemain;
        effectStrength   = other.effectStrength;
        effectType       = other.effectType;
        isPositive       = other.isPositive;
        endturnEffect    = other.endturnEffect;
    }

    public void ApplyEffect(bool revert = false)
    {
        var playerStats = Player.Instance.GetPlayerStats();

        //revert가 true면 효과를 반대로 적용한다.
        //revert가 true로 들어왔다는것이 효과를 제거한다는 뜻이므로, isPositive를 반대로 바꿔도 됨
        if (revert)
            isPositive = !isPositive;

        switch (effectType)
        {
            case EffectTypes.MaxHealth:
                playerStats.maxHealth += isPositive ? (int)effectStrength : -(int)effectStrength;
                break;
            case EffectTypes.Speed:
                playerStats.speed += isPositive ? (int)effectStrength : -(int)effectStrength;
                break;
            case EffectTypes.Defense:
                playerStats.defense += isPositive ? (int)effectStrength : -(int)effectStrength;
                break;
            case EffectTypes.Accuracy:
                playerStats.accuracyMult += isPositive ? effectStrength : -effectStrength;
                break;
            case EffectTypes.PrepareSpeed:
                playerStats.prepareSpeed += isPositive ? (int)effectStrength : -(int)effectStrength;
                break;
            case EffectTypes.Damage:
                playerStats.damage += isPositive ? (int)effectStrength : -(int)effectStrength;
                break;
            case EffectTypes.Poison:
                break;
        }
    }

    public void RemoveEffect()
    {
        //효과를 제거할때는 그냥 간단하게 ApplyEffect(true)를 호출하면 됨
        ApplyEffect(true);
    }

    public int ConsumeTurn()
    {
        endturnEffect();
        expirationRemain--;
        if (expirationRemain == 0)
            RemoveEffect();

        return expirationRemain;
    }
}