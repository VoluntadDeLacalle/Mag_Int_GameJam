using UnityEngine;
using UnityEngine.InputSystem;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region Variables       

        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Mouse1;
        public KeyCode sprintInput = KeyCode.LeftShift;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        [HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;

        private bool canMove = true;

        #endregion

        protected virtual void Start()
        {
            InitilizeController();
            InitializeTpCamera();
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            InputHandle();                  // update the input methods
            cc.UpdateAnimator();            // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
        }

        public bool CanMove()
        {
            return canMove;
        }

        public void ShouldMove(bool shouldMove)
        {
            canMove = shouldMove;
        }

        public virtual void MoveInput()
        {
            if (Time.timeScale < 0.1f || !canMove || !Player.Instance.IsAlive())
            {
                cc.input.x = 0;
                cc.input.z = 0;
            }
            else
            {
                Vector2 moveDirection = GameManager.Instance.inputManager.actions["Move"].ReadValue<Vector2>();
                moveDirection.x = Mathf.Clamp(moveDirection.x, -1, 1);
                moveDirection.y = Mathf.Clamp(moveDirection.y, -1, 1);

                cc.input.x = moveDirection.x;
                cc.input.z = moveDirection.y;
            }
        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            float X, Y;
            if (Time.timeScale < 0.1f)
            {
                X = 0;
                Y = 0;
            }
            else
            {
                Vector2 lookDirection = GameManager.Instance.inputManager.actions["Look"].ReadValue<Vector2>();
                X = lookDirection.x;
                Y = lookDirection.y;
            }

            gameObject.GetComponent<Animator>().SetFloat("CameraHorizontal", Mathf.Clamp(Mathf.Abs(X), 0, 1));

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            if (GameManager.Instance.inputManager.actions["Aim"].WasPressedThisFrame())
                cc.Strafe(true);
            else if (GameManager.Instance.inputManager.actions["Aim"].WasReleasedThisFrame())
                cc.Strafe(false);
        }

        protected virtual void SprintInput()
        {
            if (GameManager.Instance.inputManager.actions["Sprint"].WasPressedThisFrame())
                cc.Sprint(true);
            else if (GameManager.Instance.inputManager.actions["Sprint"].WasReleasedThisFrame())
                cc.Sprint(false);
            
            if (cc.input.magnitude < 0.1f)
            {
                cc.Sprint(false);
            }
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Time.timeScale < 0.1f || !canMove || !Player.Instance.IsAlive())
            {
                return;
            }

            if (GameManager.Instance.inputManager.actions["Jump"].WasPressedThisFrame() && JumpConditions())
                cc.Jump();
        }

        #endregion       
    }
}