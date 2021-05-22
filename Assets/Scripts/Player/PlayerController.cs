using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Transform sceneCamera;

    public float walkSpeed = 6.0f;
    public float turnSmoothTime = 0.1f;
    public Animator animator;

    public Transform rightHandAttachmentBone;

    // this will go somewhere else, just here to development grenade throw for player.
    public GameObject grenadePrefab;

    CharacterController characterController;
    
    float turnSmoothVelocity;
    bool isThrowing = false;
    float gravity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        //https://answers.unity.com/questions/741074/why-character-controller-floats-5-cm-above-ground.html
        // calculate the correct vertical position:
        float correctHeight = characterController.center.y + characterController.skinWidth;
        // set the controller center vector:
        characterController.center = new Vector3(0, correctHeight, 0);

        if (grenadePrefab != null)
        {
            GameObject grenade = Instantiate(grenadePrefab);
            grenade.transform.parent = rightHandAttachmentBone;
            grenade.transform.localPosition = new Vector3(0.069f, -0.048f, -0.028f);
            grenade.transform.Rotate(new Vector3(90.0f, 0, 0));
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("requestThrow");
            isThrowing = true;
        }

        if (isThrowing)
        {
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 desiredDirection = new Vector3(h, 0.0f, v).normalized;

        Vector3 moveDirection = Vector3.zero;

        if (desiredDirection.magnitude > 0)
        {
            animator.SetInteger("state", 1);
            float targetAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg + sceneCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
            
            moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            characterController.Move(moveDirection * walkSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetInteger("state", 0);
        }

        gravity -= 9.81f * Time.deltaTime;
        characterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        if (characterController.isGrounded)
        {
            gravity = 0.0f;
        }
    }

    public void OnThrowComplete()
    {
        isThrowing = false;
    }
}
