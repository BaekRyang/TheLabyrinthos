using System;
using TypeDefs;

[Serializable]
public class Effect
{
    public  EffectTypes effectType;
    public  bool        isPositive;
    public  int         expirationRemain;
    public  int         effectStrength;
    private Dele        endturnEffect;

    public Effect(int exp, EffectTypes paramEffectType, int strength, bool isPositive)
    {
        expirationRemain = exp;
        effectStrength   = strength;
        effectType       = paramEffectType;
        this.isPositive  = isPositive;

        if (effectType == EffectTypes.Poison)
        {
            endturnEffect = () =>
            {
                var playerStats = Player.Instance.PS_playerStats;
                playerStats.Health -= effectStrength;
                GameManager.Instance.UpdateStatsSlider(StatsType.Hp);
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
        var playerStats = Player.Instance.PS_playerStats;

        //revert가 true면 효과를 반대로 적용한다.
        //revert가 true로 들어왔다는것이 효과를 제거한다는 뜻이므로, isPositive를 반대로 바꿔도 됨
        if (revert)
            isPositive = !isPositive;

        switch (effectType)
        {
            case EffectTypes.MaxHealth:
                playerStats.MaxHealth += isPositive ? effectStrength : -effectStrength;
                break;
            case EffectTypes.Speed:
                playerStats.Speed += isPositive ? effectStrength * 0.01f : -effectStrength * 0.01f;
                break;
            case EffectTypes.Defense:
                playerStats.Defense += isPositive ? effectStrength : -effectStrength;
                break;
            case EffectTypes.Accuracy:
                playerStats.AccuracyMult += isPositive ? effectStrength * 0.01f : -effectStrength * 0.01f;
                break;
            case EffectTypes.PrepareSpeed:
                playerStats.PrepareSpeed += isPositive ? effectStrength : -effectStrength;
                break;
            case EffectTypes.Damage:
                playerStats.Damage += isPositive ? effectStrength : -effectStrength;
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
        endturnEffect?.Invoke();
        expirationRemain--;
        if (expirationRemain == 0)
            RemoveEffect();

        return expirationRemain;
    }
}