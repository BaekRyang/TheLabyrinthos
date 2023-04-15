using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class BattleActions : MonoBehaviour
{
    const int CONST_DEF = 20; //방어 상수

    // 정보를 받아와서 저장할 위치
    private PlayerStats PS_player;
    public Creature CR_Enemy;

    BattleMain BM_BattleMain;
    Random rand = new Random();

    void Start()
    {
        // PlayerStats 클래스 객체 가져오기(참조로 가져온다)
        PS_player = GameManager.Instance.GetComponent<Player>().GetPlayerStats();

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
            BM_BattleMain.IMG_playerTP.color = BM_BattleMain.colors[(int)SliderColor.Tp_default];
            float damage = (PS_player.damage * (1 - CR_Enemy.defense / (float)(CR_Enemy.defense + CONST_DEF)));
            damage = Mathf.Round(damage * 10f) / 10f;
            CR_Enemy.health -= damage;
            CR_Enemy.health = Mathf.Round(CR_Enemy.health * 10f) / 10f;
            BM_BattleMain.ChangeSliderValue(false, StatsType.Hp, CR_Enemy.health);
            BM_BattleMain.ChangeSliderValue(true, StatsType.Tp, 0);
            BM_BattleMain.EndTurn(true);

            StartCoroutine(LerpColor(   BM_BattleMain.enemyElements,    //해당 Target의 Image를
                                        SliderColor.Hp_hilighted,       //해당 색으로 바꿨다가
                                        1f));                           //해당 초 동안 돌아온다.

            StartCoroutine(LerpFill(    BM_BattleMain.enemyElements,
                                        0.1f,
                                        BM_BattleMain.SPR_playerAttack,
                                        BM_BattleMain.AS_playerAttack[rand.Next(BM_BattleMain.AS_playerAttack.Length)]));

            Debug.Log(damage + "DMG - Player ATTACK");
        } else
        {
            BM_BattleMain.IMG_enemyTP.color = BM_BattleMain.colors[(int)SliderColor.Tp_default];
            float damage = (CR_Enemy.damage * (1 - PS_player.defense / (float)(PS_player.defense + CONST_DEF)));
            damage = Mathf.Round(damage * 10f) / 10f;
            PS_player.health -= damage;
            PS_player.health = Mathf.Round(PS_player.health * 10f) / 10f;
            BM_BattleMain.ChangeSliderValue(true, StatsType.Hp, PS_player.health);
            BM_BattleMain.ChangeSliderValue(false, StatsType.Tp, 0);
            BM_BattleMain.EndTurn(false);

            StartCoroutine(LerpColor(BM_BattleMain.playerElements,
                                        SliderColor.Hp_hilighted,   
                                        1f));

            StartCoroutine(LerpFill(    BM_BattleMain.playerElements,
                                        0.1f,
                                        BM_BattleMain.SPR_enemyAttack,
                                        BM_BattleMain.AS_enemyAttack[rand.Next(BM_BattleMain.AS_enemyAttack.Length)]));
            Debug.Log(damage + "DMG - Enemy ATTACK");
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
}
