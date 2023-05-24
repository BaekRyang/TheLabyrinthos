using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using TypeDefs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Recipe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item I_destItem;
    public Dictionary<int, int> dict_recipe;
    int[] resourceID;
    int[] resourceCount;

    public void RunSetting(InventoryManager IM_manager)
    {
        transform.GetChild(0).GetComponent<Image>().sprite  = IM_manager.GetImage(I_destItem.i_id);
        transform.GetChild(1).GetComponent<TMP_Text>().text = I_destItem.s_name;

        resourceID = dict_recipe.Keys.ToArray();
        resourceCount = dict_recipe.Values.ToArray();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        var infoBox = InventoryManager.Instance.RT_infoBox;
        infoBox.gameObject.SetActive(true);

        if (I_destItem.IT_type == ItemType.Weapon)
        {
            infoBox.GetChild(1).transform.localScale = Vector3.one;
            Weapon tmpWeapon = I_destItem as Weapon;
            infoBox.GetChild(1).GetComponentInChildren<TMP_Text>().text = tmpWeapon.s_inspectText.Replace("\\n", "\n");
            infoBox.GetChild(1).GetComponentInChildren<TMP_Text>().text += "\nDUR : " + tmpWeapon.i_durability + "/" + tmpWeapon.i_maxDurability;
        }
        else infoBox.GetChild(1).transform.localScale = Vector3.zero;

        infoBox.localScale = Vector3.one;
        infoBox.position = transform.position;
        infoBox.GetChild(0).GetChild(0).GetComponentInChildren<TMP_Text>().text = I_destItem.s_name;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < resourceCount.Length; i++)
        {
            sb.Append(InventoryManager.definedItems[resourceID[i]].s_name + " " + resourceCount[i] + "개");
            if (i != resourceCount.Length - 1)
            {
                sb.Append("\n");
            }
        }
        infoBox.GetChild(0).GetChild(1).GetComponentInChildren<TMP_Text>().text = sb.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var infoBox = InventoryManager.Instance.RT_infoBox;
        infoBox.GetChild(1).transform.localScale = Vector3.one;
        infoBox.localScale = Vector3.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInParent<Crafting>().LoadItemToTable(I_destItem.i_id, dict_recipe);
    }
}
