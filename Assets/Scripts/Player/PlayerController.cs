using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Transform sceneCamera;

    public float walkSpeed = 6.0f;
    public float turnSmoothTime = 0.1f;
    public float explosionForce = 11.0f;
    public float ragdollStunDuration = 5.0f;
    public Animator animator;
    
    public Transform rightHandAttachmentBone;

    // this will go somewhere else, just here to development grenade throw for player.
    public GameObject grenadePrefab;
    public CapsuleCollider capsule;

    CharacterController characterController;
    
    float turnSmoothVelocity;
    bool isThrowing = false;
    bool isRagdolling = false;
    float yVelocity;
    Vector3 moveDirection = Vector3.zero;
    float ragdollStunTimer = 0.0f;

    void Start()
    {
        capsule = GetComponent<CapsuleCollider>();
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
        if (isRagdolling)
        {
            if (transform.position.y < 0)
            {
                Vector3 pos = transform.position;
                pos.y = 0;
                yVelocity = 0;
            }

            ragdollStunTimer -= Time.deltaTime;

            if (ragdollStunTimer <= 0.0f)
            {
                ExitRagdoll();
            }
        }
        else
        {
            if (characterController.enabled && characterController.isGrounded && Input.GetMouseButtonDown(0))
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

            if (!characterController.isGrounded)
            {
                desiredDirection = Vector3.zero;
            }

            if (desiredDirection.magnitude > 0)
            {
                animator.SetInteger("state", 1);
                float targetAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg + sceneCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

                moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            }
            else
            {
                animator.SetInteger("state", 0);

                if (characterController.isGrounded)
                {
                    moveDirection = Vector3.zero;
                }
            }
        }

        yVelocity -= 9.81f * Time.deltaTime;
        characterController.Move(((moveDirection * walkSpeed) + new Vector3(0, yVelocity, 0)) * Time.deltaTime); // * Time.deltaTime);

        if (characterController.isGrounded)
        {
            yVelocity = 0.0f;
        }
    }

    public void OnThrowComplete()
    {
        isThrowing = false;
    }

    public void OnTouchMine()
    {
        yVelocity = explosionForce;
        EnterRagdoll();
    }

    void EnterRagdoll()
    {
        isRagdolling = true;
        animator.enabled = false;
        capsule.enabled = false;
        characterController.enabled = false;
        ragdollStunTimer = ragdollStunDuration;
    }

    void ExitRagdoll()
    {
        isRagdolling = false;
        animator.enabled = true;
        capsule.enabled = true;
        characterController.enabled = true;
    }
}
