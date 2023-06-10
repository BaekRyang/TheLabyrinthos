using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class BattleActions : MonoBehaviour
{
    const int CONST_DEF     = 20; //방어 상수
    const int BASE_ACCURACY = 75; //기본 정확도

    const float WEAKPOINT_DMG = 1.55f;
    const float WEAKPOINT_ACC = 0.75f;
    const float THORAX_DMG    = 1.00f;
    const float THORAX_ACC    = 1.00f;
    const float OUTER_DMG     = 0.80f;
    const float OUTER_ACC     = 1.30f;

    private float                 weakpointACC;
    private float                 thoraxACC;
    private float                 outerACC;
    Dictionary<Parts, AttackPair> dict_attackTable = new Dictionary<Parts, AttackPair>();

    //정보를 받아와서 저장할 위치
    private Player      P_player;
    private PlayerStats PS_playerStats;
    public  Creature    CR_Enemy;

    BattleMain BM_BattleMain;
    Random     rand = new Random();

    [Header("Set In Inspector")]
    [SerializeField]
    public MMF_Player[] MMF_player;

    [SerializeField] public  GameObject  damageObject;
    [SerializeField] public  Material    critMaterial;
    [SerializeField] public  Color       critColor;
    [SerializeField] public  Color       missedColor;
    [DoNotSerialize] private AudioSource audioSource;

    public IEnumerator LoadSetting()
    {
        P_player       = Player.Instance;
        PS_playerStats = P_player.GetPlayerStats();

        BM_BattleMain = GetComponent<BattleMain>();

        dict_attackTable.Add(Parts.Weakpoint, new AttackPair(WEAKPOINT_DMG, WEAKPOINT_ACC));
        dict_attackTable.Add(Parts.Thorax,    new AttackPair(THORAX_DMG,    THORAX_ACC));
        dict_attackTable.Add(Parts.Outer,     new AttackPair(OUTER_DMG,     OUTER_ACC));

        weakpointACC = BASE_ACCURACY * dict_attackTable[Parts.Weakpoint].accuracy;
        thoraxACC    = BASE_ACCURACY * dict_attackTable[Parts.Thorax].accuracy;
        outerACC     = BASE_ACCURACY * dict_attackTable[Parts.Outer].accuracy;

        
        audioSource = GetComponent<AudioSource>();
        gameObject.SetActive(false); //전부 로딩 되면 오브젝트 끄기
        yield return null;
    }

    public void ButtonClick(string _ButtonType)
    {
        switch (_ButtonType)
        {
            //메인 행동 3가지
            case "Tab_Attack":
                SetDmgNAcc();
                BM_BattleMain.GO_attackList.SetActive(true);
                BM_BattleMain.GO_actionList.SetActive(false);
                break;

            case "Tab_Item":
                BM_BattleMain.inventory.SetActive(true);
                BM_BattleMain.inventory.transform.GetChild(0).GetComponent<Inventory>().UpdateInventory();
                break;

            case "Tab_Escape":
                break;

            //Attack 행동 4가지
            case "Attack_Head":
                Attack(true, Parts.Weakpoint);
                break;
            case "Attack_Thorax":
                Attack(true, Parts.Thorax);
                break;
            case "Attack_Outer":
                Attack(true, Parts.Outer);
                break;
            case "Attack_Back":
                BM_BattleMain.GO_attackList.SetActive(false);
                BM_BattleMain.GO_actionList.SetActive(true);
                break;
            case "Inventory_Back":
                BM_BattleMain.inventory.SetActive(false);
                break;
        }
    }

    public void ItemUsed()
    {
        BM_BattleMain.ChangeSliderValue(true, StatsType.Hp, PS_playerStats.Health);
        P_player.ConsumeTurn();
        BM_BattleMain.EndTurn(true);
        BM_BattleMain.GO_attackList.SetActive(false);
        BM_BattleMain.GO_actionList.SetActive(false);
        BM_BattleMain.inventory.SetActive(false);
    }

    public void Attack(bool b_IsPlayer, Parts part = Parts.Thorax)
    {
        int       randInt = rand.Next(101);
        bool      isHit   = false;
        BM_BattleMain.b_paused = true;
        float damage = 0;

        if (b_IsPlayer)
        {
            int accInt = (int)(BASE_ACCURACY * dict_attackTable[part].accuracy * P_player.WP_weapon.f_accuracyMult);

            if (randInt < //랜덤 (0~100)
                accInt)   //기본 정확도 x 부위 정확도 계수
            {
                isHit = true;

                damage = ((PS_playerStats.Damage + P_player.WP_weapon.i_damage) *          //플레이어 공격력
                          (dict_attackTable[part].damage)                       *          //부위 데미지 계수
                          (1 - CR_Enemy.defense / (float)(CR_Enemy.defense + CONST_DEF))); //방어력 계산

                double d_damageRange = P_player.WP_weapon.i_damageRange;
                damage += (float)(rand.NextDouble() * (d_damageRange * 2) - d_damageRange);

                damage          =  Mathf.Round(damage * 10f) / 10f;
                CR_Enemy.health -= damage;
                CR_Enemy.health =  Mathf.Round(CR_Enemy.health * 10f) / 10f;
                BM_BattleMain.ChangeSliderValue(false, StatsType.Hp, CR_Enemy.health);

                StartCoroutine(LerpColor(BM_BattleMain.enemyElements, //해당 Target의 Image를
                                         SliderColor.Hp_hilighted,    //해당 색으로 바꿨다가
                                         1f));                        //해당 초 동안 돌아온다.
                //맞으면 타격음으로
                switch (part)
                {
                    case Parts.Weakpoint:
                        audioSource.clip = BM_BattleMain.AC_playerAttackWeakPoint[rand.Next(BM_BattleMain.AC_playerAttackWeakPoint.Length)];
                        break;
                    case Parts.Thorax:
                        audioSource.clip = BM_BattleMain.AC_playerAttackThorax[rand.Next(BM_BattleMain.AC_playerAttackThorax.Length)];
                        break;
                    case Parts.Outer:
                        audioSource.clip = BM_BattleMain.AC_playerAttackOuter[rand.Next(BM_BattleMain.AC_playerAttackOuter.Length)];
                        break;
                    default:
                        audioSource.clip = BM_BattleMain.AC_playerAttackThorax[rand.Next(BM_BattleMain.AC_playerAttackThorax.Length)];
                        break;
                }

                //내구도 하나 빼주기
                P_player.WP_weapon.ConsumeDurability();
            }
            else
            {
                //빗나가면 다른 소리로
                audioSource.clip = BM_BattleMain.AC_playerMissed[rand.Next(BM_BattleMain.AC_playerMissed.Length)];
            }

            P_player.ConsumeTurn(); //턴 하나 소모

            Debug.Log("Player : " + randInt + " > " + accInt);
            BM_BattleMain.GO_attackList.SetActive(false);
            BM_BattleMain.GO_actionList.SetActive(false);

            // StartCoroutine(
            //     BM_BattleMain.PlayAttackScratch(BM_BattleMain.creatureAttackAnchor,
            //                                     0.1f,
            //                                     BM_BattleMain.SPR_playerAttack,
            //                                     clip)
            // );
        }
        else
        {
            if (randInt <      //랜덤 (0~100)
                BASE_ACCURACY) //**크리쳐별 정확도를 가져와서 써야함
            {
                isHit = true;

                damage                =  (CR_Enemy.damage * (1 - PS_playerStats.Defense / (float)(PS_playerStats.Defense + CONST_DEF)));
                damage                =  Mathf.Round(damage                             * 10f) / 10f;
                PS_playerStats.Health -= damage;
                PS_playerStats.Health =  Mathf.Round(PS_playerStats.Health * 10f) / 10f;
                BM_BattleMain.ChangeSliderValue(true, StatsType.Hp, PS_playerStats.Health);

                StartCoroutine(LerpColor(BM_BattleMain.playerElements,
                                         SliderColor.Hp_hilighted,
                                         1f));


                audioSource.clip = BM_BattleMain.AC_enemyAttack[rand.Next(BM_BattleMain.AC_enemyAttack.Length)];
            }
            else
            {
                audioSource.clip = BM_BattleMain.AC_playerMissed[rand.Next(BM_BattleMain.AC_playerMissed.Length)];
            }


            Debug.Log("Creature : " + randInt + " > " + BASE_ACCURACY);
            // StartCoroutine(
            //     BM_BattleMain.PlayAttackScratch(BM_BattleMain.playerAttackAnchor,
            //                                     0.1f,
            //                                     BM_BattleMain.SPR_enemyAttack,
            //                                     clip)
            // );
        }

        StartCoroutine(AnimateAction(isHit ? ActionTypes.Attack : ActionTypes.Missed, b_IsPlayer, damage, false));
    }

    IEnumerator LerpColor(UIModElements targetElements, SliderColor to, float duration, bool isEndColor = false)
    {
        Color startColor;
        Color endColor;
        if (isEndColor) //Param으로 받는 Color로 끝나야 하는가?
        {
            startColor = targetElements.hpSlider.color;
            endColor   = BM_BattleMain.colors[(int)to];
        }
        else
        {
            startColor = BM_BattleMain.colors[(int)to];
            endColor   = targetElements.hpSlider.color;
        }

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            targetElements.hpSlider.color =  Color.Lerp(startColor, endColor, (elapsedTime / duration));
            elapsedTime                   += Time.deltaTime;
            yield return null;
        }

        targetElements.hpSlider.color = endColor;
    }

    public IEnumerator LerpColor(Image targetImage, SliderColor to, float duration, bool isEndColor) //이미지 인수로 하는 오버로딩
    {
        Color startColor;
        Color endColor;
        if (isEndColor) //Param으로 받는 Color로 끝나야 하는가?
        {
            startColor = targetImage.color;
            endColor   = BM_BattleMain.colors[(int)to];
        }
        else
        {
            startColor = BM_BattleMain.colors[(int)to];
            endColor   = targetImage.color;
        }

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            targetImage.color =  Color.Lerp(startColor, endColor, (elapsedTime / duration));
            elapsedTime       += Time.deltaTime;
            yield return null;
        }

        targetImage.color = endColor;
    }

    private void SetDmgNAcc() //공격 상세창 값 초기화용
    {
        var typeDict = BM_BattleMain.dict_dmgAccList;
        typeDict[Parts.Weakpoint].percentage.text = Mathf.Clamp((weakpointACC * P_player.WP_weapon.f_accuracyMult), 0, 100) + "%";
        typeDict[Parts.Thorax].percentage.text    = Mathf.Clamp((thoraxACC    * P_player.WP_weapon.f_accuracyMult), 0, 100) + "%";
        typeDict[Parts.Outer].percentage.text     = Mathf.Clamp((outerACC     * P_player.WP_weapon.f_accuracyMult), 0, 100) + "%";


        var baseDamage  = PS_playerStats.Damage + P_player.WP_weapon.i_damage;
        var creatureDef = 1                     - CR_Enemy.defense                               / (float)(CR_Enemy.defense + CONST_DEF);
        typeDict[Parts.Weakpoint].damage.text = ((baseDamage - P_player.WP_weapon.i_damageRange) * dict_attackTable[Parts.Weakpoint].damage * (creatureDef)).ToString("0.##") + " ~ " + ((baseDamage + P_player.WP_weapon.i_damageRange) * dict_attackTable[Parts.Weakpoint].damage * (creatureDef)).ToString("0.##") + " DMG";
        typeDict[Parts.Thorax].damage.text    = ((baseDamage - P_player.WP_weapon.i_damageRange) * dict_attackTable[Parts.Thorax].damage    * (creatureDef)).ToString("0.##") + " ~ " + ((baseDamage + P_player.WP_weapon.i_damageRange) * dict_attackTable[Parts.Thorax].damage    * (creatureDef)).ToString("0.##") + " DMG";
        typeDict[Parts.Outer].damage.text     = ((baseDamage - P_player.WP_weapon.i_damageRange) * dict_attackTable[Parts.Outer].damage     * (creatureDef)).ToString("0.##") + " ~ " + ((baseDamage + P_player.WP_weapon.i_damageRange) * dict_attackTable[Parts.Outer].damage     * (creatureDef)).ToString("0.##") + " DMG";
    }

    private IEnumerator AnimateAction(ActionTypes acType, bool isPlayer, float damage, bool crit = false)
    {
        var targetMMF = isPlayer ? MMF_player[0] : MMF_player[1];
        var dmgObject = Instantiate(damageObject,
                                    transform.position + (isPlayer ? Vector3.left * 120: Vector3.right * 550),
                                    Quaternion.identity,
                                    BM_BattleMain.damageIndicate
                                    );
        var dmgText   = dmgObject.GetComponentInChildren<TMP_Text>();
        var dmgMMF    = dmgObject.GetComponent<MMF_Player>();
        dmgMMF.Initialization();

        switch (acType)
        {
            case ActionTypes.Attack:
                BM_BattleMain.creatureAttackSprite.sprite = isPlayer ? CR_Enemy.spritePack.cut_Hited : CR_Enemy.spritePack.cut_Attack;
                dmgText.text                              = damage.ToString("0.#");
                if (crit)
                {
                    dmgObject.transform.GetChild(1).gameObject.SetActive(true);
                    dmgText.fontMaterial = critMaterial;
                    dmgText.color        = critColor;
                }
                // BM_BattleMain.playerAttackSprite.sprite   = isPlayer ? P_player.cut_Attack : P_player.cut_Hited;
                break;
            case ActionTypes.Missed:
                BM_BattleMain.creatureAttackSprite.sprite = isPlayer ? CR_Enemy.spritePack.cut_Avoid : CR_Enemy.spritePack.cut_Attack;
                dmgText.text                              = "빗나감!";
                dmgText.color                             = missedColor;
                
                // BM_BattleMain.playerAttackSprite.sprite   = isPlayer ? P_player.cut_Attack : P_player.cut_Avoid;
                break;
        }

        BM_BattleMain.playerAttackScratch.gameObject.SetActive(isPlayer);
        BM_BattleMain.creatureAttackScratch.gameObject.SetActive(!isPlayer);

        dmgMMF.PlayFeedbacks();
        targetMMF.PlayFeedbacks();
        audioSource.Play();
        Debug.Log("WaitStart");
        yield return new WaitForSeconds(targetMMF.TotalDuration / 2f);
        Debug.Log("WaitEnd");

        //애니메이션 출력중 전투 종료 조건을 검사하여 전투씬 종료 시키기
        if (PS_playerStats.Health <= 0) //플레이어 체력 먼저 검사
        {
            GameManager.Instance.GameOver();
            yield break;
        }

        if (CR_Enemy.health <= 0) //그다음 크리쳐 체력 검사
        {
            StartCoroutine(BM_BattleMain.EndFight(true));
            yield break;
        }

        //종료 조건이 안되면 애니메이션 끝날때까지 대기
        yield return new WaitForSeconds(targetMMF.TotalDuration / 2f);

        //Reverse Feedback Play
        MMF_player[2].PlayFeedbacks();
        DestroyImmediate(dmgObject);
        BM_BattleMain.EndTurn(isPlayer);
        yield return null;
    }
}