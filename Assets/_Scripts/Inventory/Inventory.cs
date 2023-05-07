using System.Collections;
using System.Collections.Generic;
using TMPro;
using TypeDefs;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    Dictionary<int, int> tmpInventory;
    Dictionary<int, Item> dict_items;
    Dictionary<int, Sprite> dict_imgList;

    public Transform TF_Weapons;
    public Transform TF_Disposables;
    public Transform TF_Foods;
    public Transform TF_Others;

    [SerializeField] GameObject copyGO;

    void Awake()
    {
        TF_Weapons =        transform.Find("Elements").GetChild(0).Find("Scroll View").GetChild(0).GetChild(0).GetComponent<Transform>();
        TF_Disposables =    transform.Find("Elements").GetChild(1).Find("Scroll View").GetChild(0).GetChild(0).GetComponent<Transform>();
        TF_Foods =          transform.Find("Elements").GetChild(2).Find("Scroll View").GetChild(0).GetChild(0).GetComponent<Transform>();
        TF_Others =         transform.Find("Elements").GetChild(3).Find("Scroll View").GetChild(0).GetChild(0).GetComponent<Transform>();
    }

    public void LoadSetting()
    {
        tmpInventory = InventoryManager.Instance.dict_inventory;
        dict_items = InventoryManager.Instance.dict_items;
        dict_imgList = InventoryManager.Instance.dict_imgList;
    }
    void Start()
    {
        
    }

    public void UpdateInventory()
    {
        foreach (KeyValuePair<int, int> kvp in tmpInventory)
        {
            Item targetItem = dict_items[kvp.Key];
            switch (targetItem.IT_type)
            {
                case ItemType.Weapon:
                    {
                        GameObject tmpGO;

                        tmpGO = Instantiate(copyGO, TF_Weapons);
                        tmpGO.GetComponent<ItemObject>().I_item = targetItem;
                        tmpGO.name = targetItem.s_name;
                        tmpGO.GetComponent<ItemObject>().UpdateItem();

                        //Sprite tmpSPR;
                        //dict_imgList.TryGetValue(targetItem.i_id, out tmpSPR); //이미지 리스트에서 해당번호의 이미지를 가져온다.

                        //tmpGO.transform.GetChild(0).GetComponent<Image>().sprite = tmpSPR;
                        
                        //tmpGO.transform.GetChild(1).GetComponent<TMP_Text>().text = kvp.Value.ToString();
                        //tmpGO.transform.GetChild(1).gameObject.SetActive(true);
                        break;
                    }

                case ItemType.Disposable:
                    {
                        GameObject tmpGO;

                        tmpGO = Instantiate(copyGO, TF_Disposables);
                        tmpGO.GetComponent<ItemObject>().I_item = targetItem;
                        tmpGO.name = targetItem.s_name;
                        tmpGO.GetComponent<ItemObject>().UpdateItem();
                        break;
                    }
                case ItemType.Food:
                    {
                        GameObject tmpGO;

                        tmpGO = Instantiate(copyGO, TF_Foods);
                        tmpGO.GetComponent<ItemObject>().I_item = targetItem;
                        tmpGO.name = targetItem.s_name;
                        tmpGO.GetComponent<ItemObject>().UpdateItem();
                        break;
                    }
                case ItemType.Other:
                    {
                        GameObject tmpGO;

                        tmpGO = Instantiate(copyGO, TF_Others);
                        tmpGO.GetComponent<ItemObject>().I_item = targetItem;
                        tmpGO.name = targetItem.s_name;
                        tmpGO.GetComponent<ItemObject>().UpdateItem();
                        break;
                    }
                default:
                    break;
            }
        }

        CalcCellSize();
    }

    void CalcCellSize()
    {
        while (TF_Weapons.childCount < 6) Instantiate(copyGO, TF_Weapons);
        TF_Weapons.GetComponent<RectTransform>().sizeDelta = new Vector2(160 * TF_Weapons.childCount - 30, 0); //Cellsize + Spaceing * 칸 개수 만큼 칸 크기를 키워준다.
        TF_Weapons.GetComponent<RectTransform>().anchoredPosition = new Vector2(80 * TF_Weapons.childCount - 15, 0);

        while (TF_Disposables.childCount < 6) Instantiate(copyGO, TF_Disposables);
        TF_Disposables.GetComponent<RectTransform>().sizeDelta = new Vector2(160 * TF_Disposables.childCount - 30, 0);
        TF_Disposables.GetComponent<RectTransform>().anchoredPosition = new Vector2(80 * TF_Disposables.childCount - 15, 0);

        while (TF_Foods.childCount < 6) Instantiate(copyGO, TF_Foods);
        TF_Foods.GetComponent<RectTransform>().sizeDelta = new Vector2(160 * TF_Foods.childCount - 30, 0);
        TF_Foods.GetComponent<RectTransform>().anchoredPosition = new Vector2(80 * TF_Foods.childCount - 15, 0);

        while (TF_Others.childCount < 6) Instantiate(copyGO, TF_Others);
        TF_Others.GetComponent<RectTransform>().sizeDelta = new Vector2(160 * TF_Others.childCount - 30, 0);
        TF_Others.GetComponent<RectTransform>().anchoredPosition = new Vector2(80 * TF_Others.childCount - 15, 0);
    }

    public void DestroyElements()
    {
        while (TF_Weapons.childCount > 0) DestroyImmediate(TF_Weapons.GetChild(0).gameObject);
        while (TF_Disposables.childCount > 0) DestroyImmediate(TF_Disposables.GetChild(0).gameObject);
        while (TF_Foods.childCount > 0) DestroyImmediate(TF_Foods.GetChild(0).gameObject);
        while (TF_Others.childCount > 0) DestroyImmediate(TF_Others.GetChild(0).gameObject);

    }
}
