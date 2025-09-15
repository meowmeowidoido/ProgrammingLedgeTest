using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ClimbingController : MonoBehaviour
{
    [Header("Ledge Grabbing Settings")]
    public bool isGrabbing;
    public float grabDistance;
    public float ledgeCheckOffSet;
    public LayerMask ledgeCheck;
    public float ledgeSpeed;
    RaycastHit ledgeHit;
    Vector3 currentLedge;
    float ledgeDistance;
    public CharacterMovement characterController;
    public PlayerCamera3D playerCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLedge();
    }

    private void CheckLedge()
    {
        Debug.DrawRay(characterController.rigidbody.position, characterController.rigidbody.transform.forward,Color.yellow);
       if(isGrabbing = Physics.Raycast(characterController.transform.position,characterController.rigidbody.transform.forward, out ledgeHit,grabDistance, ledgeCheck.value)){
            currentLedge = ledgeHit.transform.position;
            ledgeDistance = Vector3.Distance(currentLedge, characterController.rigidbody.position);
            ActivateClimbing();
        }
    }
    private void ActivateClimbing()
    {
        if(isGrabbing)
        {
            Debug.Log("Climbing");
            characterController.velocity.y = ledgeHit.transform.position.y;
            characterController.rigidbody.linearVelocity += (currentLedge - characterController.rigidbody.position);
         //   characterController.
        }
    }

}
