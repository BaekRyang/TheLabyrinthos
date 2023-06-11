using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TypeDefs;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] public Image       killedBy;
    [SerializeField] public CanvasGroup statisticCanvasGroup;
    [SerializeField] public GameObject  blackPanel;
    [SerializeField] public MMF_Player  gameoverMMF;
    [SerializeField] public Transform[] statisticContent;
    [SerializeField] public GameObject  statisticPrefab;

    public void BuildStatistic(Dictionary<Statistics,float> statistics)
    {
        foreach (var (key, value) in statistics)
        {
            string type = "";
            switch (key)
            {
                case Statistics.KilledEnemy:
                    type = "처치한 적";
                    break;
                case Statistics.DealtDamage:
                    type = "가한 피해량";
                    break;
                case Statistics.TakenDamage:
                    type = "받은 피해량";
                    break;
                case Statistics.Healed:
                    type = "회복량";
                    break;
                case Statistics.UsedItem:
                    type = "사용한 아이템";
                    break;
                case Statistics.MissedAttack:
                    type = "빗나간 공격";
                    break;
                case Statistics.AvoidedAttack:
                    type = "회피한 공격";
                    break;
                case Statistics.EnteredRoom:
                    type = "입장한 방";
                    break;
            }
            AddStatistic(type, value);
        }
    }
    
    public void AddStatistic(string name, float value)
    {
        if (statisticContent[0].childCount < 7)
        {
            var go = Instantiate(statisticPrefab, statisticContent[0]);
            go.transform.GetChild(0).GetComponent<TMP_Text>().text = name;
            //value가 소수부분이 있으면 1자리까지만 표시하고 없으면 표시하지 않음
            
            go.transform.GetChild(1).GetComponent<TMP_Text>().text = value.ToString("0.#");
            
            
        }
        else
        {
            var go = Instantiate(statisticPrefab, statisticContent[1]);
            go.transform.GetChild(0).GetComponent<TMP_Text>().text = name;
            go.transform.GetChild(1).GetComponent<TMP_Text>().text = value.ToString();
        }
    }

    public void Ending()
    {
        
    }
}
