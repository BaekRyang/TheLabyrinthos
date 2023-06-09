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
        public UIModElements(Image hp, Image tp)
        {
            hpSlider = hp;
            tpSlider = tp;
        }

        public Image hpSlider;
        public Image tpSlider;
    }

    public enum ActionTypes
    {
        Attack,
        Missed
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
        public float  health       = 100.0f;
        public float  maxHealth    = 100.0f;
        public float  exp          = 0.0f;
        public float  speed        = 1.0f;
        public int    defense      = 5;
        public int    prepareSpeed = 0;
        public float  accuracyMult = 1;
        public int    damage       = 10;
    }
    
    #endregion

    #region Creature
    [Serializable]
    public class Creature
    {
        public Creature(Creature other)
        {
            health = other.health;
            speed = other.speed;
            defense = other.defense;
            prepareSpeed = other.prepareSpeed;
            damage = other.damage;
            spritePack = other.spritePack;
        }
        
        public Creature(int atk, int def, float hp, float spd, int pspd, CreatureSpritePack csp)
        {
            damage = atk;
            defense = def;
            health = hp;
            speed = spd;
            prepareSpeed = pspd;
            spritePack = csp;
        }

        public Creature(Creature creature, CreatureSpritePack csp)
        {
            health = creature.health;
            speed = creature.speed;
            defense = creature.defense;
            prepareSpeed = creature.prepareSpeed;
            damage = creature.damage;
            spritePack = csp;
        }

        public int damage = 1;
        public int defense = 5;
        public float health = 100.0f;
        public float speed = 1.0f;
        public int prepareSpeed;
        public CreatureSpritePack spritePack;
        //Sprite attackSprite = null;
    }

    [Serializable]
    public struct CreatureSpritePack
    {
        public string creatureName;
        public Sprite fullBody;
        public Sprite fullBody_WeakPoint;
        public Sprite fullBody_Thrax;
        public Sprite fullBody_Outer;
        public Sprite cut_Attack;
        public Sprite cut_Hited;
        public Sprite cut_Avoid;
        public Sprite attackScratch;
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

    #region Effect
    public enum EffectTypes
    {
        MaxHealth,
        Speed,
        Defense,
        Accuracy,
        PrepareSpeed,
        Damage,
        Poison
    }
    

    #endregion

#region Setting

    [Serializable]
    public class OptionData
    {
        public Volume         volume;
        public Resolution     resolution;
        public int screenMode;
        public int            refreshRate;
        public int           vsync;
        
        public OptionData()
        {
            volume      = new Volume(100f, 100f, 100f);
            resolution  = new Resolution(1920, 1080);
            screenMode  = (int)FullScreenMode.FullScreenWindow;
            refreshRate = 60;
            vsync       = 0;
        }

        public void ApplySetting()
        {
            Screen.SetResolution(resolution.width, resolution.height, (FullScreenMode)screenMode, refreshRate);
            QualitySettings.vSyncCount = vsync;
            
            SystemObject.Instance.audioMixer.SetFloat("Master", Mathf.Log10(volume.master / 100) * 20);
            SystemObject.Instance.audioMixer.SetFloat("Music",  Mathf.Log10(volume.music  / 100) * 20);
            SystemObject.Instance.audioMixer.SetFloat("SFX",    Mathf.Log10(volume.sfx    / 100) * 20);
            
            Debug.Log("Setting Applied");
        }
    }

    [Serializable]
    public struct Resolution
    {
        public int width;
        public int height;

        public Resolution(int w, int h)
        {
            width  = w;
            height = h;
        }
    }

    [Serializable]
    public struct Volume
    {
        public float master;
        public float music;
        public float sfx;
        
        public Volume(float master, float music, float sfx)
        {
            this.master = master;
            this.music = music;
            this.sfx = sfx;
        }
    }
    

#endregion
}
