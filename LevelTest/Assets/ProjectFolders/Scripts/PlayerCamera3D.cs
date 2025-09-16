using UnityEngine;

public class PlayerCamera3D : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Start is called before the first frame update
   public Rigidbody rb;
    float horizInput;
    float vertInput;
    public float rotationSpeed;

    public Transform objPlayer;
    public Transform orientation;
    public CharacterMovement player;
    void Start()
    {
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.lockedOn == false)
        {
            Direction3DCamera();
        }
        
    }
    private void Direction3DCamera()
    {
        horizInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");
        Vector3 camDirection = rb.position - new Vector3(transform.position.x, rb.position.y, transform.position.z);
        orientation.forward = camDirection.normalized;
        Vector3 inputDirection = orientation.forward * vertInput + orientation.right * horizInput;
        Quaternion directionToRotate = Quaternion.LookRotation(inputDirection);

        if (inputDirection != Vector3.zero)
        {
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, directionToRotate, rotationSpeed * Time.deltaTime));

        }
    }
}
