using System;
using TypeDefs;

public delegate void Dele();

[Serializable]
public class Item
{
    public int i_id;                  //아이템 식별용 아이디
    public string s_name;           //아이템 이름
    public string s_description;    //아이템 설명
    public ItemType IT_type;        //아이템 타입
    public bool b_useable;

    public Dele dele_itemEffect;    //아이템 효과(함수로)

    public delegate void ParseAttributes(Item item, string[] dataValue, int startIndex); //CSV에서 파일을 읽어서 각 멤버 변수를 초기화 하는 코드가 들어갈 자리
    public ParseAttributes parsing;
}

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
        i_id = other.i_id;
        s_name = other.s_name;
        s_description = other.s_description;
        IT_type = other.IT_type;
        b_useable = other.b_useable;
        dele_itemEffect = other.dele_itemEffect;

        s_inspectText = other.s_inspectText;
        i_damage = other.i_damage;
        i_damageRange = other.i_damageRange;
        i_preparedSpeed = other.i_preparedSpeed;
        f_speedMult = other.f_speedMult;
        f_accuracyMult = other.f_accuracyMult;
        i_maxDurability = other.i_maxDurability;
        i_durability = other.i_durability;
    }
}

public class Disposable : Item
{
    public Disposable(Item item)
    {
        IT_type = item.IT_type;
    }

    public Disposable()
    {
        IT_type = ItemType.Disposable;
    }
}

public class Food : Item
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