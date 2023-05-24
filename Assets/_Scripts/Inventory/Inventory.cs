using System.Collections.Generic;
using TypeDefs;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Transform TF_Weapons;
    public Transform TF_Disposables;
    public Transform TF_Foods;
    public Transform TF_Others;
    
    int maxChildCount = 6;
    int cellSize      = 160;
    int spacing       = 20;
    int offset        = 80;
    
    [SerializeField] GameObject copyGO;

    void Awake()
    {
        TF_Weapons     = transform.Find("Elements").GetChild(0).Find("Scroll View").GetChild(0).GetChild(0).GetComponent<Transform>();
        TF_Disposables = transform.Find("Elements").GetChild(1).Find("Scroll View").GetChild(0).GetChild(0).GetComponent<Transform>();
        TF_Foods       = transform.Find("Elements").GetChild(2).Find("Scroll View").GetChild(0).GetChild(0).GetComponent<Transform>();
        TF_Others      = transform.Find("Elements").GetChild(3).Find("Scroll View").GetChild(0).GetChild(0).GetComponent<Transform>();
    }

    void Start()
    {
    }

    public void UpdateInventory(byte target = 15, bool destroyElements = true)
    {
        if (destroyElements)
            DestroyElements();

        if ((target & (1 << 3)) != 0)
        {
            foreach (var weaponList in InventoryManager.weaponInventory)
            {
                foreach (var weapon in weaponList.Value)
                {
                    GameObject tmpGO = Instantiate(copyGO, TF_Weapons);
                    tmpGO.GetComponent<ItemObject>().I_item = weapon;
                    tmpGO.name                              = weapon.s_name;
                    tmpGO.GetComponent<ItemObject>().UpdateItem();
                }
            }
        }


        foreach (KeyValuePair<int, int> kvp in InventoryManager.inventory)
        {
            Item targetItem = InventoryManager.definedItems[kvp.Key];
            switch (targetItem.IT_type)
            {
                case ItemType.Weapon:
                    {
                        //GameObject tmpGO;

                        //tmpGO = Instantiate(copyGO, TF_Weapons);
                        //tmpGO.GetComponent<ItemObject>().I_item = targetItem;
                        //tmpGO.name = targetItem.s_name;
                        //tmpGO.GetComponent<ItemObject>().UpdateItem(kvp.Value);
                        break;
                    }

                case ItemType.Disposable:
                    {
                        if ((target & (1 << 2)) != 0)
                        {
                            GameObject tmpGO = Instantiate(copyGO, TF_Disposables);
                            tmpGO.GetComponent<ItemObject>().I_item = targetItem;
                            tmpGO.name                              = targetItem.s_name;
                            tmpGO.GetComponent<ItemObject>().UpdateItem(kvp.Value);
                        }

                        break;
                    }
                case ItemType.Food:
                    {
                        if ((target & (1 << 1)) != 0)
                        {
                            GameObject tmpGO = Instantiate(copyGO, TF_Foods);
                            tmpGO.GetComponent<ItemObject>().I_item = targetItem;
                            tmpGO.name                              = targetItem.s_name;
                            tmpGO.GetComponent<ItemObject>().UpdateItem(kvp.Value);
                        }

                        break;
                    }
                case ItemType.Other:
                    {
                        if ((target & (1 << 0)) != 0)
                        {
                            GameObject tmpGO = Instantiate(copyGO, TF_Others);
                            tmpGO.GetComponent<ItemObject>().I_item = targetItem;
                            tmpGO.name                              = targetItem.s_name;
                            tmpGO.GetComponent<ItemObject>().UpdateItem(kvp.Value);
                        }

                        break;
                    }
            }
        }

        CalcCellSize();
    }

    void CalcCellSize()
    {
        UpdateLayout(TF_Weapons);
        UpdateLayout(TF_Disposables);
        UpdateLayout(TF_Foods);
        UpdateLayout(TF_Others);
    }
    
    void UpdateLayout(Transform transform)
    {
        while (transform.childCount < maxChildCount)
            Instantiate(copyGO, transform);

        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.sizeDelta        = new Vector2(cellSize * transform.childCount - spacing,           0);
        rectTransform.anchoredPosition = new Vector2(cellSize / 2 * transform.childCount - (spacing / 2), 0);
    }

    public void DestroyElements()
    {
        while (TF_Weapons.childCount > 0) DestroyImmediate(TF_Weapons.GetChild(0).gameObject);
        while (TF_Disposables.childCount > 0) DestroyImmediate(TF_Disposables.GetChild(0).gameObject);
        while (TF_Foods.childCount > 0) DestroyImmediate(TF_Foods.GetChild(0).gameObject);
        while (TF_Others.childCount > 0) DestroyImmediate(TF_Others.GetChild(0).gameObject);
    }
}