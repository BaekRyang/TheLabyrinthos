using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Input = UnityEngine.Input;

public class PlayerController : MonoBehaviour
{
    [Header("Player Textures")]
    [SerializeField] GameObject spriteBox;
    [SerializeField] Material playerMat;
    [SerializeField] Texture[] playerTexture;

    GameObject GO_PrevInteracted = null;

    float moveSpeed = 2.5f;
    Rigidbody rigid;
    public Vector3 direction;
    int faceing = 0;

    //카메라 조종
    public bool b_camControll = false;
    public bool b_nowBattle = false;

    //카메라 회전 및 움직임
    float rotationSpeed = 2f;
    float distanceFromPlayer = 1f;

    float pitch;

    private Vector3 offset;
    public float yaw = 0;

    float minZoomDistance = 1f;
    float maxZoomDistance = 1f;

    private Vector3 previousPosition;
    private Quaternion previousRotation;

    //카메라 벽 체크
    public LayerMask wallLayer;
    private float defaultCameraDistance;

    public int roomIndex = 45;


    //플레이어 마우스 액션
    RaycastHit hit;
    bool isHit;
    float detectionDistance = 0.2f;
    LayerMask LM_InteractLayerMask; //검출하고자 하는 레이어를 지정.
    GameObject GO_LastHitGO;

    public float horizontal;
    public float vertical;

    //방 체크
    private int groundLayer;

    //이전 방의 이름과 게임 오브젝트
    [SerializeField] GameObject prevRoom;
    [SerializeField] GameObject prevRoomMinimap;

    //미니맵 방향표시용
    private RectTransform GO_minimapArrow;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        offset = new Vector3(0, 1.8f, -distanceFromPlayer);
        LM_InteractLayerMask = (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Door"));
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        wallLayer = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Door"));
    }

    private void Start()
    {
        playerTexture = Resources.LoadAll<Texture>("Sprites/Player");
        GO_minimapArrow = Minimap.instance.GO_arrow.GetComponent<RectTransform>();
        GO_minimapArrow.transform.rotation = Quaternion.Euler(0, 0, Camera.main.transform.rotation.eulerAngles.y + (90 - Camera.main.transform.rotation.eulerAngles.y) * 2);
        ResetSetting();
    }

