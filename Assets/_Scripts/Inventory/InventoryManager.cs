using System;
using System.Collections;
using System.Collections.Generic;
using TypeDefs;
using UnityEngine;
using Random = UnityEngine.Random;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    //인벤토리 key, value 형식으로 사용
    [Header("Dictionaries")]
    public static readonly Dictionary<int, int>          inventory       = new Dictionary<int, int>();
    public static readonly Dictionary<int, List<Weapon>> weaponInventory = new Dictionary<int, List<Weapon>>();
    public static readonly Dictionary<int, Item>         definedItems    = new Dictionary<int, Item>();
    public static readonly Dictionary<int, Sprite>       loadedImages    = new Dictionary<int, Sprite>();
    
    [Header("Default Sprite Setting")]
    public Sprite emptyItem;
    public Sprite spriteNotFounded;
    public Sprite weaponSpriteNotFounded;

    [Header("UI Setting")]
    public bool      b_UIOpen;
    public Inventory openedInventory;

    [Header("Set In Inspector")]
    //UI요소
    public GameObject GO_inventory;
    public GameObject GO_crafting;

    public Crafting crafting;

    public ItemActionHandler itemActionHandler;
    public EquipedItem       equippedItem;
    public Stats             stats;
    public EffectIndicator   effectIndicator;

    public GameObject ui;

    public IEnumerator LoadSetting()
    {
        Instance = this;
        crafting = GO_crafting.GetComponent<Crafting>();
        
        ui.SetActive(true);
        GO_inventory.SetActive(true);
        GO_crafting.SetActive(true);

        if (loadedImages.Count == 0) //이미지가 하나도 없을 때만 로드
        {
            var tmpArray = Resources.LoadAll<Sprite>("Sprites/Items");

            foreach (var sprite in tmpArray)
            {
                loadedImages.Add(int.Parse(sprite.name), sprite);
            }

            emptyItem              = loadedImages[-1];
            spriteNotFounded       = loadedImages[-2];
            weaponSpriteNotFounded = loadedImages[-3];

            CreateSyringes();
            
            foreach (var (_, value) in definedItems)
            {
                if (value.i_id == 306) continue;
                AddItem(value, 2);
            }
        }
        
        // AddItem(definedItems[0]); //기본칼 추가

        equippedItem.UpdateUI();
        stats.UpdateUI();
        
        GO_inventory.SetActive(false);
        yield return null;
    }

    public void OpenInventory(string target)
    {
        b_UIOpen = true;
        GameObject GO_targetUI = null;
        if (target == "Inventory")
        {
            stats.UpdateUI();
            effectIndicator.UpdateUI();
            equippedItem.UpdateUI();
            GO_targetUI = GO_inventory;
        }
        if (target == "Crafting")
            GO_targetUI = GO_crafting;

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
        if (GO_inventory.activeSelf)
        {
            StartCoroutine(Lerp.LerpValueAfter(value => GO_inventory.GetComponent<CanvasGroup>().alpha = value,
                1,
                0f,
                0.3f,
                Mathf.Lerp,
                null,
                () =>
                {
                    GO_inventory.transform.Find("Inventory").GetComponent<Inventory>().DestroyElements();
                    GO_inventory.SetActive(false);
                }
            ));
        }

        if (GO_crafting.activeSelf)
        {
            StartCoroutine(Lerp.LerpValueAfter(value => GO_crafting.GetComponent<CanvasGroup>().alpha = value,
                1,
                0f,
                0.3f,
                Mathf.Lerp,
                null,
                () =>
                {
                    GO_crafting.transform.Find("Inventory").GetComponent<Inventory>().DestroyElements();
                    GO_crafting.GetComponent<Crafting>().ResetCells(true);
                    GO_crafting.SetActive(false);
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

        b_UIOpen = false;
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
            if (!weaponInventory.ContainsKey(item.i_id))
                //해당 무기를 이미 갖고있지 않으면 리스트를 만들어준다.
                weaponInventory.Add(item.i_id, new List<Weapon>());
            //그 다음 리스트에 아이템을 추가해준다.
            weaponInventory[item.i_id].Add(new Weapon(item as Weapon));
            return;
        }

        if (inventory.ContainsKey(item.i_id)) //해당 아이템을 이미 갖고있으면
            inventory[item.i_id] += amount;   //개수만큼 더해주고
        else                                       //없으면
            inventory.Add(item.i_id, amount); //새로 만들어준다.
    }

    public bool RemoveItem(Item item, int amount = 1)
    {
        if (item.IT_type == ItemType.Weapon)
        {
            //인자로 들어온 해당 아이템을 삭제한다.
            if (weaponInventory[item.i_id].Contains(item as Weapon))
            {
                weaponInventory[item.i_id].Remove(item as Weapon);

                if (weaponInventory[item.i_id].Count == 0)
                    // 해당 무기 아이템 리스트가 비어있으면 딕셔너리에서 제거
                    weaponInventory.Remove(item.i_id);
                return true;
            }

            return false;
        }

        if (inventory.ContainsKey(item.i_id)) //해당 아이템을 가지고 있다면
        {
            inventory[item.i_id] -= amount;  //개수를 감소시키고
            if (inventory[item.i_id] <= 0)   //아이템 개수가 0 이하면
                inventory.Remove(item.i_id); //딕셔너리에서 아이템을 제거
            return true;
        }

        return false;
    }


    public bool HasItem(Item item, int amount = 1)
    {
        if (item.IT_type == ItemType.Weapon)
        {
            if (weaponInventory.ContainsKey(item.i_id))
                return true;
            return false;
        }

        if (inventory.TryGetValue(item.i_id, out var value))      //해당 아이템을 가지고 있다면     
            return value >= amount; //아이템 개수가 amount 이상이면 true, 그렇지 않으면 false 반환
        return false;                                   //해당 아이템을 가지고 있지 않다면 false 반환
    }


    public void AddItem(int item, int amount = 1)
    {
        if (item < 100)
        {
            AddItem(definedItems[item]);
            return;
        }

        if (inventory.ContainsKey(item)) //해당 아이템을 이미 갖고있으면
        {
            inventory[item] += amount; //개수만큼 더해주고
        }
        else //없으면
        {
            inventory.Add(item, amount); //새로 만들어준다.
        }
    }

    public bool RemoveItem(int item, int amount = 1)
    {
        if (item < 100)
        {
            //무기는 무조건 맨 앞에 있던걸 지운다.
            if (weaponInventory.ContainsKey(item) && weaponInventory[item].Count > 0)
            {
                if (weaponInventory[item].Count > 0)
                    weaponInventory[item].RemoveAt(0); // 무기 아이템 리스트에서 첫번째 아이템 제거

                if (weaponInventory[item].Count == 0)
                    weaponInventory.Remove(item); // 해당 무기 아이템 리스트가 비어있으면 딕셔너리에서 제거
                return true;
            }

            return false;
        }

        if (inventory.ContainsKey(item)) //해당 아이템을 가지고 있다면
        {
            inventory[item] -= amount;  //개수를 감소시키고
            if (inventory[item] <= 0)   //아이템 개수가 0 이하면
                inventory.Remove(item); //딕셔너리에서 아이템을 제거
            return true;
        }

        return false;
    }

    public bool HasItem(int item, int amount = 1)
    {
        if (item < 100)
        {
            if (weaponInventory.ContainsKey(item))
                return true;
            return false;
        }

        if (inventory.TryGetValue(item, out var value)) //해당 아이템을 가지고 있다면    
            return value >= amount;                             //아이템 개수가 amount 이상이면 true, 그렇지 않으면 false 반환
        return false;                                           //해당 아이템을 가지고 있지 않다면 false 반환
    }

    public Sprite GetImage(int id)
    {
        bool isSpriteExist = loadedImages.TryGetValue(id, out Sprite foundedSprite);

        return isSpriteExist ? foundedSprite : spriteNotFounded;
    }

    public Weapon GetWeaponInstance(int id)
    {
        if (HasItem(id))
            //해당 아이템을 갖고있다면 제일 처음 아이템을 반환한다.
            return weaponInventory[id][0];

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
                tmpDisposable = new Disposable(definedItems[104] as Disposable);
                
                tmpDisposable.CreateEffect((EffectTypes)i, j == 1);
                
                tmpDisposable.i_id = 500 + i * 2 + j;
                //tmpDisposable.color를 i 따라서 무작위 색상으로 설정 (단 너무 비슷한 색상끼리 겹치지 않게)
                tmpDisposable.color = new Color(
                    Random.Range(0.0f, 0.5f) + (i % 2) * 0.5f,
                    Random.Range(0.0f, 0.5f) + (i % 3) * 0.5f,
                    Random.Range(0.0f, 0.5f) + (i % 5) * 0.5f);

                Debug.Log(tmpDisposable.i_id + " 주사기 추가 - " + GameManager.Instance.effectsManager.GetEffectDesc(tmpDisposable.effect));
                definedItems.Add(tmpDisposable.i_id, tmpDisposable);

                if ((EffectTypes)i is EffectTypes.Poison)
                    break;
                //독은 두개 만들것이 없음
            }
        }

    }
}