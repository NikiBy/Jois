using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Jois.Player;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        private PlayerController playerController;
        private ThirdPersonCamera thirdPersonCamera;
        private bool isWindowOpen = false;

        public bool IsWindowOpen => isWindowOpen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            EnsureInputModuleForUI();

            if (closeButton == null && windowPanel != null)
            {
                closeButton = windowPanel.GetComponentInChildren<Button>();
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(CloseLesson);
                closeButton.onClick.AddListener(CloseLesson);
            }

            if (windowPanel != null) windowPanel.SetActive(false);
            if (promptPanel != null) promptPanel.SetActive(false);
        }

        private void Start()
        {
            FindPlayerReferences();
        }

        private void FindPlayerReferences()
        {
            if (playerController == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    playerController = player.GetComponent<PlayerController>();
                }
            }

            if (thirdPersonCamera == null && Camera.main != null)
            {
                thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
            }
        }

        private void EnsureInputModuleForUI()
        {
            var eventSystem = UnityEngine.EventSystems.EventSystem.current;
            if (eventSystem == null)
            {
                eventSystem = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            }

            if (eventSystem != null)
            {
#if ENABLE_INPUT_SYSTEM
                var legacyModule = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                if (legacyModule != null)
                {
                    DestroyImmediate(legacyModule);
                }

                if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }
#endif
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

            FindPlayerReferences();

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
