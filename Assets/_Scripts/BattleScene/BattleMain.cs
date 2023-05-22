using System.Collections;
using System.Collections.Generic;
using TMPro;
using TypeDefs;
using UnityEngine;
using UnityEngine.UI;
//같은 이름의 클래스가 두개라서 고정


public class BattleMain : MonoBehaviour
{
    public static BattleMain instance;

    BattleActions BA_battleActions;

    [SerializeField] float f_turnDelay = 1f;

    public bool b_playerReady;
    public bool b_enemyReady;
    public bool b_paused = true;

    [Header("Set in Inspector")]
    [SerializeField] public GameObject GO_hitImage;
    [SerializeField] public GameObject GO_action;
    [SerializeField] TMP_Text TMP_playerDamage;
    [SerializeField] TMP_Text TMP_EnemyDamage;

    [SerializeField] Image[] IMG_enemyFullBodys = new Image[5]; //순서대로 얼굴, 측면, (풀바디) WeakPoint, Thorax, Outer
    [SerializeField] Image IMG_enemySideBody;

    

    public Sprite SPR_playerAttack;
    public Sprite SPR_enemyAttack;

    [Header("Set Automatically")]
    public Slider SL_playerHP;
    public Slider SL_playerTP;
    public Slider SL_enemyHP;
    public Slider SL_enemyTP;

    public Transform TF_playerHitAnchor;
    public Transform TF_enemyHitAnchor;

    public Transform TF_playerAnimateAnchor;
    public Transform TF_enemyAnimateAnchor;

    public GameObject GO_actionList;
    public GameObject GO_attackList;

    [Header("Set Automatically : SFX")]
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

    [Header("Set in Inspector : Colors")]
    [SerializeField] public Color[] colors = new Color[5];

    public Dictionary<Parts, DmgAccText> dict_dmgAccList = new Dictionary<Parts, DmgAccText>();

    protected float f_enemySpeed;

    [Header("Set Automatically")]
    public Creature CR_Enemy;
    private Player P_player;
    PlayerStats PS_playerStats;


    void Awake()
    {
        instance = this;
        AC_playerAttackWeakPoint = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Weakpoint");
        AC_playerAttackThorax = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Thorax");
        AC_playerAttackOuter = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Outer");
        AC_playerMissed = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Miss");
        AC_enemyAttack = Resources.LoadAll<AudioClip>("SFX/EnemyAttack");
    }

    void Start()
    {
        BA_battleActions = GetComponent<BattleActions>();

        SL_playerHP = transform.Find("Character").Find("StatsArea").Find("HP").GetComponent<Slider>();
        SL_playerTP = transform.Find("Character").Find("StatsArea").Find("TP").GetComponent<Slider>();
        SL_enemyHP = transform.Find("Enemy").Find("StatsArea").Find("HP").GetComponent<Slider>();
        SL_enemyTP = transform.Find("Enemy").Find("StatsArea").Find("TP").GetComponent<Slider>();
        SL_playerTP.value = 0;

        IMG_playerHP = SL_playerHP.transform.Find("FILL_ MASK").GetChild(0).GetChild(0).GetComponent<Image>();
        IMG_playerTP = SL_playerTP.transform.Find("FILL_ MASK").GetChild(0).GetChild(0).GetComponent<Image>();
        IMG_enemyHP = SL_enemyHP.transform.Find("FILL_ MASK").GetChild(0).GetChild(0).GetComponent<Image>();
        IMG_enemyTP = SL_enemyTP.transform.Find("FILL_ MASK").GetChild(0).GetChild(0).GetComponent<Image>();

        IMG_playerHP.color  = colors[(int)SliderColor.Hp_default];
        IMG_playerTP.color  = colors[(int)SliderColor.Tp_default];
        IMG_enemyHP.color   = colors[(int)SliderColor.Hp_default];
        IMG_enemyTP.color   = colors[(int)SliderColor.Tp_default];

        TF_playerHitAnchor = transform.Find("Character").Find("HitEffect").transform;
        TF_playerAnimateAnchor = transform.Find("Character").Find("Animate").transform;
        TF_enemyHitAnchor = transform.Find("Enemy").Find("HitEffect").transform;
        TF_enemyAnimateAnchor = transform.Find("Enemy").Find("Animate").transform;

        GO_actionList = transform.Find("PlayerActions").gameObject;
        GO_attackList = transform.Find("AttackTarget").gameObject;

        playerElements = new UIModElements(
            IMG_playerHP,
            IMG_playerTP,
            TF_playerHitAnchor
        );

        enemyElements = new UIModElements(
            IMG_enemyHP,
            IMG_enemyTP,
            TF_enemyHitAnchor
        );

        dict_dmgAccList.Add(Parts.Weakpoint,
                            new DmgAccText( GO_attackList.transform.GetChild(0).Find("Percentage").GetComponent<TMP_Text>(),
                                            GO_attackList.transform.GetChild(0).Find("Damage").GetComponent<TMP_Text>()));
        dict_dmgAccList.Add(Parts.Thorax,
                            new DmgAccText(GO_attackList.transform.GetChild(1).Find("Percentage").GetComponent<TMP_Text>(),
                                            GO_attackList.transform.GetChild(1).Find("Damage").GetComponent<TMP_Text>()));
        dict_dmgAccList.Add(Parts.Outer,
                            new DmgAccText(GO_attackList.transform.GetChild(2).Find("Percentage").GetComponent<TMP_Text>(),
                                            GO_attackList.transform.GetChild(2).Find("Damage").GetComponent<TMP_Text>()));

        
    }

