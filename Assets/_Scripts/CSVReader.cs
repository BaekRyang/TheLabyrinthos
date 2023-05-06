using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;



public class CSVReader : MonoBehaviour
{
    const string positiveColor = "00FF00";
    const string negativeColor = "FF0000";

    //StreamReader sReader;
    TextAsset TextAsset;
    bool endOfFile = false;


    private Dictionary<ItemType, Item.ParseAttributes> parsingDelegates;

    Item Item = new Item();
    Weapon Weapon = new Weapon();
    Disposable Disposable = new Disposable();
    Food Food = new Food();
    Other Other = new Other();

    void Start()
    {
        WriteDelegate();

        LoadItems();

        //LoadCraftingTable(); //Crafting�� ȣ����
    }

    private void WriteDelegate() //�� �������� Parsing �ϴ� �ڵ� �ۼ� : ��� Ŭ���� �ȿ��ٰ� ����� �� �����ѵ�
                                 //������ Parsing�� CSVReader�� ���� �ϵ��� �ϱ� ���ؼ� Delegate�� ���� �Ҵ����ش�.
    {
        Item.parsing += (Item item, string[] dataValue, int startIndex) =>
        {
            Debug.Log("Parsing �� ������ ����");
        };
        Weapon.parsing += (Item baseItem, string[] dataValue, int startIndex) =>
        {
            Weapon item = baseItem as Weapon;
            for (int i = 0; i < dataValue.Length - startIndex; i++)
            {
                switch (i)
                {
                    case 0:
                        item.i_damageRange = int.Parse(dataValue[startIndex + i]);
                        break;
                    case 1:
                        item.i_damage = int.Parse(dataValue[startIndex + i]);
                        item.s_inspectText += ("ATK : " + (item.i_damage - item.i_damageRange).ToString() + " ~ " + (item.i_damage + item.i_damageRange).ToString());
                        break;
                    case 2:
                        item.f_speedMult = float.Parse(dataValue[startIndex + i]);
                        AddPercentageText(ref item.s_inspectText, "SPD", item.f_speedMult);
                        break;
                    case 3:
                        item.f_accuracyMult = float.Parse(dataValue[startIndex + i]);
                        AddPercentageText(ref item.s_inspectText, "ACC", item.f_accuracyMult);
                        break;
                    case 4:
                        item.i_preparedSpeed = int.Parse(dataValue[startIndex + i]);
                        if (item.i_preparedSpeed > 0)
                            item.s_inspectText += "\nPPS : +" + item.i_preparedSpeed + "%";
                        break;
                    case 5:
                        item.i_durability = int.Parse(dataValue[startIndex + i]);
                        item.s_inspectText += "\nDUR : " + item.i_durability;
                        break;
                }

            }
        };
        Disposable.parsing += (Item item, string[] dataValue, int startIndex) =>
        {
            return;
        };
        Other.parsing += (Item item, string[] dataValue, int startIndex) =>
        {
            return;
        };

        parsingDelegates = new Dictionary<ItemType, Item.ParseAttributes> //Delegate���� ������ Dictionary �������� ���鶧 ������ �Լ��� ȣ���Ѵ�.
        {
            { ItemType.Undefined, Item.parsing },
            { ItemType.Weapon, Weapon.parsing },
            { ItemType.Disposable, Disposable.parsing },
            { ItemType.Food, Food.parsing },
            { ItemType.Other, Other.parsing }
        };
    }

    private void LoadItems() //CSV�� �д� �⺻ ���� ó��
    {
        InventoryManager iManager = GetComponent<InventoryManager>();
        TextAsset textAsset = Resources.Load<TextAsset>("Items");
        StringReader sReader = new StringReader(textAsset.text);

        while (true)
        {
            string data = sReader.ReadLine();                       //�����͸� ���� �д´�.
            if (data == null) break;                                //��������� break

            var data_value = data.Split(',');                       //CSV������ ","�� �����ϹǷ� ,�� �����ϰ� ���ҵ� �� ���� �迭�� ����
            if (data_value[0] == "Type") continue;                  //ù���� �׸� ���� �������̹Ƿ� ù���̸� �ǳʶڴ�. (�� CSV ���Ͽ����� 1,1 �� Type�� ����)

            Item tmpItem = ParseItem(data_value);                   //ParseItem�� ȣ���Ͽ� ������ ���� �迭�� �Ѱ��ش�.
            if (tmpItem.IT_type == ItemType.Undefined) continue;    //������� �������� ������ Undefined �̸� ���������� ������� �������� �ƴϹǷ� ��������.

            iManager.dict_items.Add(tmpItem.i_id, tmpItem);         //������� �������� InventoryManager�� �ִ� ������ ����� �����ϴ� Dictionary�� �����Ѵ�.
            iManager.i_itemCount++;                                 //������� ������ ������ +1
            Debug.Log(tmpItem.s_name + " ��ϵ�");
        }
    }

