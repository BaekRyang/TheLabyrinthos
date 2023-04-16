using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Random = System.Random;
using TypeDefs;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Seed")]
    public bool b_useSeed = false;
    public string s_seed = "-";
    public int i_roomSize = 0;

    [Header("System Objects")]
    public GameObject GO_curtain;
    public Creatures creatures;

    [Header("Test Keys")]
    public bool b_hasKey = false;

    [Header("Player Prefab")]
    [SerializeField] GameObject go_playerPrefab;
    [NonSerialized] public GameObject go_player;

    [Header("Room Struct Prefabs")]
    [SerializeField] GameObject GO_startRoomPrefab;
    [SerializeField] GameObject[] GO_roomPrefabs;
    [SerializeField] GameObject[] GO_corridorPrefabs;
    [SerializeField] GameObject[] GO_craftingPrefabs;
    [SerializeField] GameObject[] GO_bossRoomPrefabs;
    [SerializeField] GameObject[] GO_shopPrefabs;

    [Header("Level Controll")]
    public int i_level = 1;

    [Header("DefaultCreatureSprite")]
    [SerializeField] public CreatureSpritePack spritePack;

    [HideInInspector]
    public Dictionary<string, Random> dict_randomObjects = new Dictionary<string, Random>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GO_startRoomPrefab =    Resources.Load   <GameObject>("RoomStructures/StartRoom/StartRoom");
        GO_roomPrefabs =        Resources.LoadAll<GameObject>("RoomStructures/Default");
        GO_corridorPrefabs =    Resources.LoadAll<GameObject>("RoomStructures/Corridor");
        GO_craftingPrefabs =    Resources.LoadAll<GameObject>("RoomStructures/SpecialRoom/Crafting");
        GO_bossRoomPrefabs =    Resources.LoadAll<GameObject>("RoomStructures/SpecialRoom/BossRoom");
        GO_shopPrefabs =        Resources.LoadAll<GameObject>("RoomStructures/SpecialRoom/Shop");

        creatures = new Creatures();
    }

    void Start()
    {
        if (!b_useSeed)
        {
            //�õ带 ���� �������� �ʾ����� ���� ������ش�.
            GetComponent<RoomCreation>().CreateSeed(ref s_seed);
        }

        dict_randomObjects.Add("Object",     new Random(Convert.ToInt32(s_seed, 16) + 1)); //������Ʈ�� ���� �õ�
        dict_randomObjects.Add("Creature",   new Random(Convert.ToInt32(s_seed, 16) + 2)); //ũ���Ŀ� ���� �õ�
        dict_randomObjects.Add("Room",       new Random(Convert.ToInt32(s_seed, 16) + 3)); //���ġ�� ���� �õ�
        
        ResetLevel(i_level);
        go_player = Instantiate(go_playerPrefab);

    }

    void Update()
    {
        
    }

    public void ResetLevel(int level)
    {
        if (!go_player.IsUnityNull()) go_player.GetComponent<Rigidbody>().useGravity = false;
        i_roomSize = 5 + Mathf.RoundToInt(level * 3.3f);
        Debug.Log("���� ���� ���� - Size : " + i_roomSize);
        GetComponent<RoomCreation>().InitStruct(Convert.ToInt32(s_seed, 16) + level, i_roomSize); //�õ�� 16����������, �˰����� 10���� => �ٲ㼭 �Ѱ���
        Debug.Log("���� ���� �Ϸ� - ��ġ ����");
        GetComponent<RoomCreation>().PlaceRoom();
        Debug.Log("�� ��ġ �Ϸ�");
        GetComponent<RoomCreation>().roomMap[45].RoomObject.GetComponent<RoomController>().go_specialObject.GetComponent<TMP_Text>().text = "Level " + i_level;
        if (!go_player.IsUnityNull()) go_player.GetComponent<Rigidbody>().useGravity = true;

    }

    public IEnumerator CurtainModify(bool open, float delay) //ȭ�� ���� Ǯ�ų� �ɱ�
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

    public IEnumerator OpenElevator(GameObject[] obj, float duration, float CHANGE_LEVEL_DELAY, PlayerController pc_controller) //obj�� ���������� ���� �� ������Ʈ ����Ǿ�����
    {
        pc_controller.b_camControll = true;
        StartCoroutine(GameManager.Instance.CurtainModify(false, CHANGE_LEVEL_DELAY)); //ȭ�� ����
        float elapsedTime = 0f;

        Vector3 obj1StartPosition = obj[0].transform.localPosition;
        Vector3 obj1EndPosition = new Vector3(obj1StartPosition.x, obj1StartPosition.y, obj1StartPosition.z + obj[0].transform.localScale.z);

        Vector3 obj2StartPosition = obj[1].transform.localPosition;
        Vector3 obj2EndPosition = new Vector3(obj2StartPosition.x, obj2StartPosition.y, obj2StartPosition.z - obj[1].transform.localScale.z);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            obj[0].transform.localPosition = Vector3.Lerp(obj1StartPosition, obj1EndPosition, t);
            obj[1].transform.localPosition = Vector3.Lerp(obj2StartPosition, obj2EndPosition, t);

            yield return null;
        }

        obj[0].transform.localPosition = obj1EndPosition;
        obj[1].transform.localPosition = obj2EndPosition; //������� �� ���� �ڵ� - ���������� ���� ����
        yield return new WaitForSeconds(CHANGE_LEVEL_DELAY - duration); //CHANGE_LEVEL_DELAY�� �������� �ð����� ��ϱ� ���� �Ϸ�ɶ����� ��ٸ�
        GameManager.Instance.ResetLevel(++i_level);
        yield return new WaitForSeconds(1f); //�� �ٲ�� ��� ������ �ʰ� ���߰�
        StartCoroutine(CurtainModify(true, CHANGE_LEVEL_DELAY)); //���� Ǯ���ְ�
        go_player.GetComponent<PlayerController>().ResetSetting(); //�÷��̾� ���ۺκп� ���� �� ���� �ڵ� �ʱ�ȭ�����ش�.
        pc_controller.b_camControll = false;
    }

    public GameObject GetRoomObject(SpecialRoomType roomType, int typeId = -1)
    {
        switch (roomType)
        {
            case SpecialRoomType.Normal:
                if (typeId != -1)   return GO_roomPrefabs[typeId];
                else                return GO_roomPrefabs[dict_randomObjects["Room"].Next(GO_roomPrefabs.Length)]; //������ ���� ��ȯ

            case SpecialRoomType.StartRoom:
                return              GO_startRoomPrefab; //���۹��� �ϳ�

            case SpecialRoomType.VerticalCorridor: //�� �ΰ��� ������ return
            case SpecialRoomType.HorizontalCorridor:
                if (typeId != -1)   return GO_corridorPrefabs[typeId];
                else                return GO_corridorPrefabs[dict_randomObjects["Room"].Next(GO_roomPrefabs.Length)];

            case SpecialRoomType.Crafting:
                if (typeId != -1)   return GO_craftingPrefabs[typeId];
                else                return GO_craftingPrefabs[dict_randomObjects["Room"].Next(GO_roomPrefabs.Length)];

            case SpecialRoomType.BossRoom:
                if (typeId != -1)   return GO_bossRoomPrefabs[typeId];
                else                return GO_bossRoomPrefabs[dict_randomObjects["Room"].Next(GO_roomPrefabs.Length)];

            case SpecialRoomType.Shop:
                if (typeId != -1)   return GO_shopPrefabs[typeId];
                else                return GO_shopPrefabs[dict_randomObjects["Room"].Next(GO_roomPrefabs.Length)];

            default:
                                    return null;
        }
    }
    
}
