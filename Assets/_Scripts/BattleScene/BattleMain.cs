using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

//같은 이름의 클래스가 두개라서 고정
using Slider = UnityEngine.UI.Slider;
using Button = UnityEngine.UI.Button;

public class BattleMain : MonoBehaviour
{
    public bool b_Paused;

    bool b_PlayerReady;
    bool b_EnemyReady;

    [SerializeField] GameObject GO_PlayerTP;
    [SerializeField] GameObject GO_EnemyTP;

    //임시
    [SerializeField] Button BTN_Player;
    [SerializeField] Button BTN_Enemy;

    [SerializeField] TMP_Text TMP_PlayerDamage;
    [SerializeField] TMP_Text TMP_EnemyDamage;

    [SerializeField] GameObject GO_ActionList;

    Slider SL_PlayerTP;
    Slider SL_EnemyTP;

    float f_EnemyHealth = 0;
    int i_EnemyDamage = 0;
    float f_EnemySpeed = 0.0f;
    int i_EmemyDef = 0;

    //플레이어는 체력을 컴포넌트의 Value값을 그대로 가져와 사용
    int i_PlayerDamage = 0;
    float f_PlayerSpeed = 0.0f;
    int i_PlayerDef = 0;
    


    void Awake()
    {
        SL_PlayerTP = GO_PlayerTP.GetComponent<Slider>();
        SL_EnemyTP = GO_EnemyTP.GetComponent<Slider>();
    }

    void Start()
    {
        SL_PlayerTP.value = 0;
        StartBattleScene(); //임시
    }

    void Update()
    {
        if (!b_Paused)
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
                b_Paused = true;
                b_PlayerReady = true;
                Debug.Log("플레이어 준비");
            }
            if (SL_EnemyTP.value >= 100)
            {
                b_Paused = true;
                b_EnemyReady = true;
                Debug.Log("크리쳐 준비");
            }
        }

        if (b_PlayerReady) GO_ActionList.SetActive(true);
        //플레이어가 준비 상태이면 적은 기다리게
        if (b_EnemyReady && !b_PlayerReady) BTN_Enemy.interactable = true;
    }

    public void PlayerButton()
    {
        BTN_Player.interactable = false;
        SL_PlayerTP.value = 0;
        b_PlayerReady = false;
        b_Paused = false;
    }

    public void EnemyButton()
    {
        BTN_Enemy.interactable = false;
        SL_EnemyTP.value = 0;
        b_EnemyReady = false;
        b_Paused = false;
    }

    void StartBattleScene()
    {
        //플레이어는 고정이니까 게임 매니저에서 가져온다.
        i_PlayerDamage = GameManager.Instance.GetComponent<Player>().GetPlayerStats.i_Damage; //공격력
        f_PlayerSpeed = GameManager.Instance.GetComponent<Player>().GetPlayerStats.f_Speed; //속도
        i_PlayerDef = GameManager.Instance.GetComponent<Player>().GetPlayerStats.i_Def; //방어력
        SL_PlayerTP.value = GameManager.Instance.GetComponent<Player>().GetPlayerStats.i_PrepareSpeed; //기민함

        TMP_PlayerDamage.text = "DMG\n" + i_PlayerDamage;

        //적의 스텟을 가져오는 부분인데 임시로 1번 Default값 사용
        f_EnemyHealth = GameManager.Instance.creatures.C_Default[0].f_Health; //체력
        i_EnemyDamage = GameManager.Instance.creatures.C_Default[0].i_AttackDamage; //공격력
        f_EnemySpeed = GameManager.Instance.creatures.C_Default[0].f_Speed; //속도
        i_EmemyDef = GameManager.Instance.creatures.C_Default[0].i_Defense; //방어력
        SL_EnemyTP.value = GameManager.Instance.creatures.C_Default[0].i_PrepareSpeed; //기민함

        TMP_EnemyDamage.text = "DMG\n" + i_EnemyDamage;


        //전투창 활성화
        this.gameObject.SetActive(true);
        
    }
}
