using System;
using System.Collections;
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
}

public class Weapon : Item
{
    public string s_inspectText;
    public int i_damage;
    public int i_damageRange;
    public int i_preparedSpeed;
    public float f_speedMult;
    public float f_accuracyMult;

    public Weapon(Item item)
    {
        IT_type = item.IT_type;
    }
}

public class Disposable : Item
{
    public Disposable(Item item)
    {
        IT_type = item.IT_type;
    }
}

public class Foods : Item
{
    public Foods(Item item)
    {
        IT_type = item.IT_type;
    }
}

public class Others : Item
{
    public Others(Item item)
    {
        IT_type = item.IT_type;
    }
}