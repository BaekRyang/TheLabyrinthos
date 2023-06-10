using System;
using TypeDefs;
using UnityEngine;


[Serializable]
public class Item
{
    public int i_id;                  //아이템 식별용 아이디
    public string s_name;           //아이템 이름
    public string s_description;    //아이템 설명
    public ItemType IT_type;        //아이템 타입
    public bool b_useable;

    public delegate void ParseAttributes(Item item, string[] dataValue); //CSV에서 파일을 읽어서 각 멤버 변수를 초기화 하는 코드가 들어갈 자리
    public ParseAttributes parsing;
}

[Serializable]
public class Weapon : Item
{
    public string s_inspectText;
    public int i_damage;
    public int i_damageRange;
    public int i_preparedSpeed;
    public float f_speedMult;
    public float f_accuracyMult;
    public int i_maxDurability;
    public int i_durability;
    public Weapon(Item item)
    {
        IT_type = item.IT_type;
    }

    public Weapon()
    {
        IT_type = ItemType.Weapon;
    }

    public Weapon(Weapon other) //복사 생성자
    {
        i_id            = other.i_id;
        s_name          = other.s_name;
        s_description   = other.s_description;
        IT_type         = other.IT_type;
        b_useable       = other.b_useable;

        s_inspectText   = other.s_inspectText;
        i_damage        = other.i_damage;
        i_damageRange   = other.i_damageRange;
        i_preparedSpeed = other.i_preparedSpeed;
        f_speedMult     = other.f_speedMult;
        f_accuracyMult  = other.f_accuracyMult;
        i_maxDurability = other.i_maxDurability;
        i_durability    = other.i_durability;
    }
    
    public void ConsumeDurability(int amount = 1)
    {
        i_durability -= amount;
        if (i_durability <= 0)
        {
            var IM = InventoryManager.Instance;
            IM.RemoveItem(this);
            Player.Instance.WP_weapon = IM.GetWeaponInstance(0);
        }
    }
}

public class Disposable : Item
{
    public Effect effect;
    public Color  color;
    public short  effectID;

    public float restoreHp;
    public float restoreHpPercent;
    public float restoreHpLossPercent;
    
    public Dele  dele_itemEffect; //아이템 효과(함수로)
    public Disposable(Item item)
    {
        IT_type = item.IT_type;
    }

    public Disposable()
    {
        IT_type = ItemType.Disposable;
    }

    public Disposable(Disposable other)
    {
        s_name          = other.s_name;
        s_description   = other.s_description;
        IT_type         = other.IT_type;
        b_useable       = other.b_useable;
        dele_itemEffect = other.dele_itemEffect;
    }

    public void CreateEffect(EffectTypes type, bool isPositive)
    {
        effect          = new Effect(5, type, 5, isPositive);
        dele_itemEffect = () => Player.Instance.AddEffect(ref effect);
        dele_itemEffect += () => InventoryManager.Instance.effectIndicator.UpdateUI();
        dele_itemEffect += () => GameManager.Instance.UpdateStatsSlider(StatsType.Hp);
    }
}

public class Food : Disposable
{
    public Food(Item item)
    {
        IT_type = item.IT_type;
    }

    public Food()
    {
        IT_type = ItemType.Food;
    }
}

public class Other : Item
{
    public Other(Item item)
    {
        IT_type = item.IT_type;
    }
    public Other()
    {
        IT_type = ItemType.Disposable;
    }
}