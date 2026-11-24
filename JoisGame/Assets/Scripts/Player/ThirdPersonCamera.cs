using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Jois.Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Distance & Offset")]
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);
        [SerializeField] private float distance = 5f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 10f;

        [Header("Sensitivity")]
        [SerializeField] private float mouseSensitivityX = 150f;
        [SerializeField] private float mouseSensitivityY = 100f;
        [SerializeField] private float minPitch = -20f;
        [SerializeField] private float maxPitch = 60f;

        [Header("Smoothing")]
        [SerializeField] private float smoothTime = 0.05f;

        private float yaw = 0f;
        private float pitch = 15f;
        private Vector3 currentVelocity;

        private void Start()
        {
            if (target == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }

            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;

            // Lock cursor for comfortable game experience
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            HandleInput();

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 targetPosition = target.position + targetOffset;
            Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
            transform.LookAt(targetPosition);
        }

        private void HandleInput()
        {
            Vector2 mouseDelta = GetMouseDelta();

            yaw += mouseDelta.x * mouseSensitivityX * 0.02f;
            pitch -= mouseDelta.y * mouseSensitivityY * 0.02f;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // Toggle cursor lock with Escape
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ToggleCursor();
            }
#else
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursor();
            }
#endif
        }

        private Vector2 GetMouseDelta()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
            {
                return Mouse.current.delta.ReadValue();
            }
            return Vector2.zero;
#else
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
            return Vector2.zero;
#endif
        }

        public void SetCursorState(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private void ToggleCursor()
        {
            bool isLocked = Cursor.lockState == CursorLockMode.Locked;
            SetCursorState(!isLocked);
        }
    }
}
