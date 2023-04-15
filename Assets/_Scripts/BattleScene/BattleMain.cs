using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//같은 이름의 클래스가 두개라서 고정
using Slider = UnityEngine.UI.Slider;
using Image = UnityEngine.UI.Image;

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

    bool b_playerReady;
    bool b_enemyReady;

    [Header("Set in Inspector")]
    [SerializeField] public GameObject GO_hitImage;
    [SerializeField] TMP_Text TMP_playerDamage;
    [SerializeField] TMP_Text TMP_EnemyDamage;
    [SerializeField] GameObject GO_actionList;
    public Sprite SPR_playerAttack;
    public Sprite SPR_enemyAttack;

    public Slider SL_playerHP;
    public Slider SL_playerTP;
    public Slider SL_enemyHP;
    public Slider SL_enemyTP;
    public Transform TF_playerHitAnchor;
    public Transform TF_enemyHitAnchor;

    [Header("Set Automatically : SFX")]
    public AudioClip[] AS_playerAttack;
    public AudioClip[] AS_enemyAttack;

    [HideInInspector] public Image IMG_playerHP;
    [HideInInspector] public Image IMG_playerTP;
    [HideInInspector] public Image IMG_enemyHP;
    [HideInInspector] public Image IMG_enemyTP;

    [HideInInspector] public UIModElements playerElements;
    [HideInInspector] public UIModElements enemyElements;

    [Header("Set in Inspector : Colors")]
    [SerializeField] public Color[] colors = new Color[5];

    protected float f_enemySpeed = 0.0f;
    protected float f_playerSpeed = 0.0f;


    void Awake()
    {
        AS_playerAttack = Resources.LoadAll<AudioClip>("SFX/PlayerAttack");
        AS_enemyAttack = Resources.LoadAll<AudioClip>("SFX/EnemyAttack");
    }

    void Start()
    {
        BA_battleActions = GetComponent<BattleActions>();
        SL_playerTP.value = 0;
        StartBattleScene(GameManager.Instance.creatures.C_default[0]); //임시

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

    }

    void Update()
    {
        if (!b_playerReady && !b_enemyReady) //둘다 준비상태가 아닐 때
        {
            
            //speed를 기반으로 증가량을 계산.
            //0~100까지 증가량은 속도 1.0 기준으로 3초가 걸린다.
            float tmp_playerIncrement = Time.deltaTime * f_playerSpeed * 33.3f;
            float tmp_enemyIncrement = Time.deltaTime * f_enemySpeed * 33.3f;

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

    void StartBattleScene(Creature CR_Opponent)
    {
        BA_battleActions.CR_Enemy = CR_Opponent;

        //행동 포인트관련 초기화 : 전투 중간에 변경될 일이 있을까? => 있으면 f_Speed같은 경우는 ref로 넘겨줘야함
        SL_playerTP.value = GameManager.Instance.GetComponent<Player>().GetPlayerStats().prepareSpeed;
        f_playerSpeed = GameManager.Instance.GetComponent<Player>().GetPlayerStats().speed;

        SL_enemyHP.value = SL_enemyHP.maxValue = CR_Opponent.health;
        SL_enemyTP.value = CR_Opponent.prepareSpeed;
        f_enemySpeed = CR_Opponent.speed-0.05f;

        TMP_playerDamage.text = "DMG\n" + GameManager.Instance.GetComponent<Player>().GetPlayerStats().damage;
        TMP_EnemyDamage.text = "DMG\n" + CR_Opponent.damage;

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

    public void EndTurn(bool b_isPlayer) {
        if (b_isPlayer)
        {
            b_playerReady = false;
            GO_actionList.SetActive(false);
        } else
        {
            b_enemyReady = false;
        }
        
    }
}

//슬라이더는 오른쪽 Display 값이랑만 연동되어있고 실제 변수와 연결되어있지 않음
//변수값을 Slider의 Value와 연동이 필요
