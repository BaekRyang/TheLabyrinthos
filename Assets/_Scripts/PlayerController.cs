using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
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
    
    //카메라 회전
    float rotationSpeed = 2f;
    float distanceFromPlayer = 5f;

    private Vector3 offset;
    public float yaw = 0;

    float minZoomDistance = 3f;
    float maxZoomDistance = 5f;

    //카메라 벽 체크
    public LayerMask wallLayer;
    private float defaultCameraDistance;


    //플레이어 마우스 액션
    RaycastHit hit;
    bool isHit;
    float detectionDistance = 0.2f;
    LayerMask LM_InteractLayerMask; // 검출하고자 하는 레이어를 지정.
    GameObject GO_LastHitGO;

    public float horizontal;
    public float vertical;

    //방 체크
    private int groundLayer;

    // 이전 방의 이름과 게임 오브젝트
    [SerializeField] GameObject prevRoom;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        offset = new Vector3(0, 1, -distanceFromPlayer);
        LM_InteractLayerMask = (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Door"));
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        wallLayer = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Door"));
    }

    private void Start()
    {
        playerTexture = Resources.LoadAll<Texture>("Sprites/Player");
        ResetSetting();
    }

    private void Update()
    {
        if (!b_camControll)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();

                Vector3 cameraRight = Camera.main.transform.right;
                cameraRight.y = 0;
                cameraRight.Normalize();

                direction = cameraForward * vertical + cameraRight * horizontal;
                direction.Normalize();
                Debug.DrawRay(transform.position, direction, Color.yellow);
            }

            //마우스 휠
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                distanceFromPlayer -= scrollInput * rotationSpeed;
                distanceFromPlayer = Mathf.Clamp(distanceFromPlayer, minZoomDistance, maxZoomDistance);
                offset = new Vector3(0, 1, -distanceFromPlayer);
                Quaternion rot = Quaternion.Euler(25, yaw, 0);
                defaultCameraDistance = Vector3.Distance(transform.position + rot * offset, transform.position);
            }


            // 우클릭을 누르고 있는 경우
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * rotationSpeed;
                
            }

            Quaternion rotation = Quaternion.Euler(25, yaw, 0);
            Vector3 newCameraPosition = transform.position + rotation * offset;
            Camera.main.transform.position = newCameraPosition;

            // 레이캐스팅을 사용하여 카메라와 플레이어 사이의 벽을 감지합니다.
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (newCameraPosition - transform.position).normalized, out hit, defaultCameraDistance, wallLayer))
            {
                // 벽을 감지한 경우, 카메라의 거리를 벽과 플레이어 사이로 조절합니다.
                Camera.main.transform.position = hit.point + hit.normal * 0.2f; // 여기서 0.1f는 카메라와 벽 사이의 여유 공간입니다.
            }

            // 기존 코드
            Camera.main.transform.LookAt(transform.position + Vector3.up);
            spriteBox.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);


            //LookAt();  

            //Click Check

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit chit;
            if (Physics.Raycast(ray, out chit, 50, LM_InteractLayerMask))
            {
                if (chit.transform.CompareTag("Interactable"))
                {
                    if (Vector3.Distance(chit.transform.position, transform.position) < 2)
                    {
                        if (GO_PrevInteracted != null && GO_PrevInteracted != chit.transform.gameObject)
                        {
                            Debug.Log("OFF");
                            GO_PrevInteracted.GetComponent<Outline>().enabled = false;
                        }
                        GO_PrevInteracted = chit.transform.gameObject;

                        chit.transform.GetComponent<Outline>().enabled = true;
                        Debug.Log("Interactable");
                        if (Input.GetMouseButtonDown(0))
                        {
                            //Interactable 한 Object를 클릭했을때 처리
                            horizontal = 0;
                            vertical = 0;
                            chit.transform.GetComponent<Interactable>().Run(this);
                            Debug.Log("INTERACTED");
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Debug.Log("Too Far");
                        }
                    }
                }
                else
                {
                    Debug.Log("Not Interactable");
                    if (GO_PrevInteracted != null)
                    {
                        Debug.Log("OFF");
                        GO_PrevInteracted.GetComponent<Outline>().enabled = false;
                        GO_PrevInteracted = null;
                    }
                }
            }
            else
            {
                if (GO_PrevInteracted != null)
                {
                    Debug.Log("OFF");
                    GO_PrevInteracted.GetComponent<Outline>().enabled = false;
                    GO_PrevInteracted = null;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (horizontal == 0 && vertical == 0)
        {
            rigid.velocity = Vector3.zero;
        } else
        {
            Move();
        }

    }

    protected void Move()
    {
        int size = 10;
        // 플레이어의 위치
        Vector3 playerPos = transform.position;

        // 플레이어가 있는 방의 인덱스 계산
        int roomIndexX = Mathf.FloorToInt(((playerPos.x + 5) / size) + 5);
        int roomIndexY = Mathf.FloorToInt(((playerPos.z + 5) / size) + 4) * 10;
        int roomIndex = roomIndexX + roomIndexY;

        // 현재 플레이어가 있는 방의 인덱스 출력
        Debug.Log("Current room index: " + roomIndex);

        prevRoom.GetComponent<RoomController>().ChangeRoomState(false); // 이전 방의 상태를 변경
        prevRoom = GameManager.Instance.GetComponent<RoomCreation>().roomMap[roomIndex].RoomObject; // 플레이어가 위치한 방의 오브젝트를 저장
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true); // 플레이어가 위치한 방의 상태를 변경

        float currentMoveSpeed = moveSpeed;
        Vector3 velocity = new Vector3(direction.x, 0, direction.z);

        rigid.velocity = velocity * currentMoveSpeed;
    }

    protected void LookAt()
    {
        if (horizontal == 0 && vertical == 0)
        {
            // 플레이어가 가만히 있을때
            faceing = 5;
        }
        else if (horizontal == 1 && vertical == 1)
        {
            // player is moving right and up (diagonal)
            faceing = 9;
            spriteBox.GetComponent<Renderer>().material.SetTexture("_PlayerTexture", playerTexture[1]);
            spriteBox.transform.localScale = new Vector3(-1, 1, 0.01f);
        }
        else if (horizontal == 1 && vertical == -1)
        {
            // player is moving right and down (diagonal)
            faceing = 3;
            spriteBox.GetComponent<Renderer>().material.SetTexture("_PlayerTexture", playerTexture[3]);
            spriteBox.transform.localScale = new Vector3(-1, 1, 0.01f);
        }
        else if (horizontal == -1 && vertical == 1)
        {
            // player is moving left and up (diagonal)
            faceing = 7;
            spriteBox.GetComponent<Renderer>().material.SetTexture("_PlayerTexture", playerTexture[1]);
            spriteBox.transform.localScale = new Vector3(1, 1, 0.01f);
        }
        else if (horizontal == -1 && vertical == -1)
        {
            // player is moving left and down (diagonal)
            faceing = 1;
            spriteBox.GetComponent<Renderer>().material.SetTexture("_PlayerTexture", playerTexture[3]);
            spriteBox.transform.localScale = new Vector3(1, 1, 0.01f);
        }
        else if (horizontal == 1)
        {
            // player is moving right
            faceing = 6;
        }
        else if (horizontal == -1)
        {
            // player is moving left
            faceing = 4;
        }
        else if (vertical == 1)
        {
            // player is moving ups
            faceing = 8;
            spriteBox.GetComponent<Renderer>().material.SetTexture("_PlayerTexture", playerTexture[0]);
            spriteBox.transform.localScale = new Vector3(1, 1, 0.01f);
        }
        else if (vertical == -1)
        {
            // player is moving down
            faceing = 2;
            spriteBox.GetComponent<Renderer>().material.SetTexture("_PlayerTexture", playerTexture[2]);
            spriteBox.transform.localScale = new Vector3(1, 1, 0.01f);
        }
    }

    public void ResetSetting()
    {
        prevRoom = GameManager.Instance.GetComponent<RoomCreation>().roomMap[45].RoomObject; // 플레이어가 위치한 방의 루트 오브젝트를 저장 (플레이어는 45번에 생성)
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true); // 플레이어가 위치한 방의 상태를 변경
        defaultCameraDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
    }


}
