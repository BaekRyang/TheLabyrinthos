using System;
using TMPro;
using UnityEngine;

namespace TypeDefs
{
    #region BattleMain
    public enum StatsType
    {
        Hp,
        Tp,
        Damage,
        Defense,
        Speed
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
    public class PlayerStats
    {
        //±âº» ½ºÅÝ
        public float health = 100.0f;
        public float maxHealth = 100.0f;
        public float exp = 1.0f;
        public float speed = 1.0f;
        public int defense = 5;
        public int prepareSpeed = 0;
        public int damage = 10;
    }
    #endregion

    #region Creature
    [Serializable]
    public class Creature
    {
        public Creature(int atk, int def, float hp, float spd, int pspd, Sprite fullBody = null, Sprite sideBody = null, Sprite face = null)
        {
            this.damage = atk;
            this.defense = def;
            this.health = hp;
            this.speed = spd;
            this.prepareSpeed = pspd;
            this.fullBody = fullBody;
            this.sideBody = sideBody;
            this.face = face;
        }

        public float health = 100.0f;
        public float speed = 1.0f;
        public int defense = 5;
        public int prepareSpeed = 0;
        public int damage = 1;
        public Sprite fullBody;
        public Sprite sideBody;
        public Sprite face;
        //Sprite attackSprite = null;
    }

    [Serializable]
    public struct CreatureSpritePack
    {
        public Sprite fullBody;
        public Sprite sideBody;
        public Sprite face;
    }
    #endregion

    #region RoomController
    public enum SpecialRoomType
    {
        Normal,
        StartRoom,
        VerticalCorridor,
        HorizontalCorridor,
        Crafting,
        BossRoom,
        Shop
    }
    #endregion

    #region Interactable
    enum ObjectType
    {
        MoveDoor,
        Door,
        Elevator,
        Item
    }
    #endregion

    #region
    public enum RoomType
    {
        empty,
        common,
        EndRoom,
        CraftingRoom,
        Shop,
        KeyRoom
    }
    #endregion

}
