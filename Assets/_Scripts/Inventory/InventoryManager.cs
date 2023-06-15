using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TypeDefs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    //인벤토리 key, value 형식으로 사용
    public static          Dictionary<int, int>          Inventory       = new Dictionary<int, int>();
    public static          Dictionary<int, List<Weapon>> WeaponInventory = new Dictionary<int, List<Weapon>>();
    public static readonly Dictionary<int, Item>         DefinedItems    = new Dictionary<int, Item>();
    public static readonly Dictionary<int, Sprite>       LoadedImages    = new Dictionary<int, Sprite>();

    public Sprite emptyItem;
    public Sprite spriteNotFounded;
    public Sprite weaponSpriteNotFounded;

    [NonSerialized] public bool      UIOpen;
    [NonSerialized] public Inventory openedInventory;
    
    [FormerlySerializedAs("InventoryTab")] public GameObject inventoryTab;
    [FormerlySerializedAs("CraftingTab")]  public GameObject craftingTab;

    public Crafting crafting;

    public ItemActionHandler itemActionHandler;
    public EquipedItem       equippedItem;
    public Stats             stats;
    public EffectIndicator   effectIndicator;

    [SerializeField]
    [FormerlySerializedAs("UIObject")]
    public GameObject UI;

    [SerializeField]
    [FormerlySerializedAs("NotificationAnchor")]
    private Transform itemNotificationAnchor;

    [SerializeField]
    [FormerlySerializedAs("NotificationPrefab")]
    private GameObject getNotificationPrefab;

    public IEnumerator LoadSetting()
    {
        Instance = this;
        crafting = craftingTab.GetComponent<Crafting>();

        UI.SetActive(true);
        inventoryTab.SetActive(true);
        craftingTab.SetActive(true);

        if (LoadedImages.Count == 0) //이미지가 하나도 없을 때만 로드
        {
            var tmpArray = Resources.LoadAll<Sprite>("Sprites/Items");

            foreach (var sprite in tmpArray)
            {
                LoadedImages.Add(int.Parse(sprite.name), sprite);
            }

            emptyItem              = LoadedImages[-1];
            spriteNotFounded       = LoadedImages[-2];
            weaponSpriteNotFounded = LoadedImages[-3];

            CreateSyringes();

            // foreach (var (_, value) in definedItems)
            // {
            //     if (value.i_id == 306) continue;
            //     AddItem(value, 2);
            // }
        }

        AddItem(DefinedItems[0]); //기본칼 추가  

        Player.Instance.WP_weapon = GetWeaponInstance(0);

        equippedItem.UpdateUI();
        stats.UpdateUI();

        inventoryTab.SetActive(false);
        yield return null;
    }

    public void OpenInventory(string target)
    {
        UIOpen = true;
        GameObject GO_targetUI = null;
        if (target == "Inventory")
        {
            stats.UpdateUI();
            effectIndicator.UpdateUI();
            equippedItem.UpdateUI();
            GO_targetUI = inventoryTab;
        }

        if (target == "Crafting")
            GO_targetUI = craftingTab;

        if (target != null)
        {
            GO_targetUI.SetActive(true);
            openedInventory = GO_targetUI.transform.Find("Inventory").GetComponent<Inventory>();
            openedInventory.UpdateInventory();
            StartCoroutine(LerpCanvas(GO_targetUI.GetComponent<CanvasGroup>(), 0, 1, 0.3f));
        }
    }

    public void CloseUI()
    {
        if (inventoryTab.activeSelf)
        {
            StartCoroutine(Lerp.LerpValueAfter(value => inventoryTab.GetComponent<CanvasGroup>().alpha = value,
                                               1,
                                               0f,
                                               0.3f,
                                               Mathf.Lerp,
                                               null,
                                               () =>
                                               {
                                                   inventoryTab.transform.Find("Inventory").GetComponent<Inventory>().DestroyElements();
                                                   inventoryTab.SetActive(false);
                                               }
                           ));
        }

        if (craftingTab.activeSelf)
        {
            StartCoroutine(Lerp.LerpValueAfter(value => craftingTab.GetComponent<CanvasGroup>().alpha = value,
                                               1,
                                               0f,
                                               0.3f,
                                               Mathf.Lerp,
                                               null,
                                               () =>
                                               {
                                                   craftingTab.transform.Find("Inventory").GetComponent<Inventory>().DestroyElements();
                                                   craftingTab.GetComponent<Crafting>().ResetCells(true);
                                                   craftingTab.SetActive(false);
                                               }
                           ));
        }

        if (GameManager.Instance.settings.activeSelf)
        {
            StartCoroutine(Lerp.LerpValueAfter(value => GameManager.Instance.settings.GetComponent<CanvasGroup>().alpha = value,
                                               1,
                                               0f,
                                               0.3f,
                                               Mathf.Lerp,
                                               null,
                                               () =>
                                                   GameManager.Instance.settings.SetActive(false)
                           ));
        }

        UIOpen = false;
    }

    private IEnumerator LerpCanvas(CanvasGroup target, float from, float to, float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            target.alpha =  Mathf.Lerp(from, to, (elapsedTime / duration));
            elapsedTime  += Time.deltaTime;
            yield return null;
        }

        target.alpha = to;
    }

    public void AddItem(Item item, int amount = 1)
    {
        if (item.IT_type == ItemType.Weapon)
        {
            if (!WeaponInventory.ContainsKey(item.i_id))
                //해당 무기를 이미 갖고있지 않으면 리스트를 만들어준다.
                WeaponInventory.Add(item.i_id, new List<Weapon>());
            //그 다음 리스트에 아이템을 추가해준다.
            WeaponInventory[item.i_id].Add(new Weapon(item as Weapon));
            PlayItemNotification(item, amount);
            return;
        }

        if (Inventory.ContainsKey(item.i_id)) //해당 아이템을 이미 갖고있으면
            Inventory[item.i_id] += amount;   //개수만큼 더해주고
        else                                  //없으면
            Inventory.Add(item.i_id, amount); //새로 만들어준다.

        PlayItemNotification(item, amount);
    }

    public void AddItem(int item, int amount = 1)
    {
        if (item == -100)
        {
            //무작위 아이템을 추가
            AddItem(DefinedItems[Random.Range(300, 311)], Random.Range(1, 4)); //무작위 아이템을 1~3개 추가
        }
        
        if (item < 100)
        {
            AddItem(DefinedItems[item]);
            return;
        }

        if (Inventory.ContainsKey(item)) //해당 아이템을 이미 갖고있으면
        {
            Inventory[item] += amount; //개수만큼 더해주고
        }
        else //없으면
        {
            Inventory.Add(item, amount); //새로 만들어준다.
        }
    }

    public bool RemoveItem(Item itemInstance, int amount = 1) //아이템의 객체를 지정해서 삭제
    {
        bool removed = false;
        if (itemInstance.IT_type == ItemType.Weapon)
        {
            //인자로 들어온 해당 아이템을 삭제한다.
            if (WeaponInventory[itemInstance.i_id].Contains(itemInstance as Weapon))
            {
                WeaponInventory[itemInstance.i_id].Remove(itemInstance as Weapon);

                if (WeaponInventory[itemInstance.i_id].Count == 0)
                    // 해당 무기 아이템 리스트가 비어있으면 딕셔너리에서 제거
                    WeaponInventory.Remove(itemInstance.i_id);
                removed = true;
                PlayItemNotification(itemInstance, -1);
            }
        }
        else
            removed = RemoveItem(itemInstance.i_id);

        return removed;
    }

    public bool RemoveItem(int itemIndex, int amount = 1) //아이템의 코드를 지정해서 일치하는 아이템 삭제
    {
        if (itemIndex < 100)
        {
            //무기는 무조건 맨 앞에 있던걸 지운다.
            if (WeaponInventory.ContainsKey(itemIndex) && WeaponInventory[itemIndex].Count > 0)
            {
                if (WeaponInventory[itemIndex].Count > 0)
                    WeaponInventory[itemIndex].RemoveAt(0); // 무기 아이템 리스트에서 첫번째 아이템 제거

                if (WeaponInventory[itemIndex].Count == 0)
                    WeaponInventory.Remove(itemIndex); // 해당 무기 아이템 리스트가 비어있으면 딕셔너리에서 제거

                PlayItemNotification(DefinedItems[itemIndex], -amount);
                return true;
            }

            return false;
        }

        if (Inventory.ContainsKey(itemIndex)) //해당 아이템을 가지고 있다면
        {
            Inventory[itemIndex] -= amount;  //개수를 감소시키고
            if (Inventory[itemIndex] <= 0)   //아이템 개수가 0 이하면
                Inventory.Remove(itemIndex); //딕셔너리에서 아이템을 제거
            PlayItemNotification(DefinedItems[itemIndex], -amount);
            return true;
        }


        return false;
    }

    public bool HasItem(Item item, int amount = 1)
    {
        if (item.IT_type == ItemType.Weapon)
        {
            if (WeaponInventory.ContainsKey(item.i_id))
                return true;
            return false;
        }

        if (Inventory.TryGetValue(item.i_id, out var value)) //해당 아이템을 가지고 있다면     
            return value >= amount;                          //아이템 개수가 amount 이상이면 true, 그렇지 않으면 false 반환
        return false;                                        //해당 아이템을 가지고 있지 않다면 false 반환
    }


    public bool HasItem(int item, int amount = 1)
    {
        if (item < 100)
        {
            if (WeaponInventory.ContainsKey(item))
                return true;
            return false;
        }

        if (Inventory.TryGetValue(item, out var value)) //해당 아이템을 가지고 있다면    
            return value >= amount;                     //아이템 개수가 amount 이상이면 true, 그렇지 않으면 false 반환
        return false;                                   //해당 아이템을 가지고 있지 않다면 false 반환
    }

    public Sprite GetImage(int id)
    {
        bool isSpriteExist = LoadedImages.TryGetValue(id, out Sprite foundedSprite);

        return isSpriteExist ? foundedSprite : spriteNotFounded;
    }

    public Weapon GetWeaponInstance(int id)
    {
        if (HasItem(id))
            //해당 아이템을 갖고있다면 제일 처음 아이템을 반환한다.
            return WeaponInventory[id][0];

        return null;
    }

    public void Wear(ref Item item)
    {
        Player.Instance.WP_weapon = item as Weapon;

        equippedItem.UpdateUI();
        stats.UpdateUI();
    }

    private void CreateSyringes()
    {
        var effectCount = Enum.GetValues(typeof(EffectTypes)).Length;

        //이펙트의 개수 출력
        Debug.Log(effectCount);

        Disposable tmpDisposable;
        for (int i = 0; i < effectCount; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                tmpDisposable = new Disposable(DefinedItems[104] as Disposable);

                tmpDisposable.CreateEffect((EffectTypes)i, j == 1);

                tmpDisposable.i_id = 500 + i * 2 + j;
                //tmpDisposable.color를 i 따라서 무작위 색상으로 설정 (단 너무 비슷한 색상끼리 겹치지 않게)
                tmpDisposable.color = new Color(
                    Random.Range(0.0f, 0.5f) + (i % 2) * 0.5f,
                    Random.Range(0.0f, 0.5f) + (i % 3) * 0.5f,
                    Random.Range(0.0f, 0.5f) + (i % 5) * 0.5f);

                Debug.Log(tmpDisposable.i_id + " 주사기 추가 - " + GameManager.Instance.effectsManager.GetEffectDesc(tmpDisposable.effect));
                DefinedItems.Add(tmpDisposable.i_id, tmpDisposable);

                if ((EffectTypes)i is EffectTypes.Poison)
                    break;
                //독은 두개 만들것이 없음
            }
        }
    }

    private void PlayItemNotification(Item item, int count)
    {
        var noti     = Instantiate(getNotificationPrefab, itemNotificationAnchor);
        var notiIcon = noti.transform.GetComponentsInChildren<Image>()[1];
        var notiText = noti.transform.GetComponentInChildren<TMP_Text>();
        var notiAnim = noti.GetComponent<MMF_Player>();
        notiAnim.Initialization();

        notiIcon.sprite = GetImage(item.i_id);
        notiText.text = $"{(count >= 1 ? "<color=#00ff68>+" : "<color=#FF0000>")}" +
                        $"{count}</color> "                                        +
                        $"{DefinedItems[item.i_id].s_name}";
        noti.GetComponent<MMF_Player>().PlayFeedbacks();
    }
}