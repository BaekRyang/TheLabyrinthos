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
    private GameObject GO_craftingTab;
    private RectTransform[] RTR_contents = new RectTransform[4];
    private int i_itemID;
    void Start()
    {
        GetComponent<CSVReader>().LoadCraftingTable();
        GO_craftingTab = GetComponent<InventoryManager>().GO_crafting;

        for (int i = 0; i < 4; i++)
            RTR_contents[i] = GO_craftingTab.transform.Find("Crafting").GetChild(1).GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetComponent<RectTransform>();

        LoadRecipe();
    }

    private void LoadRecipe()
    {
        InventoryManager IM_manager = GetComponent<InventoryManager>();
        var recipeKeys = dict_craftingTable.Keys.ToList();

        for (int i = 0; i < recipeKeys.Count; i++) //레시피 안에 있는 아이템을 전부 순환한다.
        {
            int i_type = recipeKeys[i] / 100;

            GameObject tmpGO = Instantiate(GO_recipeTab, RTR_contents[i_type]);
            tmpGO.GetComponent<Recipe>().I_destItem = IM_manager.dict_items[recipeKeys[i]];
            tmpGO.GetComponent<Recipe>().dict_recipe = dict_craftingTable[recipeKeys[i]];
            tmpGO.GetComponent<Recipe>().RunSetting(IM_manager);
        }
    }

    void Update()
    {
        
    }

    private void ConstructCraftingRecipes()
    {

    }
}
