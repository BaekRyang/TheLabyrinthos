using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TypeDefs;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int i_inventoryCount; //인스펙터 확인용
    [SerializeField] public int i_itemCount; //인스펙터 확인용


    //인벤토리 key, value 형식으로 사용
    Dictionary<int, int> dict_inventory;
    Dictionary<int, Sprite> dict_imgList = new Dictionary<int, Sprite>();
    public Dictionary<int, Item> dict_items;

    [SerializeField] Transform TF_Weapons;
    [SerializeField] Transform TF_Disposables;
    [SerializeField] Transform TF_Foods;
    [SerializeField] Transform TF_Others;


    [SerializeField] GameObject copyGO;
    private void Awake()
    {
        dict_items = new Dictionary<int, Item>();
        dict_inventory = new Dictionary<int, int>();

        var tmpArray = Resources.LoadAll<Sprite>("Sprites/Items");
        for (int i = 0; i < tmpArray.Length; i++)
        {
            dict_imgList.Add(int.Parse(tmpArray[i].name), tmpArray[i]);
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        i_inventoryCount = dict_inventory.Count;
    }

    //public void UpdateInventory()
    //{
    //    var tmpInventory = dict_inventory;
    //    foreach (KeyValuePair<Item, int> kvp in tmpInventory)
    //    {

    //        switch (kvp.Key.IT_type)
    //        {
    //            case TypeDefs.ItemType.Weapon:
    //                for (int i = 0; i < TF_Weapons.childCount; i++)
    //                {
    //                    GameObject tmpGO = TF_Weapons.GetChild(i).gameObject;
    //                    if (tmpGO.name != "None")
    //                    {
    //                        tmpGO.GetComponent<ItemObject>().I_item = kvp.Key;

    //                        Sprite tmpSPR;
    //                        dict_imgList.TryGetValue(kvp.Key.SPR_itemSprite, out tmpSPR);

    //                        tmpGO.transform.GetChild(0).GetComponent<Image>().sprite = tmpSPR;
    //                        tmpGO.name = kvp.Key.s_name;
    //                    }
    //                }
    //                break;
    //            case TypeDefs.ItemType.Disposable:
    //                for (int i = 0; i < TF_Disposables.childCount; i++)
    //                {
    //                    GameObject tmpGO = TF_Disposables.GetChild(i).gameObject;
    //                    if (tmpGO.name != "None")
    //                    {
    //                        tmpGO.GetComponent<ItemObject>().I_item = kvp.Key;

    //                        Sprite tmpSPR;
    //                        dict_imgList.TryGetValue(kvp.Key.SPR_itemSprite, out tmpSPR);

    //                        tmpGO.transform.GetChild(0).GetComponent<Image>().sprite = tmpSPR;
    //                        tmpGO.name = kvp.Key.s_name;
    //                    }
    //                }
    //                break;
    //            case TypeDefs.ItemType.Food:
    //                for (int i = 0; i < TF_Foods.childCount; i++)
    //                {
    //                    GameObject tmpGO = TF_Foods.GetChild(i).gameObject;
    //                    if (tmpGO.name != "None")
    //                    {
    //                        tmpGO.GetComponent<ItemObject>().I_item = kvp.Key;

    //                        Sprite tmpSPR;
    //                        dict_imgList.TryGetValue(kvp.Key.SPR_itemSprite, out tmpSPR);

    //                        tmpGO.transform.GetChild(0).GetComponent<Image>().sprite = tmpSPR;
    //                        tmpGO.name = kvp.Key.s_name;
    //                    }
    //                }
    //                break;
    //            case TypeDefs.ItemType.Other:
    //                for (int i = 0; i < TF_Others.childCount; i++)
    //                {
    //                    GameObject tmpGO = TF_Others.GetChild(i).gameObject;
    //                    if (tmpGO.name == "None")
    //                    {
    //                        tmpGO.GetComponent<ItemObject>().I_item = kvp.Key;

    //                        Sprite tmpSPR;
    //                        dict_imgList.TryGetValue(kvp.Key.SPR_itemSprite, out tmpSPR);

    //                        tmpGO.transform.GetChild(0).GetComponent<Image>().sprite = tmpSPR;
    //                        tmpGO.transform.GetChild(1).gameObject.SetActive(true);
    //                        tmpGO.transform.GetChild(1).GetComponent<TMP_Text>().text = kvp.Value.ToString();

    //                        tmpGO.name = kvp.Key.s_name;

    //                        Debug.Log(kvp.Key.s_name + " 를 " + kvp.Value + "개 획득");
    //                        break;
    //                    }
    //                }
    //                break;
    //            default:
    //                break;
    //        }

    //    }
    public void UpdateInventory()
    {
        var tmpInventory = dict_inventory;

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

                        Sprite tmpSPR;
                        dict_imgList.TryGetValue(int.Parse(targetItem.s_name), out tmpSPR); //이미지 리스트에서 해당번호의 이미지를 가져온다.

                        tmpGO.transform.GetChild(0).GetComponent<Image>().sprite = tmpSPR;
                        tmpGO.name = targetItem.s_name;
                        tmpGO.transform.GetChild(1).GetComponent<TMP_Text>().text = kvp.Value.ToString();
                        tmpGO.transform.GetChild(1).gameObject.SetActive(true);
                        break;
                    }

                case ItemType.Disposable:
                    {
                        GameObject tmpGO;

                        tmpGO = Instantiate(copyGO, TF_Disposables);
                        tmpGO.GetComponent<ItemObject>().I_item = targetItem;

                        Sprite tmpSPR;
                        dict_imgList.TryGetValue(int.Parse(targetItem.s_name), out tmpSPR);

                        tmpGO.transform.GetChild(0).GetComponent<Image>().sprite = tmpSPR;
                        tmpGO.name = targetItem.s_name;
                        tmpGO.transform.GetChild(1).GetComponent<TMP_Text>().text = kvp.Value.ToString();
                        tmpGO.transform.GetChild(1).gameObject.SetActive(true);
                        break;
                    }
                case ItemType.Food:
                    {
                        GameObject tmpGO;

                        tmpGO = Instantiate(copyGO, TF_Foods);
                        tmpGO.GetComponent<ItemObject>().I_item = targetItem;

                        Sprite tmpSPR;
                        dict_imgList.TryGetValue(int.Parse(targetItem.s_name), out tmpSPR);

                        tmpGO.transform.GetChild(0).GetComponent<Image>().sprite = tmpSPR;
                        tmpGO.name = targetItem.s_name;
                        tmpGO.transform.GetChild(1).GetComponent<TMP_Text>().text = kvp.Value.ToString();
                        tmpGO.transform.GetChild(1).gameObject.SetActive(true);
                        break;
                    }
                case ItemType.Other:
                    {
                        GameObject tmpGO;

                        tmpGO = Instantiate(copyGO, TF_Others);
                        tmpGO.GetComponent<ItemObject>().I_item = targetItem;

                        Sprite tmpSPR;
                        dict_imgList.TryGetValue(int.Parse(targetItem.s_name), out tmpSPR);

                        tmpGO.transform.GetChild(0).GetComponent<Image>().sprite = tmpSPR;
                        tmpGO.name = targetItem.s_name;
                        tmpGO.transform.GetChild(1).GetComponent<TMP_Text>().text = kvp.Value.ToString();
                        tmpGO.transform.GetChild(1).gameObject.SetActive(true);
                        break;
                    }
                default:
                    break;
            }
        }
        while (TF_Weapons.childCount < 5) Instantiate(copyGO, TF_Weapons);
        while (TF_Disposables.childCount < 5) Instantiate(copyGO, TF_Disposables);
        while (TF_Foods.childCount < 5) Instantiate(copyGO, TF_Foods);
        while (TF_Others.childCount < 5) Instantiate(copyGO, TF_Others);
    }

    public void DestroyElements()
    {
        while (TF_Weapons.childCount > 0) DestroyImmediate(TF_Weapons.GetChild(0).gameObject);
        while (TF_Disposables.childCount > 0) DestroyImmediate(TF_Disposables.GetChild(0).gameObject);
        while (TF_Foods.childCount > 0) DestroyImmediate(TF_Foods.GetChild(0).gameObject);
        while (TF_Others.childCount > 0) DestroyImmediate(TF_Others.GetChild(0).gameObject);

    }

    public void AddItem(Item item, int amount = 1)
    {
        if (dict_inventory.ContainsKey(item.i_id))   //해당 아이템을 이미 갖고있으면
        {
            Debug.Log(item.s_name + " + " + amount);
            dict_inventory[item.i_id] += amount;     //개수만큼 더해주고
        }
        else                                    //없으면
        {
            Debug.Log(item.s_name + " New " + amount);
            dict_inventory.Add(item.i_id, amount);   //새로 만들어준다.
        }
    }

    public void RemoveItem(Item item, int amount)
    {
        if (dict_inventory.ContainsKey(item.i_id))   //해당 아이템을 가지고 있다면
        {        
            dict_inventory[item.i_id] -= amount;     //개수를 감소시키고
            if (dict_inventory[item.i_id] <= 0)      //아이템 개수가 0 이하면
            {           
                dict_inventory.Remove(item.i_id);    //딕셔너리에서 아이템을 제거
            }
        }
    }

    public bool HasItem(Item item, int amount)
    {
        if (dict_inventory.ContainsKey(item.i_id))      //해당 아이템을 가지고 있다면     
            return dict_inventory[item.i_id] >= amount; //아이템 개수가 amount 이상이면 true, 그렇지 않으면 false 반환
        else return false;                         //해당 아이템을 가지고 있지 않다면 false 반환
    }

}
