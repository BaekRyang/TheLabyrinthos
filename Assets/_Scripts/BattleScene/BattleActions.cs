using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BattleActions : MonoBehaviour
{
    // 정보를 받아와서 저장할 위치
    private PlayerStats PS_Player;
    public Creature CR_Enemy;

    BattleMain BM_BattleMain;

    void Start()
    {
        // PlayerStats 클래스 객체 가져오기(참조로 가져온다)
        PS_Player = GameManager.Instance.GetComponent<Player>().GetPlayerStats();

        BM_BattleMain = GetComponent<BattleMain>();
    }
    void Update()
    {
        
    }

    public void ButtonClick(string _ButtonType)
    {
        switch (_ButtonType)
        {
            case "Tab_Attack":
                Attack(true);
                break;

            case "Tab_Item":

                break;

            case "Tab_Escape":

                break;

            default:
                break;
        }
    }

    public void Attack(bool b_IsPlayer)
    {
        if (b_IsPlayer)
        {
            int damage = PS_Player.i_Damage - CR_Enemy.i_Defense;
            CR_Enemy.f_Health -= damage;
            BM_BattleMain.ChangeSliderValue(false, StatsType.Hp, CR_Enemy.f_Health);
            BM_BattleMain.ChangeSliderValue(true, StatsType.Tp, 0);
            BM_BattleMain.EndTurn(true);
            Debug.Log("ATTACK");
        } else
        {
            int damage = CR_Enemy.i_Damage - PS_Player.i_Defense;
            PS_Player.f_Health -= damage;
            BM_BattleMain.ChangeSliderValue(true, StatsType.Hp, PS_Player.f_Health);
            BM_BattleMain.ChangeSliderValue(false, StatsType.Tp, 0);
            BM_BattleMain.EndTurn(false);
            Debug.Log("Enemy ATTACK");
        }
    }
}
