using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TypeDefs
{

    #region BattleMain
    public enum StatsType
    {
        Hp,
        Tp,
        Damage,
        Defense,
        Speed,
        Exp,
    }

    public enum SliderColor
    {
        Hp_default,
        Hp_hilighted,
        Tp_default,
        Tp_hilighted,
        transparent
    }

    //public struct DamagePair
    //{
    //    public DamagePair(float dmg, float acc)
    //    {
    //        percentage = acc;
    //        damageMin = dmg * 0.8f;
    //        damageMax = dmg * 1.2f;
    //    }

    //    public float percentage;
    //    public float damageMin;
    //    public float damageMax;
    //}

    public struct DmgAccText
    {
        public DmgAccText(TMP_Text percent, TMP_Text dmg)
        {
            percentage = percent;
            damage = dmg;
        }
        public TMP_Text percentage;
        public TMP_Text damage;
    }

    public class UIModElements
    {
        public UIModElements(Image hp, Image tp, Transform hit)
        {
            hpSlider = hp;
            tpSlider = tp;
            hitImage = hit;
        }

        public Image hpSlider;
        public Image tpSlider;
        public Transform hitImage;
    }

    public enum ActionTypes
    {
        Attack,
        Hited,
        Avoid
    }
    #endregion

    #region BattleAttacks
    public enum Parts
    {
        Weakpoint,
        Thorax,
        Outer
    }

    public struct AttackPair
    {
        public AttackPair(float dmg, float acc)
        {
            damage = dmg;
            accuracy = acc;
        }

        public float damage;
        public float accuracy;
    }
    #endregion

    #region Player
    [Serializable]
    public class PlayerStats
    {
        //기본 스텟
        public float health = 100.0f;
        public float maxHealth = 100.0f;
        public float exp = 1.0f;
        public float speed = 1.0f;
        public int defense = 5;
        public int prepareSpeed = 0;
        public int damage = 10;
        public Sprite attackSprite;
        public Sprite hitedSprite;
        public Sprite avoidSprite;
    }
    #endregion

    #region Creature
    [Serializable]
    public class Creature
    {
        public Creature(int atk, int def, float hp, float spd, int pspd, CreatureSpritePack csp)
        {
            this.damage = atk;
            this.defense = def;
            this.health = hp;
            this.speed = spd;
            this.prepareSpeed = pspd;
            this.spritePack = csp;
        }

        public Creature(Creature creature, CreatureSpritePack csp)
        {
            this.health = creature.health;
            this.speed = creature.speed;
            this.defense = creature.defense;
            this.prepareSpeed = creature.prepareSpeed;
            this.damage = creature.damage;
            this.spritePack = csp;
        }

        public int damage = 1;
        public int defense = 5;
        public float health = 100.0f;
        public float speed = 1.0f;
        public int prepareSpeed = 0;
        public CreatureSpritePack spritePack;
        //Sprite attackSprite = null;
    }

    [Serializable]
    public struct CreatureSpritePack
    {
        public Sprite fullBody_WeakPoint;
        public Sprite fullBody_Thrax;
        public Sprite fullBody_Outer;
        public Sprite cut_Attack;
        public Sprite cut_Hited;
        public Sprite cut_Avoid;
    }
    #endregion

    #region Interactable
    enum ObjectType
    {
        MoveDoor,
        Door,
        Keypad,
        Item,
        CraftingTable
    }
    #endregion

    #region Rooms
    public enum RoomType
    {
        empty,
        common,
        EndRoom,
        StartRoom,
        VerticalCorridor,
        HorizontalCorridor,
        CraftingRoom,
        Shop,
        KeyRoom
    }
    #endregion

    #region Item
    public enum ItemType
    {
        Undefined,
        Weapon,
        Disposable,
        Food,
        Other
    }
    #endregion
}
