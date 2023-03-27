using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

//���� �̸��� Ŭ������ �ΰ��� ����
using Slider = UnityEngine.UI.Slider;
using Button = UnityEngine.UI.Button;

public class BattleMain : MonoBehaviour
{
    public bool b_Paused;

    bool b_PlayerReady;
    bool b_EnemyReady;

    [SerializeField] GameObject GO_PlayerTP;
    [SerializeField] GameObject GO_EnemyTP;
    [SerializeField] Button BTN_Player;
    [SerializeField] Button BTN_Enemy;

    Slider SL_PlayerTP;
    Slider SL_EnemyTP;

    float f_EnemyHealth = 0;
    int i_EnemyTP = 0;

    public float f_PlayerSpeed = 0.0f;
    float f_EnemySpeed = 0.0f;


    void Awake()
    {
        SL_PlayerTP = GO_PlayerTP.GetComponent<Slider>();
        SL_EnemyTP = GO_EnemyTP.GetComponent<Slider>();
    }

    void Start()
    {
        SL_PlayerTP.value = 0;
        StartBattleScene(); //�ӽ�
    }

    void Update()
    {
        if (!b_Paused)
        {
            
            //speed�� ������� �������� ���.
            //0~100���� �������� �ӵ� 1.0 �������� 3�ʰ� �ɸ���.
            float p_increment = Time.deltaTime * f_PlayerSpeed * 33.3f;
            float e_increment = Time.deltaTime * f_EnemySpeed * 33.3f;

            //��������ŭ �����ְ�
            SL_PlayerTP.value += p_increment;
            SL_EnemyTP.value += e_increment;

            //�÷��̾ ����Ʈ�� �� ä�������� �ൿ�� �����ϵ��� ��� �����.
            if (SL_PlayerTP.value >= 100)
            {
                b_Paused = true;
                b_PlayerReady = true;
                Debug.Log("�÷��̾� �غ�");
            }
            if (SL_EnemyTP.value >= 100)
            {
                b_Paused = true;
                b_EnemyReady = true;
                Debug.Log("ũ���� �غ�");
            }
        }

        if (b_PlayerReady) BTN_Player.interactable = true;
        //�÷��̾ �غ� �����̸� ���� ��ٸ���
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
        //�÷��̾�� �����̴ϱ� ���� �Ŵ������� �����´�.
        f_PlayerSpeed = GameManager.Instance.GetComponent<Player>().GetPlayerStats.f_Speed; //�ӵ�
        SL_PlayerTP.value = GameManager.Instance.GetComponent<Player>().GetPlayerStats.i_PrepareSpeed; //�����

        //���� ������ �������� �κ��ε� �ӽ÷� 1�� Default�� ���
        f_EnemyHealth = GameManager.Instance.creatures.C_Default[0].f_Health; //ü��
        f_EnemySpeed = GameManager.Instance.creatures.C_Default[0].f_Speed; //�ӵ�
        SL_EnemyTP.value = GameManager.Instance.creatures.C_Default[0].i_PrepareSpeed; //�����

        //����â Ȱ��ȭ
        this.gameObject.SetActive(true);
        
    }
}
