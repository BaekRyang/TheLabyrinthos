using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Textures")]
    [SerializeField]
    GameObject spriteBox;

    GameObject GO_PrevInteracted;

    float          moveSpeed = 2.5f;
    Rigidbody      rigid;
    public Vector3 direction;
    int            faceing = 0;

    //카메라 조종
    public bool b_camControll;

    //카메라 회전 및 움직임
    float rotationSpeed = 2f;

    float pitch;

    public float yaw;

    private Vector3    previousPosition;
    private Quaternion previousRotation;

    public int roomIndex = 45;


    //플레이어 마우스 액션
    RaycastHit hit;
    bool       isHit;
    int        detectionDistance = 2;
    LayerMask  LM_InteractLayerMask; //검출하고자 하는 레이어를 지정.
    GameObject GO_LastHitGO;

    public float horizontal;
    public float vertical;

    //이전 방의 이름과 게임 오브젝트
    [SerializeField] public GameObject prevRoom;
    [SerializeField] GameObject prevRoomMinimap;

    //미니맵 방향표시용
    private RectTransform GO_minimapArrow;


    private void Awake()
    {
        rigid                = GetComponent<Rigidbody>();
        LM_InteractLayerMask = (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Door"));
    }

    private void Start()
    {
        GO_minimapArrow = Minimap.instance.GO_arrow.GetComponent<RectTransform>();
        GO_minimapArrow.transform.rotation = Quaternion.Euler(0,
            0,
            Camera.main.transform.rotation.eulerAngles.y + (90 - Camera.main.transform.rotation.eulerAngles.y) * 2);

    }

    private void Update()
    {
        //플레이어가 있는 방의 인덱스 계산
        CalcPlayerRoomIndex();

        if (!b_camControll)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical   = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
            {
                //카메라 방향 기준으로 방향 계산
                direction   = Camera.main.transform.forward * vertical + Camera.main.transform.right * horizontal;
                direction.y = 0; //수직 방향으로 이동하지 않도록 y축 값을 0으로 설정
                direction.Normalize();

                Debug.DrawRay(transform.position, direction, Color.yellow);
            }

            //마우스 이동에 따라 pitch값과 yaw값 변경
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            yaw   += Input.GetAxis("Mouse X") * rotationSpeed;

            //pitch값 범위 제한
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            GO_minimapArrow.transform.rotation =
                Quaternion.Euler(0,
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

    private void Move()
    {
        float   currentMoveSpeed = moveSpeed;
        Vector3 velocity         = new Vector3(direction.x, 0, direction.z);

        rigid.velocity = velocity * currentMoveSpeed;
    }

    private void InteractCalc()
    {
        Ray        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rcastHit;

        bool isNotFirstInteract = GO_PrevInteracted != null;
        // 만약 레이캐스트가 히트하지 않았거나, 오브젝트가 상호작용 가능한 것이 아니라면
        if (!Physics.Raycast(ray, out rcastHit, 50, LM_InteractLayerMask) ||
            rcastHit.transform.GetComponent<Interactable>().IsUnityNull())
        {
            if (isNotFirstInteract)
            {
                GO_PrevInteracted.GetComponent<Interactable>().ChangeState(false);
                GO_PrevInteracted = null;
            }

            return;
        }

        //이 시점에서는 오브젝트가 상호작용 가능하다는 것이 확실함
        //거리가 너무 멀다면
        if ((rcastHit.transform.position - transform.position).sqrMagnitude >= Mathf.Pow(detectionDistance, 2))
        {
            if (isNotFirstInteract)
            {
                GO_PrevInteracted.GetComponent<Interactable>().ChangeState(false);
                GO_PrevInteracted = null;
            }

            return;
        }


        if (isNotFirstInteract && GO_PrevInteracted != rcastHit.transform.gameObject)
            GO_PrevInteracted.GetComponent<Interactable>().ChangeState(false);

        GO_PrevInteracted = rcastHit.transform.gameObject;
        GO_PrevInteracted.GetComponent<Interactable>().ChangeState(true);

        //만약 마우스 버튼이 눌려지지 않았다면
        if (!Input.GetMouseButtonDown(0))
            return;

        //이동 데이터를 초기화 시켜준다.
        horizontal = 0;
        vertical   = 0;
        GO_PrevInteracted.GetComponent<Interactable>().Run(this);
    }

    public void ResetSetting()
    {
        prevRoom = GameManager.Instance.GetComponent<RoomCreation>().roomMap[45]
            .RoomObject;                                               //플레이어가 위치한 방의 루트 오브젝트를 저장 (플레이어는 45번에 생성)
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true); //플레이어가 위치한 방의 상태를 변경

        prevRoomMinimap                             = Minimap.instance.GetRoom(45);
        prevRoomMinimap.GetComponent<Image>().color = Color.white;
    }

    private void CalcPlayerRoomIndex()
    {
        int size = 10;

        Vector3 playerPos = transform.position;

        int roomIndexX = Mathf.FloorToInt(((playerPos.x + 5) / size) + 5);
        int roomIndexY = Mathf.FloorToInt(((playerPos.z + 5) / size) + 4) * 10;
        int tmpIndex   = roomIndexX + roomIndexY;
        if (roomIndex != tmpIndex)
        {
            UpdateRoomEnter(tmpIndex);
            roomIndex = tmpIndex;
        }
    }

    private void UpdateRoomEnter(int enteredRoomIndex)
    {
        if (!GameManager.Instance.GetComponent<RoomCreation>().roomMap.ContainsKey(enteredRoomIndex))
        {
            Debug.Log("존재하지 않는 방");
            return;
        }

        //이전 방의 상태를 변경
        prevRoom.GetComponent<RoomController>().ChangeRoomState(false);
        prevRoom.TryGetComponent(out BGMPlayer prevBGM);

        //플레이어가 위치한 방의 오브젝트를 저장
        prevRoom = GameManager.Instance.GetComponent<RoomCreation>().roomMap[enteredRoomIndex].RoomObject;
        //플레이어가 위치한 방의 상태를 변경
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true);

        //위랑 똑같지만 미니맵 전용
        prevRoomMinimap.GetComponent<GoodTrip>().entered = true;
        prevRoomMinimap.GetComponent<Image>().color      = Color.gray;
        prevRoomMinimap                                  = Minimap.instance.GetRoom(enteredRoomIndex);
        prevRoomMinimap.GetComponent<Image>().color      = Color.white;
        Minimap.instance.SetAnchor(enteredRoomIndex);

        if (prevRoom.GetComponent<RoomController>().hasCreature) //들어간 방에 크리쳐가 있으면
        {
            vertical = horizontal = 0; //이동 데이터를 초기화 시켜준다.

            b_camControll                    = true;
            GameManager.Instance.b_nowBattle = true;
            Cursor.lockState                 = CursorLockMode.Confined;
            GameManager.Instance.GO_BattleCanvas.SetActive(true);                                      //전투씬 켜고
            StartCoroutine(BattleMain.Instance.StartBattleScene(prevRoom.GetComponent<RoomController>().CR_creature)); //현재 방에 있는 크리쳐 정보를 넘겨준다.
        }
        else
        {
            if (!prevRoom.TryGetComponent(out BGMPlayer bgm))
                return;

            bgm.StartMusic(!prevBGM.IsUnityNull() ? prevBGM.bgm.name : null);
        }
    }

    public void ExitBattle()
    {
        GameManager.Instance.b_nowBattle = false;
        Cursor.lockState                 = CursorLockMode.Locked;
        prevRoom.GetComponent<RoomController>().DestroyCreature();
    }
}
