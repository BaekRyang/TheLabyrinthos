using System;
using System.IO;
using System.Text.RegularExpressions;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;



public class CSVReader : MonoBehaviour
{
    const string positiveColor = "00FF00";
    const string negativeColor = "FF0000";

    //StreamReader sReader;
    TextAsset ItemFile;
    bool endOfFile = false;

    [SerializeField] Item tmpItem;
    Weapon tmpWeapon;

    void Start()
    {
        InventoryManager iManager = GetComponent<InventoryManager>();
        ItemFile = Resources.Load<TextAsset>("Items");
        StringReader sReader = new StringReader(ItemFile.text);

        while (!endOfFile)
        {
            string data = sReader.ReadLine();
            if (data == null)
            {
                endOfFile = true;
                break;
            }
            var data_value = data.Split(',');

            for (int i = 0; i < data_value.Length; i++)
            {
                
                if (data_value[i] == "Type") break;
                switch (i)
                {
                    case 0:
                        tmpItem.IT_type = (ItemType)int.Parse(data_value[i]);
                        switch (tmpItem.IT_type)
                        {
                            case ItemType.Weapon:
                                tmpItem = new Weapon(tmpItem);
                                break;
                            case ItemType.Disposable:
                                tmpItem = new Disposable(tmpItem);
                                break;
                            case ItemType.Food:
                                tmpItem = new Foods(tmpItem);
                                break;
                            case ItemType.Other:
                                tmpItem = new Others(tmpItem);
                                break;
                            default:
                                break;
                        }
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
                }

                if (tmpItem.IT_type == ItemType.Weapon)
                {
                    tmpWeapon = tmpItem as Weapon; //무기로 바꾸기 위해 캐스팅
                    switch (i)
                    {
                        case 5:
                            tmpWeapon.i_damageRange = int.Parse(data_value[i]);
                            break;

                        case 6:
                            tmpWeapon.i_damage = int.Parse(data_value[i]);
                            tmpWeapon.s_inspectText += ("ATK : " + (tmpWeapon.i_damage - tmpWeapon.i_damageRange).ToString() + " ~ " + (tmpWeapon.i_damage + tmpWeapon.i_damageRange).ToString())/*.Replace("\\n", "\n")*/;
                            break;

                        case 7:
                            tmpWeapon.f_speedMult = float.Parse(data_value[i]);
                            if (tmpWeapon.f_speedMult != 1)
                                tmpWeapon.s_inspectText += "\nSPD : <color=#" + (tmpWeapon.f_speedMult < 1 ? negativeColor + ">" : positiveColor + ">+") + (tmpWeapon.f_speedMult * 100 - 100) + "%</color>";
                            break;

                        case 8:
                            tmpWeapon.f_accuracyMult = float.Parse(data_value[i]);
                            if (tmpWeapon.f_accuracyMult != 1)
                                tmpWeapon.s_inspectText += "\nACC : <color=#" + (tmpWeapon.f_accuracyMult < 1 ? negativeColor + ">" : positiveColor + ">+") + (tmpWeapon.f_accuracyMult * 100 - 100) + "%</color>";
                            break;

                        case 9:
                            tmpWeapon.i_preparedSpeed = int.Parse(data_value[i]);
                            if (tmpWeapon.i_preparedSpeed > 0)
                                tmpWeapon.s_inspectText += "\nPPS : " + (tmpWeapon.i_preparedSpeed > 0 ? "+" + tmpWeapon.i_preparedSpeed + "%" : "");
                            break;

                        default:
                            break;
                    }
                    tmpItem = tmpWeapon; //tmpWeapon에 설정된 값을 tmpItem에 적용
                }

            }

            if (tmpItem.IT_type == ItemType.Undefined) continue;
            iManager.dict_items.Add(tmpItem.i_id, tmpItem); //Id - Item 구조로 등록
            iManager.i_itemCount++;
            Debug.Log(tmpItem.s_name + " 등록됨");
        }
    }

    void Update()
    {

    }
}