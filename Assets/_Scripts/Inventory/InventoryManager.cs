using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TypeDefs;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private int i_inventoryCount; //�ν����� Ȯ�ο�
    [SerializeField] public int i_itemCount; //�ν����� Ȯ�ο�


    //�κ��丮 key, value �������� ���
    Dictionary<int, int> dict_inventory;
    Dictionary<int, Sprite> dict_imgList = new Dictionary<int, Sprite>();
    public Dictionary<int, Item> dict_items;

    [SerializeField] Transform TF_Weapons;
    [SerializeField] Transform TF_Disposables;
    [SerializeField] Transform TF_Foods;
    [SerializeField] Transform TF_Others;


    [SerializeField] GameObject copyGO;
    [SerializeField] public RectTransform RT_infoBox;
    [SerializeField] public RectTransform RT_descBox;
    private void Awake()
    {
        Instance = this;
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
                        dict_imgList.TryGetValue(targetItem.i_id, out tmpSPR); //�̹��� ����Ʈ���� �ش��ȣ�� �̹����� �����´�.

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
                        dict_imgList.TryGetValue(targetItem.i_id, out tmpSPR);

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
                        dict_imgList.TryGetValue(targetItem.i_id, out tmpSPR);

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
                        dict_imgList.TryGetValue(targetItem.i_id    , out tmpSPR);

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

        CalcCellSize();
    }

    void CalcCellSize()
    {
        while (TF_Weapons.childCount < 6) Instantiate(copyGO, TF_Weapons);
        TF_Weapons.GetComponent<RectTransform>().sizeDelta = new Vector2(160 * TF_Weapons.childCount - 30, 0); //Cellsize + Spaceing * ĭ ���� ��ŭ ĭ ũ�⸦ Ű���ش�.
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

    public void AddItem(Item item, int amount = 1)
    {
        if (dict_inventory.ContainsKey(item.i_id))   //�ش� �������� �̹� ����������
        {
            Debug.Log(item.s_name + " + " + amount);
            dict_inventory[item.i_id] += amount;     //������ŭ �����ְ�
        }
        else                                    //������
        {
            Debug.Log(item.s_name + " New " + amount);
            dict_inventory.Add(item.i_id, amount);   //���� ������ش�.
        }
    }

    public void RemoveItem(Item item, int amount)
    {
        if (dict_inventory.ContainsKey(item.i_id))   //�ش� �������� ������ �ִٸ�
        {        
            dict_inventory[item.i_id] -= amount;     //������ ���ҽ�Ű��
            if (dict_inventory[item.i_id] <= 0)      //������ ������ 0 ���ϸ�
            {           
                dict_inventory.Remove(item.i_id);    //��ųʸ����� �������� ����
            }
        }
    }

    public bool HasItem(Item item, int amount)
    {
        if (dict_inventory.ContainsKey(item.i_id))      //�ش� �������� ������ �ִٸ�     
            return dict_inventory[item.i_id] >= amount; //������ ������ amount �̻��̸� true, �׷��� ������ false ��ȯ
        else return false;                         //�ش� �������� ������ ���� �ʴٸ� false ��ȯ
    }

}
