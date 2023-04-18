using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TypeDefs;

//같은 이름의 클래스가 두개라서 고정
using Slider = UnityEngine.UI.Slider;
using Image = UnityEngine.UI.Image;

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

public class BattleMain : MonoBehaviour
{
    BattleActions BA_battleActions;

    [SerializeField] float f_turnDelay = 1f;

    bool b_playerReady;
    bool b_enemyReady;

    [Header("Set in Inspector")]
    [SerializeField] public GameObject GO_hitImage;
    [SerializeField] TMP_Text TMP_playerDamage;
    [SerializeField] TMP_Text TMP_EnemyDamage;

    [SerializeField] Image[] IMG_enemyFullBodys = new Image[3];
    [SerializeField] Image IMG_enemySideBody;
    [SerializeField] Image IMG_enemyFace;

    public GameObject GO_actionList;
    public GameObject GO_attackList;

    public Sprite SPR_playerAttack;
    public Sprite SPR_enemyAttack;

    public Slider SL_playerHP;
    public Slider SL_playerTP;
    public Slider SL_enemyHP;
    public Slider SL_enemyTP;

    public Transform TF_playerHitAnchor;
    public Transform TF_enemyHitAnchor;

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

    [HideInInspector] public UIModElements playerElements;
    [HideInInspector] public UIModElements enemyElements;

    [Header("Set in Inspector : Colors")]
    [SerializeField] public Color[] colors = new Color[5];

    public Dictionary<Parts, DmgAccText> dict_dmgAccList = new Dictionary<Parts, DmgAccText>();

    protected float f_enemySpeed = 0.0f;
    Creature CR_Enemy;
    PlayerStats PS_playerStats;


    void Awake()
    {
        AC_playerAttackWeakPoint = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Weakpoint");
        AC_playerAttackThorax = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Thorax");
        AC_playerAttackOuter = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Outer");
        AC_playerMissed = Resources.LoadAll<AudioClip>("SFX/PlayerAttack/Miss");
        AC_enemyAttack = Resources.LoadAll<AudioClip>("SFX/EnemyAttack");
    }

    void Start()
    {
        BA_battleActions = GetComponent<BattleActions>();
        SL_playerTP.value = 0;
        StartBattleScene(ref GameManager.Instance.creatures.C_default[5]); //임시

        IMG_playerHP = SL_playerHP.transform.Find("Fill Area").GetChild(0).GetComponent<Image>();
        IMG_playerTP = SL_playerTP.transform.Find("Fill Area").GetChild(0).GetComponent<Image>();
        IMG_enemyHP = SL_enemyHP.transform.Find("Fill Area").GetChild(0).GetComponent<Image>();
        IMG_enemyTP = SL_enemyTP.transform.Find("Fill Area").GetChild(0).GetComponent<Image>();

        IMG_playerHP.color  = colors[(int)SliderColor.Hp_default];
        IMG_playerTP.color  = colors[(int)SliderColor.Tp_default];
        IMG_enemyHP.color   = colors[(int)SliderColor.Hp_default];
        IMG_enemyTP.color   = colors[(int)SliderColor.Tp_default];

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
        if (!b_playerReady && !b_enemyReady) //둘다 준비상태가 아닐 때
        {

            //speed를 기반으로 증가량을 계산.
            //0~100까지 증가량은 속도 1.0 기준으로 3초가 걸린다.
            float tmp_playerIncrement = Time.deltaTime * PS_playerStats.speed * (100 / f_turnDelay);
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

    void StartBattleScene(ref Creature CR_Opponent)
    {
        //BattleMain과 BattleAction에서도 Enemy의 스텟을 참조해야 하므로 참조로 넘겨준다.
        BA_battleActions.CR_Enemy = CR_Enemy = CR_Opponent;

        //Enemy의 이미지를 사용하는 모든곳의 이미지를 해당 Enemy로 바꿔주고
        //foreach (Image img in IMG_enemyFullBodys) img.sprite    = CR_Enemy.fullBody;
        //IMG_enemySideBody.sprite                                = CR_Enemy.sideBody;
        //IMG_enemyFace.sprite                                    = CR_Enemy.face;

        //플레이어 스텟을 가져와서 저장한다. (플레이어는 일회용이 아니므로 ref 으로 넘어옴)
        PS_playerStats = GameManager.Instance.GetComponent<Player>().GetPlayerStats();

        //행동 포인트관련 초기화 : 전투 중간에 변경될 일이 있을까? => 있으면 f_Speed같은 경우는 ref로 넘겨줘야함
        ChangeSliderValue(true, StatsType.Hp, PS_playerStats.health);   //체력바 플레이어 체력으로 초기화
        SL_playerTP.value = PS_playerStats.prepareSpeed;                //플레이어 TP를 스텟으로 맞춰줌(선제공격용)

        SL_enemyHP.value = SL_enemyHP.maxValue = CR_Enemy.health;       //Enemt의 체력 초기값 설정 (MAX/NOW)
        SL_enemyTP.value = CR_Enemy.prepareSpeed;                       //Enemy의 TP 반영
        f_enemySpeed = CR_Enemy.speed;                                  //Enemy의 속도 반영

        UpdateDamageIndicator();                                        //공격력 표시창 업데이트

        //전투창 활성화
        this.gameObject.SetActive(true);        
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
            default:
                break;
        }
    }

    public void UpdateDamageIndicator()
    {
        TMP_playerDamage.text = "DMG\n" + PS_playerStats.damage;
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

    
}