    private void Update()
    {
        //플레이어가 있는 방의 인덱스 계산
        CalcPlayerRoomIndex();
        
        if (!b_camControll)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
            {
                //카메라 방향 기준으로 방향 계산
                direction = Camera.main.transform.forward * vertical + Camera.main.transform.right * horizontal;
                direction.y = 0; //수직 방향으로 이동하지 않도록 y축 값을 0으로 설정
                direction.Normalize();
                Debug.DrawRay(transform.position, direction, Color.yellow);
            }

            //마우스 이동에 따라 pitch값과 yaw값 변경
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;

            //pitch값 범위 제한
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            GO_minimapArrow.transform.rotation = 
                Quaternion.Euler(   0,
                                    0,
                                    Camera.main.transform.rotation.eulerAngles.y + (90 - Camera.main.transform.rotation.eulerAngles.y) * 2);

            spriteBox.transform.rotation = Quaternion.Euler(0, yaw, 0);

            //대상 체크
            InteractCalc();
            
        }
    }


    private void FixedUpdate()
    {
        if (horizontal == 0 && vertical == 0)
            rigid.velocity = Vector3.zero;
        else
            Move();

        // 카메라 회전
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Camera.main.transform.position = transform.position + Vector3.up * 1.65f;
        Camera.main.transform.rotation = rotation;

    }

    protected void Move()
    {
        float currentMoveSpeed = moveSpeed;
        Vector3 velocity = new Vector3(direction.x, 0, direction.z);

        rigid.velocity = velocity * currentMoveSpeed;
    }

    private void InteractCalc()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rcastHit;

        bool isNotFirstInteract = GO_PrevInteracted != null;
        // 만약 레이캐스트가 히트하지 않았거나, 오브젝트가 상호작용 가능한 것이 아니라면
        if (!Physics.Raycast(ray, out rcastHit, 50, LM_InteractLayerMask) || rcastHit.transform?.GetComponent<Interactable>() == null)
        {
            if (isNotFirstInteract)
            {
                GO_PrevInteracted.GetComponent<Outline>().enabled = false;
                GO_PrevInteracted = null;
            }
            return;
        }

        //이 시점에서는 오브젝트가 상호작용 가능하다는 것이 확실함
        //거리가 너무 멀다면
        if ((rcastHit.transform.position - transform.position).sqrMagnitude >= 4)
        {
            if (isNotFirstInteract)
            {
                GO_PrevInteracted.GetComponent<Outline>().enabled = false;
                GO_PrevInteracted = null;
            }
            return;
        }
            

        if (isNotFirstInteract && GO_PrevInteracted != rcastHit.transform.gameObject)
            GO_PrevInteracted.GetComponent<Outline>().enabled = false;

        GO_PrevInteracted = rcastHit.transform.gameObject;
        GO_PrevInteracted.GetComponent<Outline>().enabled = true;

        //만약 마우스 버튼이 눌려지지 않았다면
        if (!Input.GetMouseButtonDown(0))
            return;

        //이동 데이터를 초기화 시켜준다.
        horizontal = 0;
        vertical = 0;
        GO_PrevInteracted.GetComponent<Interactable>().Run(this);

    }

    public void ResetSetting()
    {
        prevRoom = GameManager.Instance.GetComponent<RoomCreation>().roomMap[45].RoomObject; //플레이어가 위치한 방의 루트 오브젝트를 저장 (플레이어는 45번에 생성)
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true); //플레이어가 위치한 방의 상태를 변경

        prevRoomMinimap = Minimap.instance.GetRoom(45);
        prevRoomMinimap.GetComponent<Image>().color = Color.white;
        defaultCameraDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    public void CalcPlayerRoomIndex()
    {
        int size = 10;

        Vector3 playerPos = transform.position;

        int roomIndexX = Mathf.FloorToInt(((playerPos.x + 5) / size) + 5);
        int roomIndexY = Mathf.FloorToInt(((playerPos.z + 5) / size) + 4) * 10;
        int tmpIndex = roomIndexX + roomIndexY;
        if (roomIndex != tmpIndex)
        {
            UpdateRoomEnter(tmpIndex);
            roomIndex = tmpIndex;
        }
    }

    public void UpdateRoomEnter(int roomIndex)
    {
        if (!GameManager.Instance.GetComponent<RoomCreation>().roomMap.ContainsKey(roomIndex))
        {
            Debug.Log("존재하지 않는 방");
            return;
        }
            
        prevRoom.GetComponent<RoomController>().ChangeRoomState(false); //이전 방의 상태를 변경
        prevRoom = GameManager.Instance.GetComponent<RoomCreation>().roomMap[roomIndex].RoomObject; //플레이어가 위치한 방의 오브젝트를 저장
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true); //플레이어가 위치한 방의 상태를 변경

        //위랑 똑같지만 미니맵 전용
        prevRoomMinimap.GetComponent<Image>().color = Color.gray;
        prevRoomMinimap = Minimap.instance.GetRoom(roomIndex);
        prevRoomMinimap.GetComponent<Image>().color = Color.white;
        Minimap.instance.SetAnchor(roomIndex);

        if (prevRoom.GetComponent<RoomController>().b_hasCreature) //들어간 방에 크리쳐가 있으면
        {
            //플레이어 조작은 문 열때 막히니 따로 막을 필요 없고
            b_nowBattle = true; //문 열리고난뒤 조작 풀어주는것을 막는다.
            Cursor.lockState = CursorLockMode.Confined;
            GameManager.Instance.GO_BattleCanvas.SetActive(true); //전투씬 켜고
            BattleMain.instance.StartBattleScene(ref prevRoom.GetComponent<RoomController>().CR_creature); //현재 방에 있는 크리쳐 정보를 넘겨준다.
        }
    }

    public void ExitBattle()
    {
        b_nowBattle = false;
        Cursor.lockState = CursorLockMode.Locked;
        prevRoom.GetComponent<RoomController>().DestroyCreature();

    }
}
