using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using Input = UnityEngine.Input;

public class PlayerController : MonoBehaviour
{
    private const float RAY_DISTANCE = 1f;
    
    private RaycastHit slopeHit;
    private float maxSlopeAngle = 90f;

    [SerializeField] Transform groundCheck;

    private int groundLayer;

    [SerializeField] bool isOnSlope;
    [SerializeField] bool isGrounded;

    public float MoveSpeed { get { return MoveSpeed; } }
    [SerializeField] float moveSpeed;
    Rigidbody rigid;
    Vector3 direction;

    Vector3 camPos;

    public bl_Joystick js;
    float horizontal;
    float vertical;

    private void Update()
    {
        //horizontal = Input.GetAxisRaw("Horizontal");
        //vertical = Input.GetAxisRaw("Vertical");

        horizontal = js.Horizontal;
        vertical = js.Vertical;



        direction = transform.forward * vertical + transform.right * horizontal;
        direction.Normalize();
        Debug.DrawRay(transform.position, direction, Color.yellow);

        LookAt();

        //if (Input.touchCount > 0)
        //{
        //    if (Input.GetTouch(0).phase == TouchPhase.Began)
        //    {
        //        firstPoint = Input.GetTouch(0).position;
        //        xAngleTemp = xAngle;
        //        yAngleTemp = yAngle;
        //    }
        //    if (Input.GetTouch(0).phase == TouchPhase.Moved)
        //    {
        //        secondPoint= Input.GetTouch(0).position;
        //        xAngle = xAngleTemp + (secondPoint.x - firstPoint.x) * 180 / Screen.width;
        //        yAngle = yAngleTemp - (secondPoint.y - firstPoint.x) * 90 * 3f / Screen.height; //y값 변화가 느려서 3x

        //        //회전값 40 ~ 85 Clamp
        //        if (yAngle < 40f) yAngle = 40f;
        //        if (yAngle > 85f) yAngle = 80f;

        //        transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
        //    }
        //}
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        camPos = Camera.main.transform.position;
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
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
        Camera.main.transform.position = camPos + transform.position;
        float currentMoveSpeed = moveSpeed;

        isOnSlope = IsOnSlope();
        isGrounded = IsGrounded();
        Vector3 velocity = new Vector3(direction.x, rigid.velocity.y, direction.z);
        Vector3 gravity = Vector3.down * Mathf.Abs(rigid.velocity.y);

        if (isGrounded && isOnSlope)
        {
            velocity = AdjustDirectionToSlope(direction);
            gravity = Vector3.zero;
            rigid.useGravity = false;
        }
        else
        {
            rigid.useGravity = true;
        }
        rigid.velocity = velocity * currentMoveSpeed + gravity;
    }

    protected void LookAt()
    {
        //if (direction != Vector3.zero)
        //{
        //    Quaternion targetAngle = Quaternion.LookRotation(direction);
        //    rigid.rotation = targetAngle;
        //}
        Vector3 tmpRot = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        rigid.rotation = Quaternion.Euler(tmpRot);
    }
    
    protected bool IsOnSlope()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out slopeHit, RAY_DISTANCE, groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle != 0f && angle < maxSlopeAngle;
        }
        return false;
    }

    protected Vector3 AdjustDirectionToSlope(Vector3 direction)
    { //전진하고 있는 방향 Vector와 서있는 땅의 Normal Vector를 통해 서있는 땅의 기울어진 방향의 Vector를 반환한다.
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    protected bool IsGrounded()
    {
        Vector3 boxSize = new Vector3(transform.lossyScale.x/2, 0.05f, transform.lossyScale.z/2);
        return Physics.CheckBox(groundCheck.position, boxSize, Quaternion.identity, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 boxSize = new Vector3(transform.lossyScale.x/2, 0.05f, transform.lossyScale.z/2);
        Gizmos.DrawWireCube(groundCheck.position, boxSize);
    }
}
