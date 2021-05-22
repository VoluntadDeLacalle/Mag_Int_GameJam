using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Transform sceneCamera;

    public float walkSpeed = 6.0f;
    public float turnSmoothTime = 0.1f;
    public float minDirectionChangeThreshold = 0.1f;
    public Animator animator;

    CharacterController characterController;
    
    float turnSmoothVelocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 desiredDirection = new Vector3(h, 0.0f, v).normalized;

        //animator.SetInteger("state", desiredDirection.magnitude > 0 ? 1 : 0);

        if (desiredDirection.magnitude >= minDirectionChangeThreshold)
        {
            animator.SetInteger("state", 1);
            float targetAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg + sceneCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
            
            Vector3 moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            characterController.SimpleMove(moveDirection * walkSpeed);
        }
        else
        {
            animator.SetInteger("state", 0);
        }
    }
}
