using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

//같은 이름의 클래스가 두개라서 고정
using Slider = UnityEngine.UI.Slider;
using Button = UnityEngine.UI.Button;

public enum StatsType
{
    Hp,
    Tp,
    Damage,
    Defense,
    Speed
}

public class BattleMain : MonoBehaviour
{
    BattleActions BA_BattleActions;

    bool b_PlayerReady;
    bool b_EnemyReady;

    [SerializeField] TMP_Text TMP_PlayerDamage;
    [SerializeField] TMP_Text TMP_EnemyDamage;

    [SerializeField] GameObject GO_ActionList;

    [Header("Set in Inspector")]
    public Slider SL_PlayerHP;
    public Slider SL_PlayerTP;
    public Slider SL_EnemyTP;
    public Slider SL_EnemyHP;

    protected float f_EnemySpeed = 0.0f;
    protected float f_PlayerSpeed = 0.0f;


    void Awake()
    {
    }

    void Start()
    {
        BA_BattleActions = GetComponent<BattleActions>();
        SL_PlayerTP.value = 0;
        StartBattleScene(GameManager.Instance.creatures.C_Default[0]); //임시

    }

    void Update()
    {
        if (!b_PlayerReady && !b_EnemyReady) //둘다 준비상태가 아닐 때
        {
            
            //speed를 기반으로 증가량을 계산.
            //0~100까지 증가량은 속도 1.0 기준으로 3초가 걸린다.
            float p_increment = Time.deltaTime * f_PlayerSpeed * 33.3f;
            float e_increment = Time.deltaTime * f_EnemySpeed * 33.3f;

            //증가량만큼 더해주고
            SL_PlayerTP.value += p_increment;
            SL_EnemyTP.value += e_increment;

            //플레이어가 포인트가 다 채워졌으면 행동을 실행하도록 잠시 멈춘다.
            if (SL_PlayerTP.value >= 100)
            {
                b_PlayerReady = true;
                Debug.Log("플레이어 준비");
            }
            if (SL_EnemyTP.value >= 100)
            {
                b_EnemyReady = true;
                Debug.Log("크리쳐 준비");
            }
        }

        if (b_PlayerReady) GO_ActionList.SetActive(true);
        else if (b_EnemyReady) BA_BattleActions.Attack(false);

    }

    void StartBattleScene(Creature CR_Opponent)
    {
        BA_BattleActions.CR_Enemy = CR_Opponent;

        //행동 포인트관련 초기화 : 전투 중간에 변경될 일이 있을까? => 있으면 f_Speed같은 경우는 ref로 넘겨줘야함
        SL_PlayerTP.value = GameManager.Instance.GetComponent<Player>().GetPlayerStats().i_PrepareSpeed;
        f_PlayerSpeed = GameManager.Instance.GetComponent<Player>().GetPlayerStats().f_Speed;

        SL_EnemyHP.value = SL_EnemyHP.maxValue = CR_Opponent.f_Health;
        SL_EnemyTP.value = CR_Opponent.i_PrepareSpeed;
        f_EnemySpeed = CR_Opponent.f_Speed-0.05f;

        TMP_PlayerDamage.text = "DMG\n" + GameManager.Instance.GetComponent<Player>().GetPlayerStats().i_Damage;
        TMP_EnemyDamage.text = "DMG\n" + CR_Opponent.i_Damage;

        //전투창 활성화
        this.gameObject.SetActive(true);        
    }

    public void ChangeSliderValue(bool b_IsPlayer, StatsType statsType, float f_val)
    {
        switch (statsType)
        {
            case StatsType.Hp:
                if (b_IsPlayer) SL_PlayerHP.value = f_val;
                else SL_EnemyHP.value = f_val;
                break;
            case StatsType.Tp:
                if (b_IsPlayer) SL_PlayerTP.value = f_val;
                else SL_EnemyTP.value = f_val;
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

    public void EndTurn(bool b_IsPlayer) {
        if (b_IsPlayer)
        {
            b_PlayerReady = false;
            GO_ActionList.SetActive(false);
        } else
        {
            b_EnemyReady = false;
        }
        
    }
}

//슬라이더는 오른쪽 Display 값이랑만 연동되어있고 실제 변수와 연결되어있지 않음
//변수값을 Slider의 Value와 연동이 필요
