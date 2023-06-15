using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

//같은 이름의 클래스가 두개라서 고정


public class BattleMain : MonoBehaviour
{
    public static BattleMain Instance;

    public BattleActions BA_battleActions;

    [SerializeField] float f_turnDelay = 1f;

    public bool b_playerReady;
    public bool b_enemyReady;
    public bool b_paused = true;

    [Header("Set in Inspector")]
    [SerializeField]
    private Image background;

    [SerializeField] private CanvasGroup elementGroup;

    [SerializeField] public GameObject GO_hitImage;
    [SerializeField] public GameObject GO_action;
    [SerializeField]        TMP_Text   TMP_playerDamage;
    [SerializeField]        TMP_Text   TMP_EnemyDamage;

    [SerializeField] Image[] IMG_enemyFullBodys = new Image[5]; //순서대로 얼굴, 측면, (풀바디) WeakPoint, Thorax, Outer
    [SerializeField] Image   IMG_enemyDefault;

    [SerializeField] public GameObject inventory;

    [SerializeField] private RectTransform animationAnchor;

    public Sprite     SPR_playerAttack;
    public Sprite     SPR_enemyAttack;
    public GameObject keyCard;

    [Header("Set Automatically")] public Slider SL_playerHP;
    public                               Slider SL_playerTP;
    public                               Slider SL_enemyHP;
    public                               Slider SL_enemyTP;

    public GameObject  GO_actionList;
    public GameObject  GO_attackList;
    public AudioClip[] AC_playerAttackWeakPoint;
    public AudioClip[] AC_playerAttackThorax;
    public AudioClip[] AC_playerAttackOuter;
    public AudioClip[] AC_playerMissed;
    public AudioClip[] AC_enemyAttack;

    [HideInInspector] public Image IMG_playerHP;
    [HideInInspector] public Image IMG_playerTP;
    [HideInInspector] public Image IMG_enemyHP;
    [HideInInspector] public Image IMG_enemyTP;

    //LerpColor용
    [HideInInspector] public UIModElements playerElements;
    [HideInInspector] public UIModElements enemyElements;

    [SerializeField] public RectTransform creatureAttackAnchor;
    [SerializeField] public RectTransform playerAttackAnchor;
    [DoNotSerialize] public Image         creatureAttackSprite;
    [DoNotSerialize] public Image         playerAttackSprite;
    [DoNotSerialize] public Image         playerAttackScratch;
    [DoNotSerialize] public Image         creatureAttackScratch;
    [SerializeField] public RectTransform screenEffects;
    [SerializeField] public RectTransform damageIndicate;

    [Header("Set in Inspector : Colors")]
    [SerializeField]
    public Color[] colors = new Color[5];

    public readonly Dictionary<Parts, DmgAccText> dict_dmgAccList = new Dictionary<Parts, DmgAccText>();

    protected float f_enemySpeed;

    [Header("Set Automatically")] public Creature CR_Enemy;
    private                              Player   P_player;
    PlayerStats                                   PS_playerStats;


    void Awake()
    {
        Instance                 = this;
        AC_playerAttackWeakPoint = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Weakpoint");
        AC_playerAttackThorax    = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Thorax");
        AC_playerAttackOuter     = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Outer");
        AC_playerMissed          = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Miss");
        AC_enemyAttack           = Resources.LoadAll<AudioClip>("SFX/EnemyAttack");

        creatureAttackSprite  = creatureAttackAnchor.GetChild(0).GetComponent<Image>();
        playerAttackSprite    = playerAttackAnchor.GetChild(0).GetComponent<Image>();
        playerAttackScratch   = creatureAttackAnchor.GetChild(1).GetComponent<Image>();
        creatureAttackScratch = playerAttackAnchor.GetChild(1).GetComponent<Image>();
    }

