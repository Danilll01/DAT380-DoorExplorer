using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Rigidbody))]
public class PlayerPhysicsMovement : PortalTraveller
{
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float runSpeed = 6;
    [SerializeField] private float smoothMoveTime = 0.1f;
    [SerializeField] private float jumpForce = 8;
    [SerializeField] private float gravity = 0;

    [SerializeField] private bool lockCursor;
    [SerializeField] private float mouseSensitivity = 10;
    [SerializeField] private Vector2 pitchMinMax = new Vector2 (-40, 85);
    [SerializeField] private float rotationSmoothTime = 0.1f;

    Rigidbody controller;
    Camera cam;
    [SerializeField] private float yaw;
    [SerializeField] private float pitch;
    float smoothYaw;
    float smoothPitch;

    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    bool jumping;
    float lastGroundedTime;
    [SerializeField] private bool isGrounded = true;
    bool disabled;

    void Start () {
        cam = Camera.main;
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<Rigidbody>();

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.P)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Break ();
        }
        if (Input.GetKeyDown (KeyCode.O)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            disabled = !disabled;
        }

        if (disabled) {
            return;
        }
        
        float mX = Input.GetAxisRaw ("Mouse X");
        float mY = Input.GetAxisRaw ("Mouse Y");

        // Verrrrrry gross hack to stop camera swinging down at start
        float mMag = Mathf.Sqrt (mX * mX + mY * mY);
        if (mMag > 5) {
            mX = 0;
            mY = 0;
        }

        yaw += mX * mouseSensitivity;
        pitch -= mY * mouseSensitivity;
        pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);
        smoothPitch = Mathf.SmoothDampAngle (smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        smoothYaw = Mathf.SmoothDampAngle (smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

        transform.eulerAngles = Vector3.up * smoothYaw;
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;
        
        
        // Jumping
        if (isGrounded) {
            //jumping = false;
            lastGroundedTime = Time.time;
        }
        else
        {
            CheckGround();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (isGrounded || (timeSinceLastTouchedGround < 0.15f)) {
                //jumping = true;
                
                // Adds force to the player rigidbody to jump
                controller.velocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
                controller.AddForce(0f, jumpForce, 0f, ForceMode.Impulse); 
                isGrounded = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (disabled) {
            return;
        }
        
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

        Vector3 inputDir = new Vector3 (input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir);

        float currentSpeed = (Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed;
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        
        //Vector3 velocityChange = (targetVelocity - controller.velocity);
        // velocityChange.x = Mathf.Clamp(velocityChange.x, -10, 10);
        // velocityChange.z = Mathf.Clamp(velocityChange.z, -10, 10);
        // velocityChange.y = 0;
        
        
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, smoothMoveTime);
        
        //verticalVelocity -= gravity * Time.deltaTime;
        velocity = new Vector3 (velocity.x, controller.velocity.y, velocity.z);
        //velocity = new Vector3 (velocity.x, 0, velocity.z);
        

        //var flags = controller.Move(velocity * Time.deltaTime);
        //controller.AddForce(velocity, ForceMode.VelocityChange);
        controller.velocity = velocity;
        
        
        if (!isGrounded)
        {
            controller.AddForce(0, -gravity, 0, ForceMode.Acceleration);
        }
    }

    // Sets isGrounded based on a raycast sent straigth down from the player object
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = 0.75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle (smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
        Physics.SyncTransforms();
    }
}
