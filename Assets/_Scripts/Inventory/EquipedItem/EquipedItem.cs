using System;
using System.Collections;
using System.Collections.Generic;
using Tayx.Graphy.Utils.NumString;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipedItem : MonoBehaviour
{
    private Slider    durability;
    private TMP_Text  name;
    private Transform statsDamage;
    private Transform statsSpeed;
    private Transform statsAccuracy;
    private Transform statsPrepareSpeed;
    private Image     weaponImage;

    private Player player;

    private void Awake()
    {
        var elementsAnchor = transform.GetChild(1);
        
        durability        = elementsAnchor.Find("Durability").GetComponent<Slider>();
        name              = elementsAnchor.Find("Name").GetComponent<TMP_Text>();
        weaponImage       = elementsAnchor.Find("Image").GetComponent<Image>();
        
        elementsAnchor    = elementsAnchor.Find("Desc");
        
        statsDamage       = elementsAnchor.Find("Damage");
        statsSpeed        = elementsAnchor.Find("Speed");
        statsAccuracy     = elementsAnchor.Find("Accuracy");
        statsPrepareSpeed = elementsAnchor.Find("PrepareSpeed");
    }

    private void Start()
    {
        player = GameManager.Instance.GetComponent<Player>();
    }

    public void UpdateUI()
    {
        Weapon weapon = player.WP_weapon;
        durability.maxValue = weapon.i_maxDurability;
        durability.value    = weapon.i_durability;

        name.text = weapon.s_name;
        
        UpdateStatsCell();

        bool isSpriteExist =
            InventoryManager.Instance.dict_imgList.TryGetValue(player.WP_weapon.i_id + 1000, out Sprite foundedSprite);

        weaponImage.sprite =
            isSpriteExist ? foundedSprite : InventoryManager.Instance.weaponSpriteNotFounded;
    }

    private void UpdateStatsCell()
    {
        Weapon weapon = player.WP_weapon;
        statsDamage.GetChild(0).GetComponent<TMP_Text>().text =
            $"{weapon.i_damage - weapon.i_damageRange} ~ {weapon.i_damage + weapon.i_damageRange}";

        statsSpeed.GetChild(0).GetComponent<TMP_Text>().text =
            $"{(weapon.f_speedMult * 100).ToInt()} %";
        statsSpeed.GetChild(1).GetComponent<TMP_Text>().text =
            weapon.f_speedMult > 1 ? $"<color=#00FF00>+{(weapon.f_speedMult * 100).ToInt() - 100}%</color>" : 
                weapon.f_speedMult < 1 ? $"<color=#FF0000>-{100 - (weapon.f_speedMult * 100).ToInt()}%</color>" : 
                                         "-";
        
        statsAccuracy.GetChild(0).GetComponent<TMP_Text>().text =
            $"{(weapon.f_accuracyMult * 100).ToInt()} %";
        statsAccuracy.GetChild(1).GetComponent<TMP_Text>().text =
            weapon.f_accuracyMult > 1 ? $"<color=#00FF00>+{(weapon.f_accuracyMult * 100).ToInt() - 100}%</color>" : 
            weapon.f_accuracyMult < 1 ? $"<color=#FF0000>-{100 - (weapon.f_accuracyMult * 100).ToInt()}%</color>" : 
                                        "-";
        
        statsPrepareSpeed.GetChild(0).GetComponent<TMP_Text>().text =
            $"{weapon.i_preparedSpeed}";
        statsPrepareSpeed.GetChild(1).GetComponent<TMP_Text>().text =
            $"{(weapon.i_preparedSpeed > 1 ? "<color=#00FF00>+" : "")}" +
            $"{weapon.i_preparedSpeed}";
    }
}