    public IEnumerator LoadSetting()
    {
        BA_battleActions = GetComponent<BattleActions>();

        elementGroup.gameObject.SetActive(true);
        elementGroup.transform.Find("Inventory").gameObject.SetActive(true);

        SL_playerHP       = transform.GetChild(1).Find("Character").Find("StatsArea").Find("HP").GetComponent<Slider>();
        SL_playerTP       = transform.GetChild(1).Find("Character").Find("StatsArea").Find("TP").GetComponent<Slider>();
        SL_enemyHP        = transform.GetChild(1).Find("Enemy").Find("StatsArea").Find("HP").GetComponent<Slider>();
        SL_enemyTP        = transform.GetChild(1).Find("Enemy").Find("StatsArea").Find("TP").GetComponent<Slider>();
        SL_playerTP.value = 0;

        IMG_playerHP = SL_playerHP.transform.Find("FILL_ MASK").GetChild(0).GetChild(0).GetComponent<Image>();
        IMG_playerTP = SL_playerTP.transform.Find("FILL_ MASK").GetChild(0).GetChild(0).GetComponent<Image>();
        IMG_enemyHP  = SL_enemyHP.transform.Find("FILL_ MASK").GetChild(0).GetChild(0).GetComponent<Image>();
        IMG_enemyTP  = SL_enemyTP.transform.Find("FILL_ MASK").GetChild(0).GetChild(0).GetComponent<Image>();

        IMG_playerHP.color = colors[(int)SliderColor.Hp_default];
        IMG_playerTP.color = colors[(int)SliderColor.Tp_default];
        IMG_enemyHP.color  = colors[(int)SliderColor.Hp_default];
        IMG_enemyTP.color  = colors[(int)SliderColor.Tp_default];

        GO_actionList = transform.GetChild(1).Find("PlayerActions").gameObject;
        GO_attackList = transform.GetChild(1).Find("AttackTarget").gameObject;

        playerElements = new UIModElements(
            IMG_playerHP,
            IMG_playerTP
        );

        enemyElements = new UIModElements(
            IMG_enemyHP,
            IMG_enemyTP
        );

        dict_dmgAccList.Add(Parts.Weakpoint,
                            new DmgAccText(GO_attackList.transform.GetChild(0).Find("Percentage").GetComponent<TMP_Text>(),
                                           GO_attackList.transform.GetChild(0).Find("Damage").GetComponent<TMP_Text>()));
        dict_dmgAccList.Add(Parts.Thorax,
                            new DmgAccText(GO_attackList.transform.GetChild(1).Find("Percentage").GetComponent<TMP_Text>(),
                                           GO_attackList.transform.GetChild(1).Find("Damage").GetComponent<TMP_Text>()));
        dict_dmgAccList.Add(Parts.Outer,
                            new DmgAccText(GO_attackList.transform.GetChild(2).Find("Percentage").GetComponent<TMP_Text>(),
                                           GO_attackList.transform.GetChild(2).Find("Damage").GetComponent<TMP_Text>()));

        inventory.SetActive(false);
        yield return null;
    }

    void Update()
    {
        if (b_paused)
            return;

        if (!b_playerReady && !b_enemyReady) //둘다 준비상태가 아닐 때
        {
            //speed를 기반으로 증가량을 계산.
            //0~100까지 증가량은 속도 1.0 기준으로 3초가 걸린다.
            float tmp_playerIncrement = Time.deltaTime * PS_playerStats.Speed * P_player.WP_weapon.f_speedMult * (100 / f_turnDelay);
            float tmp_enemyIncrement  = Time.deltaTime * f_enemySpeed         * (100                                  / f_turnDelay);

            //증가량만큼 더해주고
            SL_playerTP.value += tmp_playerIncrement;
            SL_enemyTP.value  += tmp_enemyIncrement;

            //플레이어가 포인트가 다 채워졌으면 행동을 실행하도록 잠시 멈춘다.
            if (SL_playerTP.value >= 100)
            {
                IMG_playerTP.color = colors[(int)SliderColor.Tp_hilighted];
                b_playerReady      = true;
                Debug.Log("플레이어 준비");
            }

            if (SL_enemyTP.value >= 100)
            {
                IMG_enemyTP.color = colors[(int)SliderColor.Tp_hilighted];
                b_enemyReady      = true;
                Debug.Log("크리쳐 준비");
            }
        }

        if (b_playerReady) GO_actionList.SetActive(true);
        else if (b_enemyReady) BA_battleActions.Attack(false);
        //Pause를 영향을 받지 않아서 크리쳐가 무한으로 공격하는 버그가 있었음
        //위에서 Pause에 따라서 early return하도록 수정했음
    }

