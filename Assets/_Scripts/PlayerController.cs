using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const float RAY_DISTANCE = 1f;
    
    private RaycastHit slopeHit;
    private float maxSlopeAngle = 90f;

    [SerializeField] Transform groundCheck;

    private int groundLayer;

    public float MoveSpeed { get { return MoveSpeed; } }
    [SerializeField] float moveSpeed;
    Rigidbody rigid;
    Vector3 direction;

    Vector3 camPos;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        camPos = Camera.main.transform.position;
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        direction = new Vector3(-input.x, 0f, -input.y);
    }

    protected void Move()
    {
        Camera.main.transform.position = camPos + transform.position;
        float currentMoveSpeed = moveSpeed;

        bool isOnSlope = IsOnSlope();
        bool isGrounded = IsGrounded();
        Vector3 velocity = direction;
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
            

        LookAt();
        rigid.velocity = velocity * currentMoveSpeed + gravity;
    }

    protected void LookAt()
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(direction);
            rigid.rotation = targetAngle;
        }
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
