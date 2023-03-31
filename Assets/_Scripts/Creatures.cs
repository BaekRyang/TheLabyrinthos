public class Creatures
{
    public Creature[] C_Default;

    //변화값
    const float f_Multipler_ATK = 1.34f;
    const float f_Multipler_DEF = 1.2f;
    const float f_Multipler_HP = 1.25f;
    const float f_Multipler_SPD = 1.0f;
    
    //조정값
    const float f_Adjustment_ATK = -0.02f;
    const float f_Adjustment_DEF = 0.01f;
    const float f_Adjustment_HP = -0.01f;
    const float f_Adjustment_SPD = 0.0f;


    public Creatures()
    {
        C_Default = new Creature[8];
        //초기값
        C_Default[0] = new Creature(10, 3, 50.0f, 1.0f);

        //초기값을 통하여 스텟 변화계산후 적용
        for (int i = 1; i < 8; i++)
        {
            int atk = (int)(C_Default[i - 1].i_AttackDamage * (f_Multipler_ATK + f_Adjustment_ATK * i));
            int def = (int)(C_Default[i - 1].i_Defense * (f_Multipler_DEF + f_Adjustment_DEF * i));
            float hp = C_Default[i - 1].f_Health * (f_Multipler_HP + f_Adjustment_HP * i);
            float spd = C_Default[i - 1].f_Speed * (f_Multipler_SPD + f_Adjustment_SPD * i);
            C_Default[i] = new Creature(atk, def, hp, spd);
        }
    }
}

public class Creature
{
    public Creature(int atk, int def, float hp, float spd, int pspd = 0) {
        this.i_AttackDamage = atk;
        this.i_Defense = def;
        this.f_Health = hp;
        this.f_Speed = spd;
        this.i_PrepareSpeed = pspd;
    }

    public int i_AttackDamage = 0;
    public int i_Defense = 0;
    public float f_Health = 0.0f;
    public float f_Speed = 1.0f;
    public int i_PrepareSpeed = 0;
}