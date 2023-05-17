using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TypeDefs;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] GameObject GO_recipeTab;

    [Header("Set automatically")]
    public Dictionary<int, Dictionary<int, int>> dict_craftingTable = new Dictionary<int, Dictionary<int, int>>();
    public RectTransform[] RTR_contents = new RectTransform[4];

    public GameObject GO_indicator;
    public GameObject GO_destItemCell;
    public GameObject[] GO_resourceCells = new GameObject[5];

    private Dictionary<int, int> dict_targetRecipe;

    InventoryManager IM_manager;

    Inventory InventoryUI;

    void Awake()
    {
        for (int i = 0; i < 4; i++)
            RTR_contents[i] = transform.Find("Crafting").GetChild(1).GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetComponent<RectTransform>();

        Transform tmp_CraftingArea = transform.Find("Crafting").Find("Elements").Find("CraftingArea");
        GO_indicator = tmp_CraftingArea.Find("Indicator").gameObject;
        GO_destItemCell = tmp_CraftingArea.Find("DestItem").gameObject;

        for (int i = 0; i < 5; i++)
        {
            GO_resourceCells[i] = tmp_CraftingArea.Find("Ingredients").GetChild(i).gameObject;
            GO_resourceCells[i].SetActive(false);
        }

        GO_resourceCells[0].SetActive(true);
    }
    void Start()
    {
        IM_manager = InventoryManager.Instance;
        InventoryUI = IM_manager.GO_crafting.transform.Find("Inventory").GetComponent<Inventory>();
        StartCoroutine(LoadRecipe());
    }

    private IEnumerator LoadRecipe()
    {
        Debug.Log("LOAD");
        yield return StartCoroutine(GameManager.Instance.GetComponent<CSVReader>().LoadCraftingTable()); //DB 작성이 완료될때까지 기다린다.
        Debug.Log("LOAD START");
        var recipeKeys = dict_craftingTable.Keys.ToList();

        for (int i = 0; i < recipeKeys.Count; i++) //레시피 안에 있는 아이템을 전부 순환한다.
        {
            Debug.Log(recipeKeys[i]);
            int i_type = recipeKeys[i] / 100;
            GameObject tmpGO = Instantiate(GO_recipeTab, RTR_contents[i_type]);
            tmpGO.GetComponent<Recipe>().I_destItem = IM_manager.dict_items[recipeKeys[i]];
            tmpGO.GetComponent<Recipe>().dict_recipe = dict_craftingTable[recipeKeys[i]];
            tmpGO.GetComponent<Recipe>().RunSetting(IM_manager);
        }
        
        gameObject.SetActive(false); //다 끝났으면 비활성화 시킨다.
    }

    public void LoadItemToTable(int destItemID, Dictionary<int,int> recipe) //레시피 클릭하면 위에 올리는 메서드
    {
        //올리기 전에 전부 지워주고
        ResetCells(true);

        //완성품 칸 업데이트
        GO_destItemCell.GetComponent<ItemObject>().I_item = IM_manager.dict_items[destItemID];
        GO_destItemCell.GetComponent<ItemObject>().UpdateItem();

        dict_targetRecipe = recipe; //혹시 다른데서 쓸 지도 모르니깐 저장하고

        int count = 0;
        foreach (var kvp in dict_targetRecipe) //재료 칸에 레시피 따라서 올리고 업데이트
        {
            //셀 켜주고
            GO_resourceCells[count].SetActive(true); 

            //재료가 만약 무기이면서 소유를 하고있다면 그 중 첫번째를 올린다.
            if (kvp.Key < 100 && IM_manager.HasItem(kvp.Key))
            {
                GO_resourceCells[count].GetComponent<ItemObject>().I_item = IM_manager.GetWeaponInstance(kvp.Key);
                GO_resourceCells[count].GetComponent<ItemObject>().hasItem = true;
                IM_manager.RemoveItem(kvp.Key);
            }
            else
            {
                //무기인데 소유중이 아니거나, 무기가 아닌경우는 원본을 올린다.
                GO_resourceCells[count].GetComponent<ItemObject>().I_item = IM_manager.dict_items[kvp.Key];
                GO_resourceCells[count].GetComponent<ItemObject>().hasItem = IM_manager.RemoveItem(kvp.Key, kvp.Value);
            }
                
            GO_resourceCells[count].GetComponent<ItemObject>().UpdateItem(kvp.Value);
            count++;
        }
        
        InventoryUI.UpdateInventory();

        for (int i = count; i < 5; i++)
            GO_resourceCells[i].SetActive(false); //안쓰는 칸은 꺼준다.

        CalcCanCraft();
    }

    public void ResetCells(bool returnItem) //제작 칸을 전부 비우고 초기화 한다.
    {
        if (GO_resourceCells[0].GetComponent<ItemObject>().I_item == null)
            return;


        GO_destItemCell.GetComponent<ItemObject>().I_item = null;
        GO_destItemCell.GetComponent<ItemObject>().UpdateItem();
        GO_destItemCell.GetComponent<ItemObject>().b_canClick = false;

        if (returnItem)
        {
            for (int i = 0; i < 5; i++)
            {
                if (GO_resourceCells[i].activeSelf && GO_resourceCells[i].GetComponent<ItemObject>().hasItem)
                {
                    IM_manager.AddItem(
                        //올라간 아이템을
                        GO_resourceCells[i].GetComponent<ItemObject>().I_item,

                        //개수만큼 인벤에 다시 넣는다.
                        int.Parse(GO_resourceCells[i].transform.GetChild(1).GetComponent<TMP_Text>().text));
                }
            }
        }

        GO_resourceCells[0].GetComponent<ItemObject>().I_item = null;
        GO_resourceCells[0].GetComponent<ItemObject>().UpdateItem();

        for (int i = 1; i < 5; i++)
            GO_resourceCells[i].SetActive(false);
    }

    public void CalcCanCraft() //레시피 칸에 있는 아이템들을 소유하고 있는지 확인하는 메서드
    {
        int count = 0;
        int unprepared = 0;
        foreach (var kvp in dict_targetRecipe)
        {
            if (GO_resourceCells[count].GetComponent<ItemObject>().hasItem)
                GO_resourceCells[count].GetComponent<ItemObject>().TansparentItem(false);
            else
            {
                GO_resourceCells[count].GetComponent<ItemObject>().TansparentItem(true);
                unprepared++; //없으면 반투명시키고, 카운트 증가
            }
            count++;
        }

        if (unprepared != 0) //카운트가 올라있으면 재료가 다 있지 않은것임
        {
            GO_indicator.GetComponentInChildren<TMP_Text>().text = "<color=#FF0000>재료가 부족합니다.</color>";
            GO_destItemCell.GetComponent<ItemObject>().TansparentItem(true);
        }
        else
        { //재료가 다 있다면 반투명을 없애고 클릭 가능한 상태로 만든다.
            GO_indicator.GetComponentInChildren<TMP_Text>().text = GO_destItemCell.GetComponent<ItemObject>().I_item.s_name;
            GO_destItemCell.GetComponent<ItemObject>().TansparentItem(false);
            GO_destItemCell.GetComponent<ItemObject>().b_canClick = true;
        }
            
    }

    public void CraftItem() //아이템 제작 메서드
    {
        //재료 아이템 인벤에서 없애주고
        //foreach (var kvp in dict_targetRecipe)
        //    IM_manager.RemoveItem(kvp.Key, kvp.Value);

        /* 재료 아이템은 조합대에 올릴때 인벤에서 사라짐 */

        //완성품 넣어준다.
        IM_manager.AddItem(GO_destItemCell.GetComponent<ItemObject>().I_item);

        //인벤토리 업데이트 해준다.
        InventoryUI.UpdateInventory(15, true); 

        //제작대는 비워준다.
        ResetCells(false);

    }

    void Update()
    {
        
    }

}
