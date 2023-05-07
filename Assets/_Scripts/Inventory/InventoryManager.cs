using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TypeDefs;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private int i_inventoryCount; //인스펙터 확인용
    [SerializeField] public int i_itemCount; //인스펙터 확인용

    //인벤토리 key, value 형식으로 사용
    public Dictionary<int, int> dict_inventory;
    public Dictionary<int, Sprite> dict_imgList = new Dictionary<int, Sprite>();
    public Dictionary<int, Item> dict_items;

    public bool b_UIOpen;

    [Header("Set In Inspector")]
    //UI요소
    public GameObject GO_inventory;
    public GameObject GO_crafting;

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
        GO_inventory.transform.Find("Inventory").GetComponent<Inventory>().LoadSetting();
        GO_inventory.SetActive(false);
        GO_crafting.transform.Find("Inventory").GetComponent<Inventory>().LoadSetting();
        //GO_crafting.SetActive(false); //Crafting에서 로딩 다 끝나면 알아서 비활성화 한다.
    }

    void Update()
    {
        
    }

    public void OpenInventory(string target)
    {
        b_UIOpen = true;
        GameObject GO_targetUI = null;
        if (target == "Inventory")
            GO_targetUI = GO_inventory;
        if (target == "Crafting")
            GO_targetUI = GO_crafting;

        if (target != null)
        {
            GO_targetUI.SetActive(true);
            GO_targetUI.transform.Find("Inventory").GetComponent<Inventory>().UpdateInventory();
            StartCoroutine(LerpCanvas(GO_targetUI.GetComponent<CanvasGroup>(), 0, 1, 0.3f));
        }

        GetComponent<Player>().WP_weapon = dict_items[0] as Weapon;

    }

    public IEnumerator CloseUI()
    {
        if (GO_inventory.activeSelf)
        {
            Debug.Log("CLOSE Inven");
            yield return StartCoroutine(LerpCanvas(GO_inventory.GetComponent<CanvasGroup>(), 1, 0, 0.3f));
            GO_inventory.transform.Find("Inventory").GetComponent<Inventory>().DestroyElements();
            GO_inventory.SetActive(false);
        }
        else if (GO_crafting.activeSelf)
        {
            Debug.Log("CLOSE Craft");
            yield return StartCoroutine(LerpCanvas(GO_crafting.GetComponent<CanvasGroup>(), 1, 0, 0.3f));
            GO_crafting.transform.Find("Inventory").GetComponent<Inventory>().DestroyElements();
            GO_crafting.GetComponent<Crafting>().ResetCells();
            GO_crafting.SetActive(false);
        }
        b_UIOpen = false;

    }

    private IEnumerator LerpCanvas(CanvasGroup target, float from, float to, float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            target.alpha = Mathf.Lerp(from, to, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.alpha = to;
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

    public void AddItem(int item, int amount = 1)
    {
        if (dict_inventory.ContainsKey(item))   //해당 아이템을 이미 갖고있으면
        {
            dict_inventory[item] += amount;     //개수만큼 더해주고
        }
        else                                    //없으면
        {
            dict_inventory.Add(item, amount);   //새로 만들어준다.
        }
    }

    public void RemoveItem(int item, int amount)
    {
        if (dict_inventory.ContainsKey(item))   //해당 아이템을 가지고 있다면
        {
            dict_inventory[item] -= amount;     //개수를 감소시키고
            if (dict_inventory[item] <= 0)      //아이템 개수가 0 이하면
            {
                dict_inventory.Remove(item);    //딕셔너리에서 아이템을 제거
            }
        }
    }

    public bool HasItem(int item, int amount)
    {
        if (dict_inventory.ContainsKey(item))      //해당 아이템을 가지고 있다면     
            return dict_inventory[item] >= amount; //아이템 개수가 amount 이상이면 true, 그렇지 않으면 false 반환
        else return false;                         //해당 아이템을 가지고 있지 않다면 false 반환
    }

}
