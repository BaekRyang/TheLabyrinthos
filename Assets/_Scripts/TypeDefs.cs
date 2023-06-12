using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
            damage     = dmg;
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
            damage   = dmg;
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
        private float health       = 100.0f;
        private float maxHealth    = 100.0f;
        private int   exp          = 0;
        private float speed        = 1.0f;
        private int   defense      = 5;
        private int   prepareSpeed = 0;
        private float accuracyMult = 1;
        private int   damage       = 10;
        private int   level        = 1;

        public float Health
        {
            get => health;
            set
            {
                //현재 씬이 "Level"이라면 
                if (SceneManager.GetActiveScene().name == "Level")
                {
                    if (value - health >= 1) //체력회복일 경우
                        GameManager.Instance.statistics[Statistics.Healed] += value - health;
                
                    health = (float) Math.Round(value, 1);
                
                    GameManager.Instance.UpdateStatsSlider(StatsType.Hp);
                    if (InventoryManager.Instance.b_UIOpen)
                        InventoryManager.Instance.stats.UpdateUI();
                }
                else
                {
                    health = value;
                }
            }
        }

        public float MaxHealth
        {
            get => maxHealth;
            set
            {
                if (SceneManager.GetActiveScene().name == "Level")
                {
                    maxHealth = value;
                    GameManager.Instance.UpdateStatsSlider(StatsType.Hp);
                    if (InventoryManager.Instance.b_UIOpen)
                        InventoryManager.Instance.stats.UpdateUI();
                }
                else
                {
                    maxHealth = value;
                }
            }
        }

        public float MissingHealth => MaxHealth - Health;

        public int Exp
        {
            get => exp;
            set
            {
                if (SceneManager.GetActiveScene().name == "Level")
                {
                    exp += value;
                    if (Exp >= 100)
                    {
                        exp -= 100;
                        Level++;
                        MaxHealth += 5;
                        Health    += MissingHealth / 2;
                        Damage    += 2;
                        Defense   += 2;
                    }

                    GameManager.Instance.UpdateStatsSlider(StatsType.Exp);
                    if (InventoryManager.Instance.b_UIOpen)
                        InventoryManager.Instance.stats.UpdateUI();
                }
                else
                {
                    exp = value;
                }
            }
        }

        public float Speed
        {
            get => speed;
            set
            {
                if (SceneManager.GetActiveScene().name == "Level")
                {
                    speed = value;
                    if (InventoryManager.Instance.b_UIOpen)
                        InventoryManager.Instance.stats.UpdateUI();
                }
                else
                {
                    speed = value;
                }
            }
        }

        public int Defense
        {
            get => defense;
            set
            {
                if (SceneManager.GetActiveScene().name == "Level")
                {
                    defense = value;
                    if (InventoryManager.Instance.b_UIOpen)
                        InventoryManager.Instance.stats.UpdateUI();
                }
                else
                {
                    defense = value;
                }
            }
        }

        public int PrepareSpeed
        {
            get => prepareSpeed;
            set
            {
                prepareSpeed = value;
            }
        }

        public float AccuracyMult
        {
            get => accuracyMult;
            set
            {
                accuracyMult = value;
            }
        }

        public int Damage
        {
            get => damage;
            set
            {
                damage = value;
            }
        }

        public int Level
        {
            get => level;
            set
            {
                if (SceneManager.GetActiveScene().name == "Level")
                {
                    level = value;
                    if (InventoryManager.Instance.b_UIOpen)
                        InventoryManager.Instance.stats.UpdateUI();
                }
                else
                {
                    level = value;
                }
            }
        }
    }

#endregion

#region Creature

    [Serializable]
    public class Creature
    {
        public Creature(Creature other)
        {
            health       = other.health;
            speed        = other.speed;
            defense      = other.defense;
            prepareSpeed = other.prepareSpeed;
            damage       = other.damage;
            spritePack   = other.spritePack;
        }

        public Creature(int atk, int def, float hp, float spd, int pspd, CreatureSpritePack csp)
        {
            damage       = atk;
            defense      = def;
            health       = hp;
            speed        = spd;
            prepareSpeed = pspd;
            spritePack   = csp;
        }

        public Creature(Creature creature, CreatureSpritePack csp)
        {
            health       = creature.health;
            speed        = creature.speed;
            defense      = creature.defense;
            prepareSpeed = creature.prepareSpeed;
            damage       = creature.damage;
            spritePack   = csp;
        }

        public int   damage  = 1;
        public int   defense = 5;
        public float health  = 100.0f;
        public float speed   = 1.0f;
        public int   prepareSpeed;

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

    public enum ObjectType
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
        public Volume     volume;
        public Resolution resolution;
        public int        screenMode;
        public int        refreshRate;
        public int        vsync;

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
            this.music  = music;
            this.sfx    = sfx;
        }
    }

