using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Seed")]
    public bool UseSeed = false;
    public string seed = "-";
    public int roomCount = 10;

    [Header("System Objects")]
    public GameObject GO_curtain;
    public Creatures creatures = new Creatures();

    [Header("Test Keys")]
    public bool hasKey = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (!UseSeed)
        {
            //시드를 따로 지정하지 않았으면 새로 만들어준다.
            GetComponent<RoomCreation>().CreateSeed(ref seed);
        }
        Debug.Log("구조 생성 시작");
        GetComponent<RoomCreation>().InitStruct(Convert.ToInt32(seed, 16), roomCount); //시드는 16진수이지만, 알고리즘은 10진수 => 바꿔서 넘겨줌
        Debug.Log("구조 생성 완료 - 배치 시작");
        GetComponent<RoomCreation>().PlaceRoom();
        Debug.Log("방 배치 완료");
    }

    void Update()
    {
        
    }

    public IEnumerator CurtainModify(bool open, float delay)
    {
        Image IMG_blackPanel = GO_curtain.GetComponent<Image>();
        float elapsedTime = 0f;

        
        Color startColor = IMG_blackPanel.color;
        Color endColor = IMG_blackPanel.color;
        if(open)
        {
            startColor.a = 1f;
            endColor.a = 0f;
        } else
        {
            startColor.a = 0f;
            endColor.a = 1f;
        }
        

        while (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / delay;
            IMG_blackPanel.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        IMG_blackPanel.color = endColor; // 완전히 불투명한 상태로 설정
    }
}
