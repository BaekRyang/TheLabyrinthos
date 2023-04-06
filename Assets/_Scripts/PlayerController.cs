using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = UnityEngine.Input;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject spriteBox;
    [SerializeField] Material playerMat;

    [SerializeField] Texture[] playerTexture;

    [SerializeField] Transform groundCheck;

    [SerializeField] GameObject GO_PrevInteracted = null;
    public float MoveSpeed { get { return MoveSpeed; } }
    [SerializeField] float moveSpeed;
    Rigidbody rigid;
    public Vector3 direction;
    int faceing = 0;
    
    //카메라 회전
    float rotationSpeed = 2f;
    float distanceFromPlayer = 5f;

    private Vector3 offset;
    private float yaw = 0;

    float minZoomDistance = 3f;
    float maxZoomDistance = 5f;

    //

    //플레이어 마우스 액션
    RaycastHit hit;
    bool isHit;
    float detectionDistance = 0.2f;
    LayerMask LM_InteractLayerMask; // 검출하고자 하는 레이어를 지정합니다.
    GameObject GO_LastHitGO;
    //

    float horizontal;
    float vertical;

    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        offset = new Vector3(0, 1, -distanceFromPlayer);
        LM_InteractLayerMask = 1 << LayerMask.NameToLayer("Interactable");
    }

    private void Start()
    {
        playerTexture = Resources.LoadAll<Texture>("Sprites/Player");
    }

    private void Update()
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
            Debug.Log(scrollInput);
            distanceFromPlayer -= scrollInput * rotationSpeed;
            distanceFromPlayer = Mathf.Clamp(distanceFromPlayer, minZoomDistance, maxZoomDistance);
            offset = new Vector3(0, 1, -distanceFromPlayer);
        }


        // 우클릭을 누르고 있는 경우
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        }

        Quaternion rotation = Quaternion.Euler(25, yaw, 0);
        Camera.main.transform.position = transform.position + rotation * offset;
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
                        chit.transform.GetComponent<Interactable>().Run();
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
        } else
        {
            if (GO_PrevInteracted != null)
            {
                Debug.Log("OFF");
                GO_PrevInteracted.GetComponent<Outline>().enabled = false;
                GO_PrevInteracted = null;
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

    private void OnDrawGizmos()
    {
        
    }
}