    void Update()
    {
        if (!b_paused && !b_playerReady && !b_enemyReady) //둘다 준비상태가 아닐 때
        {

            //speed를 기반으로 증가량을 계산.
            //0~100까지 증가량은 속도 1.0 기준으로 3초가 걸린다.
            float tmp_playerIncrement = Time.deltaTime * PS_playerStats.speed * P_player.WP_weapon.f_speedMult * (100 / f_turnDelay);
            float tmp_enemyIncrement = Time.deltaTime * f_enemySpeed * (100 / f_turnDelay);

            //증가량만큼 더해주고
            SL_playerTP.value += tmp_playerIncrement;
            SL_enemyTP.value += tmp_enemyIncrement;

            //플레이어가 포인트가 다 채워졌으면 행동을 실행하도록 잠시 멈춘다.
            if (SL_playerTP.value >= 100)
            {
                IMG_playerTP.color = colors[(int)SliderColor.Tp_hilighted];
                b_playerReady = true;
                Debug.Log("플레이어 준비");
            }
            if (SL_enemyTP.value >= 100)
            {
                IMG_enemyTP.color = colors[(int)SliderColor.Tp_hilighted];
                b_enemyReady = true;
                Debug.Log("크리쳐 준비");
            }
        }

        if (b_playerReady) GO_actionList.SetActive(true);
        else if (b_enemyReady) BA_battleActions.Attack(false);

    }

    public void StartBattleScene(ref Creature CR_Opponent)
    {
        //BattleMain과 BattleAction에서도 Enemy의 스텟을 참조해야 하므로 참조로 넘겨준다.
        BA_battleActions.CR_Enemy = CR_Enemy = CR_Opponent;

        //Enemy의 이미지를 사용하는 모든곳의 이미지를 해당 Enemy로 바꿔주고
        IMG_enemyFullBodys[0].sprite = CR_Opponent.spritePack.fullBody_WeakPoint;
        IMG_enemyFullBodys[1].sprite = CR_Opponent.spritePack.fullBody_Thrax;
        IMG_enemyFullBodys[2].sprite = CR_Opponent.spritePack.fullBody_Outer;

        //플레이어 스텟을 가져와서 저장한다. (플레이어는 일회용이 아니므로 ref 으로 넘어옴)
        P_player = GameManager.Instance.GetComponent<Player>();
        PS_playerStats = P_player.GetPlayerStats();

        //행동 포인트관련 초기화 : 전투 중간에 변경될 일이 있을까? => 있으면 f_Speed같은 경우는 ref로 넘겨줘야함
        ChangeSliderValue(true, StatsType.Hp, PS_playerStats.health);   //체력바 플레이어 체력으로 초기화
        SL_playerTP.value = PS_playerStats.prepareSpeed                 //플레이어 TP를 스텟으로 맞춰줌(선제공격용)
                            + P_player.WP_weapon.i_preparedSpeed;       //무기 스텟을 더해준다.

        SL_enemyHP.value = SL_enemyHP.maxValue = CR_Enemy.health;       //Enemt의 체력 초기값 설정 (MAX/NOW)
        SL_enemyTP.value = CR_Enemy.prepareSpeed;                       //Enemy의 TP 반영
        f_enemySpeed = CR_Enemy.speed;                                  //Enemy의 속도 반영

        UpdateDamageIndicator();                                        //공격력 표시창 업데이트
        b_paused = false;
    }