#endregion

#region SystemObject

    public class SaveData
    {
        public string                                 seed;
        public int                                    level;
        public int                                    stayingRoomIndex;
        public PlayerStats                            playerStats;
        public Weapon                                 equippedWeapon;
        public List<Effect>                           appliedEffects;
        public Dictionary<int, int>                   inventroy;
        public Dictionary<int, List<Weapon>>          weaponInventory;
        public List<int>                              roomVisited;
        public Dictionary<int, Dictionary<int, bool>> interactedObject;
        public Dictionary<int, bool>                  creatureKilled;
        public Dictionary<Statistics, float>          statistics;


        public SaveData()
        {
            seed              = "";
            level             = 0;
            stayingRoomIndex  = 0;
            playerStats       = new PlayerStats();
            equippedWeapon    = new Weapon();
            appliedEffects    = new List<Effect>();
            inventroy         = new Dictionary<int, int>();
            weaponInventory   = new Dictionary<int, List<Weapon>>();
            roomVisited       = new List<int>();
            interactedObject  = new Dictionary<int, Dictionary<int, bool>>();
            creatureKilled    = new Dictionary<int, bool>();
            statistics        = new Dictionary<Statistics, float>();
        }

        public void LoadToSave()
        {
            seed             = GameManager.Instance.Seed;
            level            = GameManager.Instance.i_level;
            stayingRoomIndex = Player.Instance.GetComponent<PlayerController>().roomIndex;
            playerStats      = Player.Instance.PS_playerStats;
            equippedWeapon   = Player.Instance.WP_weapon;
            appliedEffects   = Player.Instance.effectList;
            inventroy        = InventoryManager.inventory;
            weaponInventory  = InventoryManager.weaponInventory;

            roomVisited = new List<int>();
            interactedObject = new Dictionary<int, Dictionary<int, bool>>();
            creatureKilled = new Dictionary<int, bool>();

            foreach (var room in Minimap.Instance.GO_anchor.transform.GetComponentsInChildren<GoodTrip>())
            {
                if (room.entered)
                    roomVisited.Add(room.index);
            }


            var map = GameManager.Instance.GetComponent<RoomCreation>().roomMap;
            
            foreach (var (index, roomNode) in map)
            {
                if (!roomVisited.Contains(index)) //방문 안한방은 저장 안해도됨
                    continue;

                interactedObject.Add(index, new Dictionary<int, bool>()); //방문은 했으니깐 안에 오브젝트들의 상태를 저장해야함
            
                var interactables = roomNode.RoomObject.transform.GetComponentsInChildren<Interactable>(); //Interactable 컴포넌트를 가진 오브젝트들을 가져옴
            
                for (var i = 0; i < interactables.Length; i++) //오브젝트들을 돌면서 상태를 저장
                {
                    var  interactable = interactables[i];
                    bool interacted   = false;

                    if (interactable.type == ObjectType.Door || interactable.type == ObjectType.Item)
                    {
                        //Interact를 했던 오브젝트들만 저장 (일반 문과 아이템만 저장)
                        if (interactable.interacted)
                            interacted = true;
                    }
                    
                    interactedObject[index].Add(i, interacted);
                }
            }

            foreach (var (index, roomNode) in map)
            {
                if (roomVisited.Contains(index))
                    creatureKilled.Add(index, roomNode.RoomObject.GetComponent<RoomController>().hasCreature);
            }
            
            statistics = GameManager.Instance.statistics;
        }
    }

#endregion

    [Serializable]
    public enum Statistics
    {
        KilledEnemy,
        DealtDamage,
        TakenDamage,
        Healed,
        UsedItem,
        MissedAttack,
        AvoidedAttack,
        EnteredRoom,
    }
}