    public IEnumerator StartBattleScene(Creature CR_Opponent)
    {
        //BattleMain과 BattleAction에서도 Enemy의 스텟을 참조해야 하므로 참조로 넘겨준다.
        BA_battleActions.CR_Enemy = CR_Enemy = CR_Opponent;

        //Enemy의 이미지를 사용하는 모든곳의 이미지를 해당 Enemy로 바꿔주고
        IMG_enemyFullBodys[0].sprite = CR_Opponent.spritePack.fullBody_WeakPoint;
        IMG_enemyFullBodys[1].sprite = CR_Opponent.spritePack.fullBody_Thrax;
        IMG_enemyFullBodys[2].sprite = CR_Opponent.spritePack.fullBody_Outer;

        IMG_enemyDefault.transform.GetChild(0).GetComponent<Image>().sprite =
            IMG_enemyDefault.GetComponent<Image>().sprite =
                CR_Opponent.spritePack.fullBody;

        creatureAttackScratch.sprite = CR_Opponent.spritePack.attackScratch;
        // creatureAttackScratch.sprite = P_player.attackScratch; //지금은 무기별 이펙트가 없으니

        //플레이어 스텟을 가져와서 저장한다. (플레이어는 일회용이 아니므로 ref 으로 넘어옴)
        P_player       = Player.Instance;
        PS_playerStats = P_player.PS_playerStats;

        //행동 포인트관련 초기화 : 전투 중간에 변경될 일이 있을까? => 있으면 f_Speed같은 경우는 ref로 넘겨줘야함
        ChangeSliderValue(true, StatsType.Hp, PS_playerStats.Health); //체력바 플레이어 체력으로 초기화
        SL_playerTP.value = PS_playerStats.PrepareSpeed               //플레이어 TP를 스텟으로 맞춰줌(선제공격용)
                          + P_player.WP_weapon.i_preparedSpeed;       //무기 스텟을 더해준다.

        SL_enemyHP.value = SL_enemyHP.maxValue = CR_Enemy.health; //Enemy의 체력 초기값 설정 (MAX/NOW)
        SL_enemyTP.value = CR_Enemy.prepareSpeed;                 //Enemy의 TP 반영
        f_enemySpeed     = CR_Enemy.speed;                        //Enemy의 속도 반영

        // UpdateDamageIndicator();                                        //공격력 표시창 업데이트
        b_paused = false;

        //damage indicator 안에 있는 자식 오브젝트들이 있으면 전부 삭제
        foreach (Transform child in damageIndicate)
            DestroyImmediate(child.gameObject);

        InventoryManager.Instance.openedInventory = inventory.transform.GetComponentInChildren<Inventory>();
        yield return StartCoroutine(Lerp.LerpValue(color => background.color = color,
                                                   new Color(.5f, .5f, .5f, 0),
                                                   new Color(.5f, .5f, .5f, 1),
                                                   1,
                                                   Color.Lerp,
                                                   Lerp.EaseIn));
    }

    public void ChangeSliderValue(bool b_IsPlayer, StatsType statsType, float f_val)
    {
        switch (statsType)
        {
            case StatsType.Hp:
                if (b_IsPlayer)
                {
                    SL_playerHP.maxValue = PS_playerStats.MaxHealth;
                    SL_playerHP.value    = f_val;
                }
                else SL_enemyHP.value = f_val;

                break;
            case StatsType.Tp:
                if (b_IsPlayer) SL_playerTP.value = f_val;
                else SL_enemyTP.value             = f_val;
                break;
            case StatsType.Damage:
                break;
            case StatsType.Defense:
                break;
            case StatsType.Speed:
                break;
        }
    }

