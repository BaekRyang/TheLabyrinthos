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
            //�õ带 ���� �������� �ʾ����� ���� ������ش�.
            GetComponent<RoomCreation>().CreateSeed(ref seed);
        }
        Debug.Log("���� ���� ����");
        GetComponent<RoomCreation>().InitStruct(Convert.ToInt32(seed, 16), roomCount); //�õ�� 16����������, �˰����� 10���� => �ٲ㼭 �Ѱ���
        Debug.Log("���� ���� �Ϸ� - ��ġ ����");
        GetComponent<RoomCreation>().PlaceRoom();
        Debug.Log("�� ��ġ �Ϸ�");
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

        IMG_blackPanel.color = endColor; // ������ �������� ���·� ����
    }
}
