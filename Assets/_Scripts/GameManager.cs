using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

public delegate void Dele();

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Seed")] 
    [SerializeField] private bool   b_useSeed;
    [SerializeField] private string s_seed = "-";
    [SerializeField] private int    i_roomSize;

    [Header("System Objects")]
    [SerializeField] public GameObject GO_curtain;
    [SerializeField] public GameObject GO_BattleCanvas;

    [Header("Set Automatically")]
    [SerializeField] Creatures creatures;

    [SerializeField] public Creature CR_levelDefault;

    [Header("Player Prefab")]
    [SerializeField]
    private GameObject go_playerPrefab;

    [NonSerialized] public GameObject go_player;

    [Header("Room Struct Prefabs")]
    [SerializeField] private GameObject GO_startRoomPrefab;
    [SerializeField] private GameObject[] GO_roomPrefabs;
    [SerializeField] private GameObject[] GO_corridorPrefabs;
    [SerializeField] private GameObject[] GO_craftingPrefabs;
    [SerializeField] private GameObject[] GO_keyRoomPrefabs;

    [Header("Test Keys")] 
    public bool b_hasKey;

    [SerializeField] 
    private GameObject[] GO_shopPrefabs;

    [Header("Level Controll")] 
    public int i_level = 1;

    [HideInInspector] public Dictionary<string, Random> dict_randomObjects = new Dictionary<string, Random>();

    [Header("InventoryUIs")]
    [SerializeField] private Slider SL_hpBar;
    [SerializeField] private Slider SL_expBar;

    [Header("Setting UI")]
    [SerializeField] public GameObject settings;

    [Header("EffectManagement")]
    [SerializeField] public EffectsManager effectsManager;
    
    [Header("Battle Controll")]
    public bool b_nowBattle;


    void Awake()
    {
        Instance ??= this;

        GO_startRoomPrefab = Resources.Load<GameObject>("RoomStructures/StartRoom/StartRoom");
        GO_roomPrefabs     = Resources.LoadAll<GameObject>("RoomStructures/Default");
        GO_corridorPrefabs = Resources.LoadAll<GameObject>("RoomStructures/Corridor");
        GO_craftingPrefabs = Resources.LoadAll<GameObject>("RoomStructures/SpecialRoom/Crafting");
        GO_keyRoomPrefabs = Resources.LoadAll<GameObject>("RoomStructures/SpecialRoom/KeyRoom");
        GO_shopPrefabs     = Resources.LoadAll<GameObject>("RoomStructures/SpecialRoom/Shop");

        creatures = new Creatures();
    }

    void Start()
    {
        StartCoroutine(LoadSettings());
        Cursor.lockState = CursorLockMode.Locked;
        GameObject data = GameObject.Find("DataPacker");

        if (!data.IsUnityNull())
        {
            if (!data.GetComponent<DataCarrier>().useSeed)
                //시드를 따로 지정하지 않았으면 새로 만들어준다.
                GetComponent<RoomCreation>().CreateSeed(out s_seed);
            else
                s_seed = data.GetComponent<DataCarrier>().seed;

            Destroy(data);
        }
        else
        {
            if (!b_useSeed)
                //시드를 따로 지정하지 않았으면 새로 만들어준다.
                GetComponent<RoomCreation>().CreateSeed(out s_seed);
        }

        dict_randomObjects.Add("Object", new Random(Convert.ToInt32(s_seed, 16) + 1));   //오브젝트용 랜덤 시드
        dict_randomObjects.Add("Creature", new Random(Convert.ToInt32(s_seed, 16) + 2)); //크리쳐용 랜덤 시드
        dict_randomObjects.Add("Room", new Random(Convert.ToInt32(s_seed, 16) + 3));     //방배치용 랜덤 시드
        dict_randomObjects.Add("Effect", new Random(Convert.ToInt32(s_seed, 16) + 4));   //아이템용 랜덤 시드
        dict_randomObjects.Add("Syringe", new Random(Convert.ToInt32(s_seed, 16) + 5));  //주사기용 랜덤 시드

        ResetLevel(i_level);
        go_player                 = Instantiate(go_playerPrefab);
        go_player.name            = "Player";
    }

    IEnumerator LoadSettings()
    {
        yield return new WaitForSeconds(1f);
        //Awake나 Start 대기용
        
        yield return StartCoroutine(GetComponent<CSVReader>().LoadSetting());
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Item Table Load Complete");

        yield return StartCoroutine(GetComponent<InventoryManager>().LoadSetting());
        Player.Instance.WP_weapon = InventoryManager.weaponInventory[0][0];
        yield return new WaitForSeconds(0.1f);
        Debug.Log("ItemManager Load Complete");

        yield return StartCoroutine(Crafting.Instance.LoadSetting());
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Crafting Recipe Load Complete");

        yield return StartCoroutine(BattleMain.instance.LoadSetting());
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Battle Main Load Complete");
        
        yield return StartCoroutine(BattleMain.instance.BA_battleActions.LoadSetting());
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Battle Action Load Complete");
        
        UpdateStatsSlider(StatsType.Hp);
        UpdateStatsSlider(StatsType.Exp);

        yield return new WaitForSeconds(2f);
        
        yield return StartCoroutine(CurtainModify(true, 2));
        yield return null;
    }

    void Update()
    {
        //UI 켜거나 끄기
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
            Inventory("Inventory");
    }

    public void ResetLevel(int level)
    {
        if (!go_player.IsUnityNull()) go_player.GetComponent<Rigidbody>().useGravity = false;
        i_roomSize = 5 + Mathf.RoundToInt(level * 3.3f);
        Debug.Log("구조 생성 시작 - Size : " + i_roomSize);
        GetComponent<RoomCreation>()
            .InitStruct(Convert.ToInt32(s_seed, 16) + level, i_roomSize); //시드는 16진수이지만, 알고리즘은 10진수 => 바꿔서 넘겨줌
        Debug.Log("구조 생성 완료 - 배치 시작");
        GetComponent<RoomCreation>().PlaceRoom();
        Debug.Log("방 배치 완료");
        GetComponent<RoomCreation>().roomMap[45].RoomObject.GetComponent<RoomController>().go_specialObject
            .GetComponent<TMP_Text>().text = "Level " + i_level;
        if (!go_player.IsUnityNull()) go_player.GetComponent<Rigidbody>().useGravity = true;

        Minimap.instance.CreateMinimap(GetComponent<RoomCreation>().roomMap);
        CR_levelDefault = creatures.C_default[level - 1]; //현재레벨 기본 크리쳐 스텟 설정

        GetComponent<RoomCreation>().roomMap[45].RoomObject.GetComponent<BGMPlayer>().StartMusic(null);
    }

    public IEnumerator CurtainModify(bool open, float delay) //화면 암전 풀거나 걸기
    {
        Image IMG_blackPanel = GO_curtain.GetComponent<Image>();
        float elapsedTime    = 0f;


        Color startColor = IMG_blackPanel.color;
        Color endColor   = IMG_blackPanel.color;
        if (open)
        {
            startColor.a = 1f;
            endColor.a   = 0f;
        }
        else
        {
            startColor.a = 0f;
            endColor.a   = 1f;
        }


        while (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / delay;
            IMG_blackPanel.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        IMG_blackPanel.color = endColor; //완전히 불투명한 상태로 설정
    }

    public IEnumerator OpenElevator(GameObject[] obj, float duration, float CHANGE_LEVEL_DELAY, PlayerController pc_controller) //obj는 엘레베이터 양쪽 문 오브젝트 저장되어있음
    {
        pc_controller.b_camControll = true;
        StartCoroutine(Instance.CurtainModify(false, CHANGE_LEVEL_DELAY)); //화면 암전
        float elapsedTime = 0f;

        Vector3 obj1StartPosition = obj[0].transform.localPosition;
        Vector3 obj1EndPosition = new Vector3(obj1StartPosition.x, obj1StartPosition.y,
            obj1StartPosition.z + obj[0].transform.localScale.z);

        Vector3 obj2StartPosition = obj[1].transform.localPosition;
        Vector3 obj2EndPosition = new Vector3(obj2StartPosition.x, obj2StartPosition.y,
            obj2StartPosition.z - obj[1].transform.localScale.z);

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
        yield return
            new WaitForSeconds(CHANGE_LEVEL_DELAY - duration); //CHANGE_LEVEL_DELAY가 문닫히는 시간보다 기니까 암전 완료될때까지 기다림
        Instance.ResetLevel(++i_level);
        yield return new WaitForSeconds(1f);                       //방 바뀌는 모습 보이지 않게 멈추고
        StartCoroutine(CurtainModify(true, CHANGE_LEVEL_DELAY));   //암전 풀어주고
        go_player.GetComponent<PlayerController>().ResetSetting(); //플레이어 조작부분에 현재 방 관련 코드 초기화시켜준다.
        pc_controller.b_camControll = false;
    }

    public GameObject GetRoomObject(RoomType roomType, int typeId = -1)
    {
        switch (roomType)
        {
            case RoomType.common:
                if (typeId != -1) return GO_roomPrefabs[typeId];
                return GO_roomPrefabs[dict_randomObjects["Room"].Next(GO_roomPrefabs.Length)]; //무작위 구조 반환

            case RoomType.EndRoom:
                if (typeId != -1) return GO_roomPrefabs[typeId];
                return GO_roomPrefabs[dict_randomObjects["Room"].Next(GO_roomPrefabs.Length)];

            case RoomType.StartRoom:
                return GO_startRoomPrefab; //시작방은 하나

            case RoomType.VerticalCorridor: //이 두개는 같은걸 return
            case RoomType.HorizontalCorridor:
                if (typeId != -1) return GO_corridorPrefabs[typeId];
                return GO_corridorPrefabs[dict_randomObjects["Room"].Next(GO_corridorPrefabs.Length)];

            case RoomType.CraftingRoom:
                if (typeId != -1) return GO_craftingPrefabs[typeId];
                return GO_craftingPrefabs[dict_randomObjects["Room"].Next(GO_craftingPrefabs.Length)];

            case RoomType.KeyRoom:
                if (typeId != -1) return GO_keyRoomPrefabs[typeId];
                return GO_keyRoomPrefabs[dict_randomObjects["Room"].Next(GO_keyRoomPrefabs.Length)];

            case RoomType.Shop:
                if (typeId != -1) return GO_shopPrefabs[typeId];
                return GO_shopPrefabs[dict_randomObjects["Room"].Next(GO_shopPrefabs.Length)];

            default:
                if (typeId != -1) return GO_roomPrefabs[typeId];
                return GO_roomPrefabs[dict_randomObjects["Room"].Next(GO_roomPrefabs.Length)];
        }
    }

    public void UpdateStatsSlider(StatsType statsType)
    {
        var stats = Player.Instance.GetPlayerStats();
        switch (statsType)
        {
            case StatsType.Hp:
                SL_hpBar.maxValue = stats.maxHealth;
                SL_hpBar.value    = stats.health;
                break;
            case StatsType.Exp:
                SL_expBar.value = stats.exp;
                break;
        }
    }

    public void Inventory(string targetUI)
    {
        bool b_setClose = InventoryManager.Instance.b_UIOpen;

        PlayerController PC_tmp = go_player.GetComponent<PlayerController>();
        if (b_setClose) //인벤끄기
        {
            PC_tmp.b_camControll = false;

            var infoBox  = InventoryManager.Instance.RT_infoBox;
            var infoBox2 = InventoryManager.Instance.RT_descBox;
            infoBox.localScale = Vector3.zero;
            infoBox.gameObject.SetActive(false);
            infoBox2.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            InventoryManager.Instance.CloseUI();
        }
        else
        {
            //아래값 0으로 안바꾸면 마지막 누른 버튼의 상태를 유지한다.
            PC_tmp.horizontal    = 0;
            PC_tmp.vertical      = 0;
            PC_tmp.b_camControll = true;
            Cursor.lockState     = CursorLockMode.Confined;
            InventoryManager.Instance.OpenInventory(targetUI);
        }
    }

    public void SystemButtonAction(string buttonType)
    {
        if (buttonType == "Setting")
        {
            if (settings.activeSelf)
            {
                buttonType = "Resume";
            }
            else
            {
                settings.SetActive(true);
                CanvasGroup canvasAlpha = settings.GetComponent<CanvasGroup>();
                StartCoroutine(Lerp.LerpValue<float>(value => canvasAlpha.alpha = value, 0, 1, 0.3f, Mathf.Lerp,
                    Lerp.EaseOut));
            }
        }

        if (buttonType == "Resume")
        {
            CanvasGroup canvasAlpha = settings.GetComponent<CanvasGroup>();
            StartCoroutine(Lerp.LerpValueAfter<float>(value => canvasAlpha.alpha = value, 1, 0, 0.3f, Mathf.Lerp,
                Lerp.EaseOut, () => settings.gameObject.SetActive(false)));
        }

        if (buttonType == "Exit")
            Application.Quit();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }
}