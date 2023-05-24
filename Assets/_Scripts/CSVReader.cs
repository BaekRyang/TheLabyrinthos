using System.Collections;
using System.Collections.Generic;
using System.IO;
using TypeDefs;
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

        //LoadCraftingTable(); //Crafting이 호출함
    }

    private void WriteDelegate() //각 아이템을 Parsing 하는 코드 작성 : 사실 클래스 안에다가 만들면 더 간단한데
                                 //아이템 Parsing을 CSVReader가 전부 하도록 하기 위해서 Delegate를 통해 할당해준다.
    {
        Item.parsing += (item, dataValue, startIndex) =>
        {
            Debug.Log("Parsing 할 데이터 없음");
        };
        Weapon.parsing += (baseItem, dataValue, startIndex) =>
        {
            Weapon item = baseItem as Weapon;
            for (int i = startIndex; i < dataValue.Length; i++)
            {
                switch (i - 5)
                {
                    case 0:
                        item.i_damageRange = int.Parse(dataValue[i]);
                        break;
                    case 1:
                        item.i_damage = int.Parse(dataValue[ i]);
                        item.s_inspectText += ("ATK : " + (item.i_damage - item.i_damageRange) + " ~ " + (item.i_damage + item.i_damageRange));
                        break;
                    case 2:
                        item.f_speedMult = float.Parse(dataValue[i]);
                        AddPercentageText(ref item.s_inspectText, "SPD", item.f_speedMult);
                        break;
                    case 3:
                        item.f_accuracyMult = float.Parse(dataValue[i]);
                        AddPercentageText(ref item.s_inspectText, "ACC", item.f_accuracyMult);
                        break;
                    case 4:
                        item.i_preparedSpeed = int.Parse(dataValue[i]);
                        if (item.i_preparedSpeed > 0)
                            item.s_inspectText += "\nPPS : +" + item.i_preparedSpeed + "%";
                        break;
                    case 5:
                        item.i_maxDurability = item.i_durability = int.Parse(dataValue[i]);
                        //item.s_inspectText += "\nDUR : " + item.i_durability + "/" + item.i_maxDurability;
                        break;
                }

            }
        };
        Disposable.parsing += (item, dataValue, startIndex) =>
        {
        };
        Other.parsing += (item, dataValue, startIndex) =>
        {
        };

        parsingDelegates = new Dictionary<ItemType, Item.ParseAttributes> //Delegate들을 저장할 Dictionary 아이템을 만들때 여기의 함수를 호출한다.
        {
            { ItemType.Undefined, Item.parsing },
            { ItemType.Weapon, Weapon.parsing },
            { ItemType.Disposable, Disposable.parsing },
            { ItemType.Food, Food.parsing },
            { ItemType.Other, Other.parsing }
        };
    }

    private void LoadItems() //CSV를 읽는 기본 로직 처리
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Items");
        StringReader sReader = new StringReader(textAsset.text);

        while (true)
        {
            string data = sReader.ReadLine();                       //데이터를 한줄 읽는다.
            if (data == null) break;                                //비어있으면 break

            var data_value = data.Split(',');        //CSV파일은 ","로 구분하므로 ,로 분할하고 분할된 각 값을 배열에 저장
            if (data_value[0] == "Type") continue;                  //첫줄은 항목 유형 설명탭이므로 첫줄이면 건너뛴다. (이 CSV 파일에서는 1,1 에 Type이 있음)

            Item tmpItem = ParseItem(data_value);                   //ParseItem을 호출하여 위에서 만든 배열을 넘겨준다.
            if (tmpItem.IT_type == ItemType.Undefined) continue;    //만들어진 아이템의 종류가 Undefined 이면 정상적으로 만들어진 아이템이 아니므로 지나간다.

            InventoryManager.definedItems.Add(tmpItem.i_id, tmpItem);         //만들어진 아이템을 InventoryManager에 있는 아이템 목록을 저장하는 Dictionary에 저장한다.
            Debug.Log(tmpItem.s_name + " 등록됨");
        }
    }

    private Item ParseItem(string[] data_value)
    {
        Item tmpItem = new Item();
        for (int i = 0; i < data_value.Length; i++)                 //각 열의 데이터를 읽어서 아이템 데이터에 저장한다.
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

                default:                                                                        //아이템 공통 내용을 전부 작성했다면, 아이템 특수 내용을 작성한다. 이때 위에서 작성한 Dictionary를 사용한다.
                    Item.ParseAttributes parseAttributes = parsingDelegates[tmpItem.IT_type];   //아이템 타입에 따라서 다른 함수를 호출해준다.
                    parseAttributes?.Invoke(tmpItem, data_value, 5);                            //위 함수가 Null이 아니면 호출한다. tmpItem을 넘겨서 거기에 데이터를 저장한다.
                    i = 100;                                                                    //그리고 for문을 종료시키기 위하여 i를 늘려준다.
                    break;                                                                      //조건을 i < 5 를 써도 되지만, 그냥 이렇게 한다.
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

    public IEnumerator LoadCraftingTable()
    {
        Crafting C_crafting = InventoryManager.Instance.GO_crafting.GetComponent<Crafting>();
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
                        if (string.IsNullOrWhiteSpace(data_value[i])) //만약 빈공간이 나왔다면 = 조합 아이템이 5개 미만인 경우
                        {
                            i = 100; //for문을 더 볼필요도 없음
                            break;  
                        }

                        string[] s_recipeDetail = data_value[i].Split(".");     //(아이템 ID . 필요한 개수) 구조를 갖고있으므로 "."으로 쪼개주고.
                        C_crafting.dict_craftingTable[int.Parse(data_value[0])][int.Parse(s_recipeDetail[0])] = int.Parse(s_recipeDetail[1]); //완성 아이템을 Key로 하는 Dict에 넣어준다.
                        break;
                }
            }
        }

        yield return null;
    }

}