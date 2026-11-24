using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Jois.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 6f;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float rotationSmoothTime = 0.08f;
        [SerializeField] private float gravity = -25f;
        [SerializeField] private float jumpHeight = 1.5f;

        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        private CharacterController characterController;
        private Vector3 verticalVelocity;
        private float rotationVelocity;
        private bool isGrounded;
        private bool canMove = true;

        public bool CanMove
        {
            get => canMove;
            set
            {
                canMove = value;
                if (!canMove)
                {
                    verticalVelocity = Vector3.zero;
                }
            }
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            // Ground check
            isGrounded = characterController.isGrounded;
            if (isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -2f; // Firm ground snap
            }

            if (!canMove) return;

            Vector2 input = GetMovementInput();
            bool isRunning = GetRunInput();
            bool jumpPressed = GetJumpInput();

            Vector3 horizontalMove = Vector3.zero;

            if (input.sqrMagnitude >= 0.01f)
            {
                // Project camera directions strictly onto the horizontal plane (Y = 0)
                // This prevents flying or diving when looking up or down!
                Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
                Vector3 camRight = cameraTransform != null ? cameraTransform.right : Vector3.right;
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                Vector3 targetDirection = (camForward * input.y + camRight * input.x).normalized;

                if (targetDirection.sqrMagnitude >= 0.001f)
                {
                    float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                    float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                    float currentSpeed = isRunning ? runSpeed : walkSpeed;
                    horizontalMove = targetDirection * currentSpeed;
                }
            }

            // Jump
            if (jumpPressed && isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Gravity
            verticalVelocity.y += gravity * Time.deltaTime;
            if (verticalVelocity.y < -35f) verticalVelocity.y = -35f; // Clamp fall speed

            // Combine into ONE unified movement call (essential for smooth physics & ground detection)
            Vector3 finalMovement = horizontalMove + verticalVelocity;
            characterController.Move(finalMovement * Time.deltaTime);
        }

        private Vector2 GetMovementInput()
        {
            float horizontal = 0f;
            float vertical = 0f;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
            }
#else
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
#endif
            return new Vector2(horizontal, vertical);
        }

        private bool GetRunInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            }
            return false;
#else
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
        }

        private bool GetJumpInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return Keyboard.current.spaceKey.wasPressedThisFrame;
            }
            return false;
#else
            return Input.GetKeyDown(KeyCode.Space);
#endif
        }
    }
}
