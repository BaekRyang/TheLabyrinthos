using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using TypeDefs;



public class BattleActions : MonoBehaviour
{
    const int CONST_DEF         = 20; //방어 상수
    const int BASE_ACCURACY     = 75; //기본 정확도

    const float WEAKPOINT_DMG   = 1.55f;
    const float WEAKPOINT_ACC   = 0.75f;
    const float THORAX_DMG      = 1.00f;
    const float THORAX_ACC      = 1.00f;
    const float OUTER_DMG       = 0.80f;
    const float OUTER_ACC       = 1.30f;

    Dictionary<Parts, AttackPair> dict_attackTable = new Dictionary<Parts, AttackPair>();

    //정보를 받아와서 저장할 위치
    private PlayerStats PS_player;
    public Creature CR_Enemy;

    BattleMain BM_BattleMain;
    Random rand = new Random();

    void Start()
    {
        //PlayerStats 클래스 객체 가져오기(참조로 가져온다)
        PS_player = GameManager.Instance.GetComponent<Player>().GetPlayerStats();

        BM_BattleMain = GetComponent<BattleMain>();

        dict_attackTable.Add(Parts.Weakpoint, new AttackPair(WEAKPOINT_DMG, WEAKPOINT_ACC));
        dict_attackTable.Add(Parts.Thorax, new AttackPair(THORAX_DMG, THORAX_ACC));
        dict_attackTable.Add(Parts.Outer, new AttackPair(OUTER_DMG, OUTER_ACC));
    }
    void Update()
    {
        
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

            default:
                break;
        }
    }

    public void Attack(bool b_IsPlayer, Parts part = Parts.Thorax)
    {
        AudioClip clip;
        int randInt = rand.Next(101);

        if (b_IsPlayer)
        {
            int accInt = (int)(BASE_ACCURACY * dict_attackTable[part].accuracy);

            if (randInt <   //랜덤 (0~100)
                accInt)     //기본 정확도 x 부위 정확도 계수
            {
                float damage = (PS_player.damage *                                                  //플레이어 공격력
                                (dict_attackTable[part].damage) *                                   //부위 데미지 계수
                                (1 - CR_Enemy.defense / (float)(CR_Enemy.defense + CONST_DEF)));    //방어력 계산

                damage = Mathf.Round(damage * 10f) / 10f;
                CR_Enemy.health -= damage;
                CR_Enemy.health = Mathf.Round(CR_Enemy.health * 10f) / 10f;
                BM_BattleMain.ChangeSliderValue(false, StatsType.Hp, CR_Enemy.health);

                StartCoroutine(LerpColor(BM_BattleMain.enemyElements,   //해당 Target의 Image를
                                            SliderColor.Hp_hilighted,   //해당 색으로 바꿨다가
                                            1f));                       //해당 초 동안 돌아온다.

                switch (part)
                {
                    case Parts.Weakpoint:
                        clip = BM_BattleMain.AC_playerAttackWeakPoint[rand.Next(BM_BattleMain.AC_playerAttackWeakPoint.Length)];
                        break;
                    case Parts.Thorax:
                        clip = BM_BattleMain.AC_playerAttackThorax[rand.Next(BM_BattleMain.AC_playerAttackThorax.Length)];
                        break;
                    case Parts.Outer:
                        clip = BM_BattleMain.AC_playerAttackOuter[rand.Next(BM_BattleMain.AC_playerAttackOuter.Length)];
                        break;
                    default:
                        clip = BM_BattleMain.AC_playerAttackThorax[rand.Next(BM_BattleMain.AC_playerAttackThorax.Length)];
                        break;
                }
                
                //맞으면 타격음으로
            } 
            else
            {
                clip = BM_BattleMain.AC_playerMissed[rand.Next(BM_BattleMain.AC_playerMissed.Length)];
                //빗나가면 다른 소리로
            }

            Debug.Log("Player : " + randInt + " > " + accInt);
            BM_BattleMain.EndTurn(true);
            BM_BattleMain.GO_attackList.SetActive(false);
            BM_BattleMain.GO_actionList.SetActive(false);

            StartCoroutine(LerpFill(    BM_BattleMain.enemyElements,
                                        0.1f,
                                        BM_BattleMain.SPR_playerAttack,
                                        clip));
        } else
        {
            if (randInt <       //랜덤 (0~100)
               BASE_ACCURACY)   //**크리쳐별 정확도를 가져와서 써야함
            {
                float damage = (CR_Enemy.damage * (1 - PS_player.defense / (float)(PS_player.defense + CONST_DEF)));
                damage = Mathf.Round(damage * 10f) / 10f;
                PS_player.health -= damage;
                PS_player.health = Mathf.Round(PS_player.health * 10f) / 10f;
                BM_BattleMain.ChangeSliderValue(true, StatsType.Hp, PS_player.health);

                StartCoroutine(LerpColor(BM_BattleMain.playerElements,
                                            SliderColor.Hp_hilighted,
                                            1f));
                clip = BM_BattleMain.AC_enemyAttack[rand.Next(BM_BattleMain.AC_enemyAttack.Length)];
            } 
            else
            {
                clip = BM_BattleMain.AC_playerMissed[rand.Next(BM_BattleMain.AC_playerMissed.Length)];
            }

            BM_BattleMain.EndTurn(false);
            Debug.Log("Creature : " + randInt + " > " + BASE_ACCURACY);
            StartCoroutine(LerpFill(    BM_BattleMain.playerElements,
                                        0.1f,
                                        BM_BattleMain.SPR_enemyAttack,
                                        clip));
        }
    }

    IEnumerator LerpColor(UIModElements targetElements, SliderColor to, float duration, bool isEndColor = false)
    {
        Color startColor;
        Color endColor;
        if (isEndColor) //Param으로 받는 Color로 끝나야 하는가?
        {
            startColor   = targetElements.hpSlider.color;
            endColor     = BM_BattleMain.colors[(int)to];
        } else {
            startColor   = BM_BattleMain.colors[(int)to];
            endColor     = targetElements.hpSlider.color;
        }

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            targetElements.hpSlider.color = Color.Lerp(startColor, endColor, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetElements.hpSlider.color = endColor;
    }

    IEnumerator LerpColor(Image targetImage, SliderColor to, float duration, bool isEndColor) //이미지 인수로 하는 오버로딩
    {
        Color startColor;
        Color endColor;
        if (isEndColor) //Param으로 받는 Color로 끝나야 하는가?
        {
            startColor = targetImage.color;
            endColor = BM_BattleMain.colors[(int)to];
        }
        else
        {
            startColor = BM_BattleMain.colors[(int)to];
            endColor = targetImage.color;
        }

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            targetImage.color = Color.Lerp(startColor, endColor, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetImage.color = endColor;
    }

    IEnumerator LerpFill(UIModElements targetElements, float duration, Sprite hitSprite, AudioClip clip) //0에서 100까지 채우기
    {
        Random tmpRand = new Random();
        GameObject tmpGO = GameObject.Instantiate(BM_BattleMain.GO_hitImage, targetElements.hitImage);
        Image hitImage = tmpGO.GetComponent<Image>();
        hitImage.sprite = hitSprite;
        hitImage.transform.localPosition = new Vector3(tmpRand.Next(-50, 50), tmpRand.Next(-150, 150), 0); //위치는 랜덤
        tmpGO.transform.Rotate(0, 0, tmpRand.Next(180)); //방향도 랜덤

        tmpGO.GetComponent<AudioSource>().clip = clip;
        tmpGO.GetComponent<AudioSource>().Play();
        //플레이어 공격 소리중 아무거나 가져와서 재생


        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
             hitImage.fillAmount = Mathf.Lerp(0, 1, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hitImage.fillAmount = 1;
        StartCoroutine(LerpColor(hitImage, SliderColor.transparent, 0.5f, true));
        yield return new WaitForSeconds(0.5f);
        Destroy(tmpGO);
    }

    public void SetDmgNAcc() //공격 상세창 값 초기화용
    {
        var typeDict = BM_BattleMain.dict_dmgAccList;
        typeDict[Parts.Weakpoint]   .percentage.text = (BASE_ACCURACY * dict_attackTable[Parts.Weakpoint] .accuracy).ToString() + "%";
        typeDict[Parts.Thorax]      .percentage.text = (BASE_ACCURACY * dict_attackTable[Parts.Thorax]    .accuracy).ToString() + "%";
        typeDict[Parts.Outer]       .percentage.text = (BASE_ACCURACY * dict_attackTable[Parts.Outer]     .accuracy).ToString() + "%";

        typeDict[Parts.Weakpoint]   .damage.text = (PS_player.damage * dict_attackTable[Parts.Weakpoint]  .damage * (1 - CR_Enemy.defense / (float)(CR_Enemy.defense + CONST_DEF))).ToString("0.##") + " DMG";
        typeDict[Parts.Thorax]      .damage.text = (PS_player.damage * dict_attackTable[Parts.Thorax]     .damage * (1 - CR_Enemy.defense / (float)(CR_Enemy.defense + CONST_DEF))).ToString("0.##") + " DMG";
        typeDict[Parts.Outer]       .damage.text = (PS_player.damage * dict_attackTable[Parts.Outer]      .damage * (1 - CR_Enemy.defense / (float)(CR_Enemy.defense + CONST_DEF))).ToString("0.##") + " DMG";
    }
}