    public void ChangeSliderValue(bool b_IsPlayer, StatsType statsType, float f_val)
    {
        switch (statsType)
        {
            case StatsType.Hp:
                if (b_IsPlayer) SL_playerHP.value = f_val;
                else SL_enemyHP.value = f_val;
                break;
            case StatsType.Tp:
                if (b_IsPlayer) SL_playerTP.value = f_val;
                else SL_enemyTP.value = f_val;
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
        TMP_playerDamage.text = "DMG\n" + (PS_playerStats.damage + P_player.WP_weapon.i_damage);
        TMP_EnemyDamage.text = "DMG\n" + CR_Enemy.damage;
    }

    public void EndTurn(bool b_isPlayer) {
        ChangeSliderValue(b_isPlayer, StatsType.Tp, 0);
        if (b_isPlayer)
        {
            b_playerReady = false;
            GO_actionList.SetActive(false);
            IMG_playerTP.color = colors[(int)SliderColor.Tp_default];
        } else
        {
            IMG_enemyTP.color = colors[(int)SliderColor.Tp_default];
            b_enemyReady = false;
        }
        
    }

    public IEnumerator EndFight(bool playerWin)
    {
        PlayerController PC_player = GameObject.Find("Player").GetComponent<PlayerController>();
        b_paused = true; //행동 멈추고
        yield return new WaitForSeconds(1f);

        //화면 암전
        StartCoroutine(GameManager.Instance.CurtainModify(false, 1));
        yield return new WaitForSeconds(0.5f);

        Debug.Log("값 초기화");
        //각종 값들 초기화 해주고
        b_enemyReady = false;
        b_playerReady = false;
        ChangeSliderValue(true, StatsType.Tp, 0);
        ChangeSliderValue(false, StatsType.Tp, 0);
        
        //인벤토리 내부 값 업데이트
        InventoryManager.Instance.equippedItem.UpdateUI();
        InventoryManager.Instance.stats.UpdateUI();

        //전투씬 끄기
        PC_player.b_camControll = false;
        PC_player.ExitBattle();

        //외부 상시 스텟바 업데이트
        GameManager.Instance.ChangeStatsSlider(StatsType.Hp, PS_playerStats.health);

        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.StartCoroutine(GameManager.Instance.CurtainModify(true, 1)); //BattleMain은 사라질거니까 GM에서 실행해준다.
        gameObject.SetActive(false);
    }

    public IEnumerator AnimateAction(ActionTypes acType, bool isPlayer)
    {
        GameObject tmpGO = Instantiate(GO_action);
        Image tmpIMG = tmpGO.GetComponent<Image>();
        RectTransform tmpRect = tmpGO.GetComponent<RectTransform>();
        BattleAnimate tmpANI = tmpGO.GetComponent<BattleAnimate>();

        if (isPlayer)   
        {
            tmpGO.transform.SetParent(TF_playerAnimateAnchor);
            tmpANI.b_isPlayer = true;
            switch (acType)
            {
                case ActionTypes.Attack:
                    tmpIMG.sprite = null;
                    break;
                case ActionTypes.Hited:
                    tmpIMG.sprite = null;
                    break;
                case ActionTypes.Avoid:
                    tmpIMG.sprite = null;
                    break;
            }
            tmpRect.localPosition = new Vector2(1536, 0);
        } else
        {
            tmpGO.transform.SetParent(TF_enemyAnimateAnchor);
            switch (acType)
            {
                case ActionTypes.Attack:
                    tmpIMG.sprite = CR_Enemy.spritePack.cut_Attack;
                    break;
                case ActionTypes.Hited:
                    tmpIMG.sprite = CR_Enemy.spritePack.cut_Hited;
                    break;
                case ActionTypes.Avoid:
                    tmpIMG.sprite = CR_Enemy.spritePack.cut_Avoid;
                    break;
            }
            tmpRect.localPosition = new Vector2(1536, 0);
        }

        yield return StartCoroutine(tmpANI.Run(false, 0.2f));

        yield return StartCoroutine(BA_battleActions.LerpColor(tmpIMG, SliderColor.transparent, 0.5f, true));

        Destroy(tmpGO);
    }

}
