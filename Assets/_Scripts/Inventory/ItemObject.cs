using TMPro;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour, IScrollHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    ScrollRect SR_parent;
    
    [Header("For Inventory Actions")]
    public bool transferScrollEvent; //인벤토리 UI인지 표시

    //조합대에서도 이거 쓰니까 있는 설정
    public bool b_canClick;

    [Header("Item value : id를 기준으로 사용됨")] 
    public Item I_item;

    [Header("For Crafting Actions")] 
    public bool hasItem;

    private void Awake()
    {
        if (transferScrollEvent)
            SR_parent = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    public void UpdateItem(int amount = 1)
    {
        bool isEmptyCell = I_item.IsUnityNull();

        if (isEmptyCell)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = InventoryManager.Instance.emptyItem;
            return;
        }

        if (I_item.i_id >= 500)
        {
            var tmpImage = transform.GetChild(0).GetComponent<Image>();
            tmpImage.sprite = InventoryManager.loadedImages[104];

            if (I_item is Disposable tmpDisposable)
                tmpImage.color = tmpDisposable.color;
        }
        else
        {
            bool isSpriteExist = InventoryManager.loadedImages.TryGetValue(I_item.i_id, out Sprite foundedSprite);

            transform.GetChild(0).GetComponent<Image>().sprite =
                isSpriteExist ? foundedSprite : InventoryManager.Instance.spriteNotFounded;
        }

        

        if (amount > 1)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(1).GetComponent<TMP_Text>().text = amount.ToString();
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void TransparentItem(bool transparent)
    {
        transform.GetChild(0).GetComponent<Image>().color = transparent ?
            new Color(0.5f, 0.5f, 0.5f, 0.5f) :
            new Color(1, 1, 1, 1);
    }

    public void OnScroll(PointerEventData eventData) //스크롤 요소때문에 이벤트를 넘겨준다.
    {
        if (!SR_parent.IsUnityNull())
            SR_parent.OnScroll(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (I_item == null || I_item.IT_type == ItemType.Undefined) return;

        var infoBox = GameManager.Instance.b_nowBattle ?
            BattleMain.instance.infoBox :
            InventoryManager.Instance.RT_infoBox;
        infoBox.gameObject.SetActive(true);

        var textComponents = infoBox.GetComponentsInChildren<TMP_Text>();

        if (I_item.IT_type == ItemType.Weapon)
        {
            infoBox.GetChild(1).transform.localScale = Vector3.one;
            Weapon tmpWeapon = I_item as Weapon;
            textComponents[2].text =  tmpWeapon.s_inspectText.Replace("\\n", "\n");
            textComponents[2].text += "\nDUR : " + tmpWeapon.i_durability + "/" + tmpWeapon.i_maxDurability;
        }
        else infoBox.GetChild(1).transform.localScale = Vector3.zero;

        infoBox.localScale = Vector3.one;
        
        infoBox.position   = transform.position;
        
        // StopAllCoroutines();
        // StartCoroutine(Lerp.LerpValue(
        //     value => infoBox.position = value,
        //     infoBox.position,
        //     transform.position,
        //     0.2f,
        //     Vector3.Lerp,
        //     Lerp.EaseInOut));
        
        textComponents[0].text = I_item.s_name;
        textComponents[1].text = I_item.s_description.Replace("\\n", "\n");

        if (I_item is Disposable disposableItem)
        {
            if (disposableItem.effect == null)
                return;
            
            if (!GameManager.Instance.effectsManager.IsKnown(disposableItem.effect.effectType, disposableItem.effect.isPositive))
                textComponents[1].text += "\n\n<b><color=#999999>알 수 없는 효과</color></b>";
            else
                textComponents[1].text += "\n\n<b><color=#CCCC00>" + 
                                          GameManager.Instance.effectsManager.GetEffectDesc(disposableItem.effect) +
                                          "</color></b>";
        }
            

        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var infoBox = GameManager.Instance.b_nowBattle ?
            BattleMain.instance.infoBox :
            InventoryManager.Instance.RT_infoBox;
        infoBox.GetChild(1).transform.localScale = Vector3.one;
        infoBox.localScale                       = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked");
        if (!b_canClick)
            return;

        if (gameObject.name == "DestItem")
        {
            InventoryManager.Instance.crafting.CraftItem();
            OnPointerExit(eventData);
            return;
        }

        var itemHandler = GameManager.Instance.b_nowBattle ?
            BattleMain.instance.infoBox.transform.parent.GetChild(1).GetComponent<ItemActionHandler>() :
            InventoryManager.Instance.itemActionHandler;
        
        itemHandler.LoadItem(ref I_item, transform.position);
    }

}