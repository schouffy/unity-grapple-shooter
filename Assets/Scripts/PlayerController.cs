using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed;
    public float airMovementSpeed;
    public float maxSpeed;

    [Header("Friction")]
    public float friction;
    public float airFriction;

    [Header("Rotation")]
    public float MouseSensitivityX;
    public float MouseSensitivityY;
    public float rotationBounds;

    [Header("Gravity")]
    public float extraGravity;

    [Header("Ground Detection")]
    public LayerMask whatIsGround;
    public float checkYOffset;
    public float checkRadius;
    public float groundTimer;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;

    [Header("Data")]
    public Camera playerCamera;
    public Transform playerCameraHolder;
    public Rigidbody playerRigidBody;

    private float _xRotation;
    private float _yRotation;
    private float _grounded;
    public bool _realGrounded;
    private float _jumpCooldown;

    private Collider[] _colliderList = new Collider[100];

    public AudioClip HitWallFx;
    public float HitWallMakesSoundThreshold;

    private void Start()
    {
        _yRotation = transform.rotation.eulerAngles.y;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        ApplyMovement();
        ApplyFriction();
        ApplyGravity();
    }

    private void Update()
    {
        Rotation();
        Jumping();
    }

    private void GroundCheck()
    {
        _grounded -= Time.fixedDeltaTime;
        var size = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, checkYOffset, 0), checkRadius, _colliderList, whatIsGround);
        _realGrounded = size > 0;
        if (_realGrounded)
            _grounded = groundTimer;
    }

    private void ApplyMovement()
    {
        var inputX = Input.GetAxisRaw("Horizontal") < 0 ? -1 : (Input.GetAxisRaw("Horizontal") > 0 ? 1 : 0);
        var inputY = Input.GetAxisRaw("Vertical") < 0 ? -1 : (Input.GetAxisRaw("Vertical") > 0 ? 1 : 0);
        var axis = new Vector2(inputX, inputY).normalized;

        var speed = _realGrounded ? movementSpeed : airMovementSpeed;
        var vertical = axis.y * speed * Time.fixedDeltaTime * transform.forward;
        var horizontal = axis.x * speed * Time.fixedDeltaTime * transform.right;

        if (CanApplyForce(vertical, axis))
            playerRigidBody.velocity += vertical;

        if (CanApplyForce(horizontal, axis))
            playerRigidBody.velocity += horizontal;
    }

    private void ApplyFriction()
    {
        var vel = playerRigidBody.velocity;
        var target = _realGrounded ? friction : airFriction;
        vel.x = Mathf.Lerp(vel.x, 0f, target * Time.fixedDeltaTime);
        vel.z = Mathf.Lerp(vel.z, 0f, target * Time.fixedDeltaTime);
        playerRigidBody.velocity = vel;
    }

    private void Rotation()
    {
        Cursor.lockState = CursorLockMode.Locked;
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X") * MouseSensitivityX, Input.GetAxisRaw("Mouse Y") * MouseSensitivityY);

        _xRotation -= mouseDelta.y;
        _xRotation = Mathf.Clamp(_xRotation, -rotationBounds, rotationBounds);
        _yRotation += mouseDelta.x;

        transform.rotation = Quaternion.Euler(0, _yRotation, 0);
        playerCameraHolder.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }

    private void ApplyGravity()
    {
        var vel = playerRigidBody.velocity;
        vel.y -= Mathf.Abs(vel.y) * Time.fixedDeltaTime * extraGravity;
        playerRigidBody.velocity = vel;
    }

    private void Jumping()
    {
        _jumpCooldown -= Time.deltaTime;
        if (!(_grounded >= 0) || !(_jumpCooldown <= 0) || !Input.GetButtonDown("Jump")) return;
        var vel = playerRigidBody.velocity;
        vel.y = jumpForce;
        playerRigidBody.velocity = vel;
        _jumpCooldown = jumpCooldown;
    }

    private bool CanApplyForce(Vector3 target, Vector2 axis)
    {
        var targetC = Get2DVec(target).normalized;
        var velocityC = Get2DVec(playerRigidBody.velocity).normalized;
        var dotProduct = Vector2.Dot(velocityC, targetC);
        return dotProduct <= 0 || dotProduct * Get2DVec(playerRigidBody.velocity).magnitude < maxSpeed * GetAxisForce(axis);
    }

    private static float GetAxisForce(Vector2 axis)
    {
        return (int)axis.x != 0 ? Mathf.Abs(axis.x) : Mathf.Abs(axis.y);
    }

    private static Vector2 Get2DVec(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > HitWallMakesSoundThreshold)
        {
            GetComponent<AudioSource>().PlayOneShot(HitWallFx);
        }
    }

    public void RemoveControlFromPlayer()
    {
        this.GetComponentInChildren<HandsSpring>().enabled = false;
        this.GetComponentInChildren<HandsSway>().enabled = false;
        this.GetComponentInChildren<PlayerShooter>().enabled = false;
        this.GetComponentInChildren<GrapplingHook>().enabled = false;

        this.enabled = false;
    }
}
