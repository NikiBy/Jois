using UnityEngine;
using TMPro;
using Jois.Player;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Jois.UI
{
    public class LessonUI : MonoBehaviour
    {
        public static LessonUI Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject windowPanel;
        [SerializeField] private GameObject promptPanel;

        [Header("Text Fields")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI theoryText;
        [SerializeField] private TextMeshProUGUI promptText;

        private PlayerController playerController;
        private ThirdPersonCamera thirdPersonCamera;
        private bool isWindowOpen = false;

        public bool IsWindowOpen => isWindowOpen;

        private void Awake()
        {
            if (Instance != null && Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (windowPanel != null) windowPanel.SetActive(false);
            if (promptPanel != null) promptPanel.SetActive(false);
        }

        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
            }

            if (Camera.main != null)
            {
                thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
            }
        }

        private void Update()
        {
            if (isWindowOpen)
            {
                // Close on Escape
#if ENABLE_INPUT_SYSTEM
                if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    CloseLesson();
                }
#else
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseLesson();
                }
#endif
            }
        }

        public void ShowPrompt(string message)
        {
            if (isWindowOpen) return;

            if (promptText != null) promptText.text = message;
            if (promptPanel != null) promptPanel.SetActive(true);
        }

        public void HidePrompt()
        {
            if (promptPanel != null) promptPanel.SetActive(false);
        }

        public void OpenLesson(string title, string theory)
        {
            isWindowOpen = true;
            HidePrompt();

            if (titleText != null) titleText.text = title;
            if (theoryText != null) theoryText.text = theory;

            if (windowPanel != null) windowPanel.SetActive(true);

            // Freeze player and unlock mouse cursor for UI clicking
            if (playerController != null) playerController.CanMove = false;
            if (thirdPersonCamera != null) thirdPersonCamera.SetCursorState(false);
        }

        public void CloseLesson()
        {
            isWindowOpen = false;
            if (windowPanel != null) windowPanel.SetActive(false);

            // Resume player movement and lock mouse cursor back
            if (playerController != null) playerController.CanMove = true;
            if (thirdPersonCamera != null) thirdPersonCamera.SetCursorState(true);
        }
    }
}
