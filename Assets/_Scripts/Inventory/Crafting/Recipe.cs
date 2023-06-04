using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Recipe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item I_destItem;
    public Dictionary<int, int> dict_recipe;
    int[] resourceID;
    int[] resourceCount;

    public void RunSetting()
    {
        transform.GetChild(0).GetComponent<Image>().sprite  = InventoryManager.Instance.GetImage(I_destItem.i_id);
        transform.GetChild(1).GetComponent<TMP_Text>().text = I_destItem.s_name;

        resourceID = dict_recipe.Keys.ToArray();
        resourceCount = dict_recipe.Values.ToArray();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        var infoBoxPack = PopUpManager.Instance.infoBoxP;
        
        infoBoxPack.box.gameObject.SetActive(true);
        infoBoxPack.box.position = transform.position;

        if (I_destItem.IT_type == ItemType.Weapon)
        {
            infoBoxPack.inspectObject.SetActive(true);
            Weapon tmpWeapon = I_destItem as Weapon;
            infoBoxPack.inspect.text =  tmpWeapon.s_inspectText.Replace("\\n", "\n");
            infoBoxPack.inspect.text += "\nDUR : " + tmpWeapon.i_durability + "/" + tmpWeapon.i_maxDurability;
        }
        
        infoBoxPack.title.text = I_destItem.s_name;

        StringBuilder sb = new StringBuilder();
        sb.Append(I_destItem.s_description + "\n\n" + "<color=orange>필요한 재료</color>\n");
        for (int i = 0; i < resourceCount.Length; i++)
        {
            sb.Append(InventoryManager.definedItems[resourceID[i]].s_name + " <color=green>" + resourceCount[i] + "개</color>");
            if (i != resourceCount.Length - 1)
            {
                sb.Append("\n");
            }
        }
        infoBoxPack.desc.text = sb.ToString();
        
        StartCoroutine(PopUpManager.UpdateUI(infoBoxPack));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PopUpManager.Instance.infoBox.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInParent<Crafting>().LoadItemToTable(I_destItem.i_id, dict_recipe);
    }
}
