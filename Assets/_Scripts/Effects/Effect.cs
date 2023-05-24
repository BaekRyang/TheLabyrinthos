using TypeDefs;

public class Effect
{
    public  EffectTypes effectType;
    private int         expirationRemain;
    private effectStats statsEffect;
    private float       effectStrength;
    private Dele        endturnEffect;

    public Effect(int exp, effectStats effectType, float strength)
    {
        expirationRemain = exp;
        statsEffect = effectType;
        effectStrength = strength;
    }

    public Effect(int exp, EffectList effectType, float strength)
    {
        var playerStats = Player.Instance.GetPlayerStats();
        expirationRemain = exp;
        effectStrength = strength;

        switch (effectType)
        {
            case EffectList.Poison:
                endturnEffect = () =>
                {
                    playerStats.health -= effectStrength;
                };
                break;
        }
    }

    public void ApplyEffect()
    {
        var playerStats = Player.Instance.GetPlayerStats();
        switch (statsEffect)
        {
            case effectStats.Damage:
                effectType = EffectTypes.DamageUp;
                playerStats.damage += (int)effectStrength;
                break;

            case effectStats.Defense:
                effectType = EffectTypes.DefenseUp;
                playerStats.defense += (int)effectStrength;
                break;

            case effectStats.Speed:
                effectType = EffectTypes.SpeedUp;
                playerStats.speed += effectStrength;
                break;

            case effectStats.Accuracy:
                effectType = EffectTypes.AccuracyUp;
                playerStats.accuracyMult += effectStrength;
                break;

            case effectStats.MaxHealth:
                effectType = EffectTypes.MaxHealthUp;
                playerStats.maxHealth += (int)effectStrength;
                break;

            case effectStats.PrepareSpeed:
                effectType = EffectTypes.PrepareSpeedUp;
                playerStats.prepareSpeed += (int)effectStrength;
                break;
        }
    }

    public void RemoveEffect()
    {
        var playerStats = GameManager.Instance.GetComponent<Player>().GetPlayerStats();
        switch (statsEffect)
        {
            case effectStats.Damage:
                playerStats.damage -= (int)effectStrength;
                break;

            case effectStats.Defense:
                playerStats.defense -= (int)effectStrength;
                break;

            case effectStats.Speed:
                playerStats.speed -= effectStrength;
                break;

            case effectStats.Accuracy:
                playerStats.accuracyMult -= effectStrength;
                break;

            case effectStats.MaxHealth:
                playerStats.maxHealth -= (int)effectStrength;
                break;

            case effectStats.PrepareSpeed:
                playerStats.prepareSpeed -= (int)effectStrength;
                break;
        }
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