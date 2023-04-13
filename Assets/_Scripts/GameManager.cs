using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Seed")]
    public bool UseSeed = false;
    public string seed = "-";
    public int i_roomSize = 0;

    [Header("System Objects")]
    public GameObject GO_curtain;
    public Creatures creatures = new Creatures();

    [Header("Test Keys")]
    public bool hasKey = false;

    [Header("Player Prefab")]
    [SerializeField] GameObject go_playerPrefab;
    [NonSerialized] public GameObject go_player;

    [Header("Room Struct Prefabs")]
    public GameObject[] GO_RoomPrefabs;

    [Header("Level Controll")]
    public int i_level = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GO_RoomPrefabs = Resources.LoadAll<GameObject>("RoomStructures");
    }

    void Start()
    {
        if (!UseSeed)
        {
            //시드를 따로 지정하지 않았으면 새로 만들어준다.
            GetComponent<RoomCreation>().CreateSeed(ref seed);
        }
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
        Debug.Log("구조 생성 시작 - Size : " + i_roomSize);
        GetComponent<RoomCreation>().InitStruct(Convert.ToInt32(seed, 16) + level, i_roomSize); //시드는 16진수이지만, 알고리즘은 10진수 => 바꿔서 넘겨줌
        Debug.Log("구조 생성 완료 - 배치 시작");
        GetComponent<RoomCreation>().PlaceRoom();
        Debug.Log("방 배치 완료");
        GetComponent<RoomCreation>().roomMap[45].RoomObject.GetComponent<RoomController>().go_specialObject.GetComponent<TMP_Text>().text = "Level " + i_level;
        if (!go_player.IsUnityNull()) go_player.GetComponent<Rigidbody>().useGravity = true;

    }

    public IEnumerator CurtainModify(bool open, float delay) //화면 암전 풀거나 걸기
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

    public IEnumerator OpenElevator(GameObject[] obj, float duration, float CHANGE_LEVEL_DELAY, PlayerController pc_controller) //obj는 엘레베이터 양쪽 문 오브젝트 저장되어있음
    {
        pc_controller.b_camControll = true;
        StartCoroutine(GameManager.Instance.CurtainModify(false, CHANGE_LEVEL_DELAY)); //화면 암전
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
        obj[1].transform.localPosition = obj2EndPosition; //여기까지 문 관련 코드 - 엘레베이터 문이 닫힘
        yield return new WaitForSeconds(CHANGE_LEVEL_DELAY - duration); //CHANGE_LEVEL_DELAY가 문닫히는 시간보다 기니까 암전 완료될때까지 기다림
        GameManager.Instance.ResetLevel(++i_level);
        yield return new WaitForSeconds(1f); //방 바뀌는 모습 보이지 않게 멈추고
        StartCoroutine(CurtainModify(true, CHANGE_LEVEL_DELAY)); //암전 풀어주고
        go_player.GetComponent<PlayerController>().ResetSetting(); //플레이어 조작부분에 현재 방 관련 코드 초기화시켜준다.
        pc_controller.b_camControll = false;
    }

    public GameObject GetRoomObject(int typeId = -1)
    {
        if (typeId != -1) return GO_RoomPrefabs[typeId];
        else
        {
            return GO_RoomPrefabs[1]; //무작위 구조 반환
        }
    }
}
