using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rigidbody;
    float horizInput;
    float vertInput;

    [Header("Movement Settings")]
    public float maxSpeed = 2;
    public float accelerateTime = 2f;
    public float decelerateTime = 2f;
    float deceleration;
    float acceleration;
    public Vector3 playerInput;
    Vector3 playerDirection; 
    public Vector3 velocity;
    public Vector3 inputDirection;
    [Header("Jump Settings")]
    public float gravity;
    float initialJumpSpeed;
    public float apexHeight = 3f;
    public float apexTime = 0.5f;
    public float maxJumpHeight  = 200f;

    [Header("Ground Check Settings")]
    public bool isGrounded = false;
    public float groundCheckOffset = 0.5f;
    public Vector3 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;
    public Transform camera;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        acceleration = maxSpeed / accelerateTime;
        deceleration = maxSpeed / decelerateTime;
        gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));
        initialJumpSpeed = 2 * apexHeight / apexTime;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        CameraFacing();
        CheckForGround();
      
    }
    private void PlayerInputs()
    { 
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");
        playerInput = new Vector3(horizInput, 0, vertInput);


    }
    private void FixedUpdate()
    {
        PlayerMovement(playerDirection);
        JumpUpdate();

    }
    private void PlayerMovement(Vector3 playerDir)
    {


        velocity.x = CalculatePlayerMovement(playerDir.x, velocity.x);
        velocity.z = CalculatePlayerMovement(playerDir.z, velocity.z);

        //other code that im using to test out movement and the motion
       //float currentSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
       //Vector3 playerVelocity = currentSpeed * playerDirection;
        //rigidbody.linearVelocity = new Vector3(playerVelocity.x, velocity.y, playerVelocity.z);
       rigidbody.linearVelocity = new Vector3(velocity.x, velocity.y, velocity.z);
    }

    private float CalculatePlayerMovement(float input, float velocity)
    {
        if (input != 0)
        {
            
            velocity += acceleration * input * Time.fixedDeltaTime;
            velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);
           

        }
        else
        {
            velocity = Mathf.MoveTowards(velocity, 0, deceleration * Time.deltaTime);
            
        }
        return velocity;
    }

    public void CameraFacing()
    {
        Vector3 cameraForward = camera.transform.forward * playerInput.z;
        Vector3 cameraRight = camera.transform.right * playerInput.x;
        cameraForward.y = 0;
        cameraRight.y = 0;
        playerDirection = (cameraForward + cameraRight).normalized; 
    }
    private void CheckForGround()
    {
        Debug.DrawLine(transform.position + Vector3.down * groundCheckOffset, transform.position + Vector3.down * groundCheckOffset - Vector3.down * groundCheckSize.y / 2, Color.red);
        isGrounded = Physics.CheckBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize / 2, Quaternion.identity, groundCheckMask.value); //if physics box collides with the ground, player is grounded
    }
    private void JumpUpdate()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        if(isGrounded)
        {

            velocity.y = -0.1f;
            velocity.y = Mathf.Max(velocity.y, -200);

        }
        if (isGrounded && Input.GetKey(KeyCode.Space)) 
        {
            Debug.Log("Jump!");
            velocity.y = initialJumpSpeed;
            isGrounded = false;
        }

        // Clamp falling speed to terminal velocity
    }
    
}
