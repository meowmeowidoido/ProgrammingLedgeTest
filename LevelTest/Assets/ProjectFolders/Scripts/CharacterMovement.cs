using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rigidbody;
    float horizInput;
    float vertInput;
    float rotationSpeed;

    public Transform playerOrientation;
    [Header("Movement Settings")]
    public float maxSpeed = 5;
    public float accelerateTime = 0.2f;
    public float decelerateTime = 20f;
    public float rotateSpeed = 3;
    Vector3 velocity;
    Quaternion currentRotation;
    [Header("Jump Settings")]
    bool grounded;
    
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
    
    }

    // Update is called once per frame
    void Update()
    {
        horizInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");
    }
    void PlayerMovement()
    {
        if(vertInput!=0)
        {
            velocity.z += vertInput * Time.deltaTime;

        }
        if(horizInput!=0)
        {
            velocity.x += horizInput * Time.deltaTime;
           // velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        if (horizInput==0 && vertInput==0)
        {
            //rigidbody.linearVelocity = new Vector3 (velocity.x , 0 , velocity.z);
        
           
          // velocity.x = Mathf.MoveTowards(velocity.x, 0, (decelerateTime*2) * Time.fixedDeltaTime);
       
         //   velocity.z = Mathf.MoveTowards(velocity.z,0, (decelerateTime* 2)*Time.fixedDeltaTime);
        }
    }
    private void FixedUpdate()
    {
        PlayerMovement();
        rigidbody.linearVelocity= new Vector3(velocity.x, velocity.y, velocity.z) * maxSpeed;
    }
}