    private Item ParseItem(string[] data_value)
    {
        Item tmpItem = new Item();

        for (int i = 0; i < data_value.Length; i++)                 //�� ���� �����͸� �о ������ �����Ϳ� �����Ѵ�.
        {
            switch (i)
            {
                case 0:
                    tmpItem.IT_type = (ItemType)int.Parse(data_value[i]);
                    tmpItem = CreateSpecificItem(tmpItem);
                    break;
                case 1:
                    tmpItem.i_id = int.Parse(data_value[i]);
                    break;
                case 2:
                    tmpItem.s_name = data_value[i];
                    break;
                case 3:
                    tmpItem.s_description = data_value[i];
                    break;
                case 4:
                    tmpItem.b_useable = bool.Parse(data_value[i]);
                    break;

                default:                                                                        //������ ���� ������ ���� �ۼ��ߴٸ�, ������ Ư�� ������ �ۼ��Ѵ�. �̶� ������ �ۼ��� Dictionary�� ����Ѵ�.
                    Item.ParseAttributes parseAttributes = parsingDelegates[tmpItem.IT_type];   //������ Ÿ�Կ� ���� �ٸ� �Լ��� ȣ�����ش�.
                    parseAttributes?.Invoke(tmpItem, data_value, 5);                            //�� �Լ��� Null�� �ƴϸ� ȣ���Ѵ�. tmpItem�� �Ѱܼ� �ű⿡ �����͸� �����Ѵ�.
                    i = 100;                                                                    //�׸��� for���� �����Ű�� ���Ͽ� i�� �÷��ش�.
                    break;                                                                      //������ i < 5 �� �ᵵ ������, �׳� �̷��� �Ѵ�.
            }
        }

        return tmpItem;
    }

    private Item CreateSpecificItem(Item item)
    {
        switch (item.IT_type)
        {
            case ItemType.Weapon:
                return new Weapon(item);
            case ItemType.Disposable:
                return new Disposable(item);
            case ItemType.Food:
                return new Food(item);
            case ItemType.Other:
                return new Other(item);
            default:
                return item;
        }
    }

    private void AddPercentageText(ref string inspectText, string attributeName, float multiplier)
    {
        if (multiplier != 1)
        {
            string color = multiplier < 1 ? negativeColor : positiveColor;
            inspectText += $"\n{attributeName} : <color=#{color}>{(multiplier > 1 ? "+" : "")}{(multiplier * 100 - 100):F0}%</color>";
        }
    }

    public void LoadCraftingTable()
    {
        Crafting C_crafting = GetComponent<Crafting>();
        TextAsset textAsset = Resources.Load<TextAsset>("Craftings");
        StringReader sReader = new StringReader(textAsset.text);

        while (true)
        {
            string data = sReader.ReadLine();                      
            if (data == null) break;                               

            var data_value = data.Split(',');                      
            if (data_value[0] == "CraftedID") continue;

            for (int i = 0; i < data_value.Length; i++) {
                switch (i)
                {
                    case 0:
                        C_crafting.dict_craftingTable[int.Parse(data_value[0])] = new Dictionary<int, int>();
                        break;

                    default:
                        if (string.IsNullOrWhiteSpace(data_value[i])) //���� ������� ���Դٸ� = ���� �������� 5�� �̸��� ���
                        {
                            i = 100; //for���� �� ���ʿ䵵 ����
                            break;  
                        }

                        string[] s_recipeDetail = data_value[i].Split(".");     //(������ ID . �ʿ��� ����) ������ ���������Ƿ� "."���� �ɰ��ְ�.
                        C_crafting.dict_craftingTable[int.Parse(data_value[0])][int.Parse(s_recipeDetail[0])] = int.Parse(s_recipeDetail[1]); //�ϼ� �������� Key�� �ϴ� Dict�� �־��ش�.
                        break;                                                                      
                }
            }
        }
    }

}