    public void UpdateDamageIndicator()
    {
        TMP_playerDamage.text = "DMG\n" + (PS_playerStats.Damage + P_player.WP_weapon.i_damage);
        TMP_EnemyDamage.text  = "DMG\n" + CR_Enemy.damage;
    }

    public void EndTurn(bool b_isPlayer)
    {
        ChangeSliderValue(b_isPlayer, StatsType.Tp, 0);
        if (b_isPlayer)
        {
            b_playerReady = false;
            GO_actionList.SetActive(false);
            IMG_playerTP.color = colors[(int)SliderColor.Tp_default];
        }
        else
        {
            IMG_enemyTP.color = colors[(int)SliderColor.Tp_default];
            b_enemyReady      = false;
        }

        b_paused = false;
    }

    public IEnumerator EndFight(bool playerWin)
    {
        PlayerController PC_player = GameObject.Find("Player").GetComponent<PlayerController>();
        b_paused = true; //행동 멈추고

        if (PC_player.prevRoom.GetComponent<RoomController>().RT_roomType == RoomType.KeyRoom)
            Instantiate(keyCard, PC_player.prevRoom.transform.position + Vector3.up, Quaternion.identity);

        //화면 암전
        yield return StartCoroutine(GameManager.Instance.CurtainModify(false, 1));

        Debug.Log("값 초기화");
        //각종 값들 초기화 해주고
        b_enemyReady  = false;
        b_playerReady = false;
        ChangeSliderValue(true,  StatsType.Tp, 0);
        ChangeSliderValue(false, StatsType.Tp, 0);

        //인벤토리 내부 값 업데이트
        InventoryManager.Instance.equippedItem.UpdateUI();
        // InventoryManager.Instance.stats.UpdateUI();

        //애니메이션 되감기
        yield return BA_battleActions.MMF_player[2].PlayFeedbacksCoroutine(transform.position);

        //전투씬 끄기
        PC_player.b_camControll = false;
        PC_player.ExitBattle();

        P_player.PS_playerStats.Exp += GameManager.Instance.levelEXP;

        GameManager.Instance.statistics[Statistics.KilledEnemy]++;

        // //외부 상시 스텟바 업데이트
        // GameManager.Instance.UpdateStatsSlider(StatsType.Hp);
        // GameManager.Instance.UpdateStatsSlider(StatsType.Exp);

        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.StartCoroutine(GameManager.Instance.CurtainModify(true, 1)); //BattleMain은 사라질거니까 GM에서 실행해준다.
        gameObject.SetActive(false);
    }

    public IEnumerator PlayAttackScratch(RectTransform targetAnchor, float duration, Sprite hitSprite, AudioClip clip) //0에서 100까지 채우기
    {
        Random     tmpRand  = new Random();
        GameObject tmpGO    = Instantiate(GO_hitImage, targetAnchor);
        Image      hitImage = tmpGO.GetComponent<Image>();

        hitImage.sprite                  = hitSprite;                                                      //공격자에 따라서 공격 이미지 바꿔주고
        hitImage.transform.localPosition = new Vector3(tmpRand.Next(-50, 50), tmpRand.Next(-150, 150), 0); //위치는 랜덤
        tmpGO.transform.Rotate(0, 0, tmpRand.Next(180));                                                   //방향도 랜덤

        tmpGO.GetComponent<AudioSource>().clip = clip;
        tmpGO.GetComponent<AudioSource>().Play();
        //공격 소리중 아무거나 가져와서 재생


        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            hitImage.fillAmount =  Mathf.Lerp(0, 1, (elapsedTime / duration));
            elapsedTime         += Time.deltaTime;
            yield return null;
        }

        hitImage.fillAmount = 1;

        StartCoroutine(
            Lerp.LerpValue(
                value => hitImage.color = value,
                colors[(int)SliderColor.transparent],
                colors[(int)SliderColor.Tp_default],
                .5f,
                Color.Lerp
            )
        );

        yield return new WaitForSeconds(0.5f);
        Destroy(tmpGO);
    }
}