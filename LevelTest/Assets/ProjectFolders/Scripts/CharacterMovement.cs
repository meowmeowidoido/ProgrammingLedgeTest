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
    float horizInput; //for horizontal movement input
    float vertInput; //vertical

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

    [Header("Ground Check Settings")]
    public bool isGrounded = false;
    public float groundCheckOffset = 0.5f;
    public Vector3 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;
    public Transform camera;

    public enum MovementType
    {
        running,
        climbing
        
    }
    public MovementType currentMovement;

    [Header("Ledge Grabbing Settings")]
    public bool isGrabbing;
    public float grabDistance;
    public float ledgeCheckOffSet;
    public LayerMask ledgeCheck;
    public float ledgeSpeed;
    RaycastHit ledgeHit;
    Vector3 currentLedge;
    float ledgeDistance;
    public bool lockedOn;
    float timeToReGrab = 1;
    public bool ledgeJumpActive;
    
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
        Debug.Log(timeToReGrab);
        PlayerInputs();
        CameraFacing();
        CheckForGround();
        if (timeToReGrab< 0.6f )
        {
            timeToReGrab += Time.deltaTime;
        }
        if (timeToReGrab >=0.6f)
        {
            rigidbody.freezeRotation = true;
            CheckLedge();
            timeToReGrab = 1;
        }
        }
        private void PlayerInputs()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");
        playerInput = new Vector3(horizInput, 0, vertInput);


    }
    private void FixedUpdate()
    {
        HandleMovement();

    }
    private void PlayerMovement(Vector3 playerDir)
    {


        velocity.x = CalculatePlayerMovement(playerDir.x, velocity.x);
        velocity.z = CalculatePlayerMovement(playerDir.z, velocity.z);

    
        rigidbody.linearVelocity = new Vector3(velocity.x, velocity.y, velocity.z);
    }

    private float CalculatePlayerMovement(float input, float velocity)
    {
        if (lockedOn == false)
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
        }
        return velocity;
    }
   
    public void HandleMovement()
    {

        switch (currentMovement) {
            case MovementType.running:
                PlayerMovement(playerDirection);
                JumpUpdate();
                break;
            case MovementType.climbing:
                JumpUpdate();

                ClimbingMovement();
              
                break;
        }
    }
   

  
  private void ClimbingMovement()
    {
        velocity.x = CalculatePlayerMovement(playerDirection.z, velocity.z);
        rigidbody.linearVelocity = new Vector3(velocity.x,0, playerDirection.z);
        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.linearVelocity = new Vector3(velocity.x, velocity.y, playerDirection.z);

        }
        rigidbody.freezeRotation = true;
        if (Input.GetKey(KeyCode.Q))
        {
            rigidbody.freezeRotation = true;
            Debug.Log("input");
            currentMovement = MovementType.running;
            timeToReGrab = 0;
            isGrabbing = false;
            lockedOn = false;
           // rigidbody.constraints = RigidbodyConstraints.None;
           //JumpUpdate is used to bring the play down with the gravity,thought i figured it out and it isnt working so!
            JumpUpdate();

      


        }
        if (isGrabbing == false)
        {
            

            lockedOn = false;
            rigidbody.useGravity = true;
            rigidbody.constraints = RigidbodyConstraints.None;
            //JumpUpdate is used to bring the play down with the gravity, 


        }

        if (isGrounded)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            //gravity from the actual rigidbody turns false 
          //rigidbody.useGravity = false;
            currentMovement = MovementType.running;
        }
    }



        public void CameraFacing()
    { 
            //camera directions change with input depending on where the players camera is facing
            //the cameras forward position is multiplied with the Z axis of the player input. forward and backwards movement
            Vector3 cameraForward = camera.transform.forward * playerInput.z;
            Vector3 cameraRight = camera.transform.right * playerInput.x;
            //avoids instances where the player goes up when facing their camera up
            cameraForward.y = 0;
            cameraRight.y = 0;
            playerDirection = (cameraForward + cameraRight).normalized;
        
    }
    private void CheckForGround()
    {
        //checks the ground to see whether the player is grounded or not!
        Debug.DrawLine(transform.position + Vector3.down * groundCheckOffset, transform.position + Vector3.down * groundCheckOffset - Vector3.down * groundCheckSize.y / 2, Color.red);
        isGrounded = Physics.CheckBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize / 2, Quaternion.identity, groundCheckMask.value); //if physics box collides with the ground, player is grounded
    }
    private void JumpUpdate()
    {
        if (!isGrounded || lockedOn==false)
        {
            
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        if(isGrounded || lockedOn==true)
        {

           
            velocity.y = -0.1f;
            velocity.y = Mathf.Max(velocity.y, -200);

            if (currentMovement == MovementType.climbing)
            {
                rigidbody.constraints = RigidbodyConstraints.None;
                rigidbody.useGravity = false;

                rigidbody.freezeRotation = true;
                timeToReGrab = 1;
            }
            
               
              
            

         }

       if ((isGrounded || lockedOn==true) && Input.GetKey(KeyCode.Space)) 
        {
            Debug.Log("Jump!");
            velocity.y = initialJumpSpeed;
            isGrounded = false;
            lockedOn = false;

        }

    }
    private void CheckLedge()
    {
        Debug.DrawRay(rigidbody.position, rigidbody.transform.forward, Color.yellow);
        if (isGrabbing = Physics.Raycast(transform.position, rigidbody.transform.forward, out ledgeHit, grabDistance, ledgeCheck.value))
        {
            lockedOn = true;
            currentLedge = ledgeHit.transform.position;
            ledgeDistance = Vector3.Distance(currentLedge, rigidbody.position);
            currentMovement = MovementType.climbing;
            ActivateClimbingFreeze();
        }
        else
        {
            currentMovement = MovementType.running;
            isGrabbing = false;
            lockedOn = false;
        }
   
    }
    private void ActivateClimbingFreeze()
    {
        if (isGrabbing)
        {
            Quaternion lookLedge = Quaternion.LookRotation(ledgeHit.transform.forward);
            Debug.Log("Climbing");
            if (ledgeDistance > 1.5)
            {
                rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, lookLedge * rigidbody.transform.rotation, ledgeSpeed));
                rigidbody.linearVelocity += (currentLedge - rigidbody.position)  ;
                

            }       
            rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, lookLedge * rigidbody.transform.rotation, ledgeSpeed));       
          //rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            rigidbody.freezeRotation = true;
            
        }
 
    }

}

