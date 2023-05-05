using System;
using System.Collections;
using TypeDefs;

public delegate void Dele();

[Serializable]
public class Item
{
    public int i_id;                  //������ �ĺ��� ���̵�
    public string s_name;           //������ �̸�
    public string s_description;    //������ ����
    public ItemType IT_type;        //������ Ÿ��
    public bool b_useable;
    public Dele dele_itemEffect;    //������ ȿ��(�Լ���)

    public delegate void ParseAttributes(Item item, string[] dataValue, int startIndex); //CSV���� ������ �о �� ��� ������ �ʱ�ȭ �ϴ� �ڵ尡 �� �ڸ�
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
    public int i_durability;
    public Weapon(Item item)
    {
        IT_type = item.IT_type;
    }

    public Weapon()
    {
        IT_type = ItemType.Weapon;
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