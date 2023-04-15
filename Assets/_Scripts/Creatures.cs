using UnityEngine;

public class Creatures
{
    public Creature[] C_default;

    //변화값
    const float ATTACK_MULT = 1.125f;
    const float DEFENCE_MULT = 1.2f;
    const float HP_MULT = 1.25f;
    const float SPEED_MULT = 1.0f;
    
    //조정값
    const float ATTACK_ADJ = -0.02f;
    const float DEFENCE_ADJ = -0.01f;
    const float HP_ADJ = -0.01f;
    const float SPEED_ADJ = 0.0f;


    public Creatures()
    {
        C_default = new Creature[8];
        //초기값
        C_default[0] = new Creature(8, 3, 50.0f, 1.0f);

        //초기값을 통하여 스텟 변화계산후 적용
        for (int i = 1; i < 8; i++)
        {
            int atk = (int)(C_default[i - 1].damage * (ATTACK_MULT + ATTACK_ADJ * i));
            int def = (int)(C_default[i - 1].defense * (DEFENCE_MULT + DEFENCE_ADJ * i));
            float hp = C_default[i - 1].health * (HP_MULT + HP_ADJ * i);
            float spd = C_default[i - 1].speed * (SPEED_MULT + SPEED_ADJ * i);
            C_default[i] = new Creature(atk, def, hp, spd);
        }
    }
}

public class Creature
{
    public Creature(int atk, int def, float hp, float spd, int pspd = 0) {
        this.damage = atk;
        this.defense = def;
        this.health = hp;
        this.speed = spd;
        this.prepareSpeed = pspd;
    }

    public float health = 100.0f;
    public float speed = 1.0f;
    public int defense = 5;
    public int prepareSpeed = 0;
    public int damage = 1;
    //Sprite attackSprite = null;
}