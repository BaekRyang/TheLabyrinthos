using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public RectTransform RTR_destItemCell;
    public GameObject[] GO_resourceCells = new GameObject[5];

    InventoryManager IM_manager;

    void Awake()
    {
        for (int i = 0; i < 4; i++)
            RTR_contents[i] = transform.Find("Crafting").GetChild(1).GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetComponent<RectTransform>();

        Transform tmp_CraftingArea = transform.Find("Crafting").Find("Elements").Find("CraftingArea");
        GO_indicator = tmp_CraftingArea.Find("Indicator").gameObject;
        RTR_destItemCell = tmp_CraftingArea.Find("DestItem").GetComponent<RectTransform>();

        for (int i = 0; i < 5; i++)
        {
            GO_resourceCells[i] = tmp_CraftingArea.Find("Ingredients").GetChild(i).gameObject;
            GO_resourceCells[i].SetActive(false);
        }
    }
    void Start()
    {
        IM_manager = InventoryManager.Instance;
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

    public void LoadItemToTable(int destItemID, Dictionary<int,int> recipe)
    {
        RTR_destItemCell.GetComponent<ItemObject>().I_item = IM_manager.dict_items[destItemID];
        RTR_destItemCell.GetComponent<ItemObject>().UpdateItem();

        int count = 0;
        foreach (var kvp in recipe)
        {
            GO_resourceCells[count].SetActive(true);
            GO_resourceCells[count].GetComponent<ItemObject>().I_item = IM_manager.dict_items[kvp.Key];
            GO_resourceCells[count].GetComponent<ItemObject>().UpdateItem(kvp.Value);
            count++;
        }

        for (int i = count; i < 5; i++)
            GO_resourceCells[i].SetActive(false); //안쓰는 칸은 꺼준다.
    }

    void Update()
    {
        
    }

}
