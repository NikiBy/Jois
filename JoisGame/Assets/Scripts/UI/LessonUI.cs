using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Jois.Player;
using Jois.Level;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Jois.UI
{
    public class LessonUI : MonoBehaviour
    {
        public static LessonUI Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject windowPanel;
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private GameObject quizContainer;
        [SerializeField] private GameObject bugContainer;
        [SerializeField] private GameObject feedbackPanel;

        [Header("Text Fields")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI theoryText;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private TextMeshProUGUI quizQuestionText;
        [SerializeField] private TextMeshProUGUI bugTaskText;
        [SerializeField] private TextMeshProUGUI feedbackText;

        [Header("Buttons & Parents")]
        [SerializeField] private Transform quizOptionsParent;
        [SerializeField] private Transform bugLinesParent;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button completeLessonButton;
        [SerializeField] private Image feedbackBackground;

        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI hudProgressText;

        private PlayerController playerController;
        private ThirdPersonCamera thirdPersonCamera;
        private bool isWindowOpen = false;
        private LessonData currentLessonData;
        private bool isCurrentLessonSolved = false;

        private readonly List<GameObject> activeOptionButtons = new List<GameObject>();
        private readonly List<GameObject> activeBugLineButtons = new List<GameObject>();

        public bool IsWindowOpen => isWindowOpen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            FixEventSystemModule();

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(CloseLesson);
            }

            if (completeLessonButton != null)
            {
                completeLessonButton.onClick.RemoveAllListeners();
                completeLessonButton.onClick.AddListener(OnCompleteLessonClicked);
            }

            if (windowPanel != null) windowPanel.SetActive(false);
            if (promptPanel != null) promptPanel.SetActive(false);
            if (feedbackPanel != null) feedbackPanel.SetActive(false);
        }

        private void Start()
        {
            FindPlayerReferences();

            if (ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.OnLevelUnlocked += UpdateHUD;
                UpdateHUD(ProgressionManager.Instance.CurrentUnlockedLevel);
            }
        }

        private void OnDestroy()
        {
            if (ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.OnLevelUnlocked -= UpdateHUD;
            }
        }

        public void UpdateHUD(int unlockedLevel)
        {
            if (hudProgressText != null)
            {
                int max = ProgressionManager.Instance != null ? ProgressionManager.Instance.MaxLevels : 10;
                hudProgressText.text = $"Прогресс: {unlockedLevel} / {max} уровней";
            }
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

        private void FixEventSystemModule()
        {
            var eventSystem = UnityEngine.EventSystems.EventSystem.current;
            if (eventSystem == null)
            {
                eventSystem = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            }

            if (eventSystem != null)
            {
                var legacy = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                if (legacy != null)
                {
                    DestroyImmediate(legacy);
                }

                if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }
            }
        }

        private void Update()
        {
            if (isWindowOpen)
            {
                if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    CloseLesson();
                }
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

        public void OpenLesson(LessonData data)
        {
            currentLessonData = data;
            isCurrentLessonSolved = false;

            isWindowOpen = true;
            HidePrompt();
            FindPlayerReferences();
            FixEventSystemModule();

            if (windowPanel != null) windowPanel.SetActive(true);

            // Freeze player movement and show cursor
            if (playerController != null) playerController.CanMove = false;
            if (thirdPersonCamera != null) thirdPersonCamera.SetCursorState(false);

            // Fill header & theory
            if (titleText != null) titleText.text = data != null ? data.LessonTitle : "Урок";
            if (theoryText != null) theoryText.text = data != null ? data.TheoryText : "";

            // Reset complete button
            SetCompleteButtonState(false);
            if (feedbackPanel != null) feedbackPanel.SetActive(false);

            if (data == null) return;

            if (data.LessonType == LessonType.Quiz)
            {
                SetupQuiz(data);
            }
            else
            {
                SetupFindBug(data);
            }
        }

        // Backward compatibility overload
        public void OpenLesson(string title, string theory)
        {
            currentLessonData = null;
            isCurrentLessonSolved = false;

            isWindowOpen = true;
            HidePrompt();
            FindPlayerReferences();
            FixEventSystemModule();

            if (windowPanel != null) windowPanel.SetActive(true);
            if (playerController != null) playerController.CanMove = false;
            if (thirdPersonCamera != null) thirdPersonCamera.SetCursorState(false);

            if (titleText != null) titleText.text = title;
            if (theoryText != null) theoryText.text = theory;

            if (quizContainer != null) quizContainer.SetActive(false);
            if (bugContainer != null) bugContainer.SetActive(false);
            if (feedbackPanel != null) feedbackPanel.SetActive(false);

            SetCompleteButtonState(true);
        }

        private void SetupQuiz(LessonData data)
        {
            if (quizContainer != null) quizContainer.SetActive(true);
            if (bugContainer != null) bugContainer.SetActive(false);

            if (quizQuestionText != null) quizQuestionText.text = data.QuizQuestion;

            ClearDynamicItems(activeOptionButtons);

            if (quizOptionsParent == null) return;

            for (int i = 0; i < data.QuizAnswers.Length; i++)
            {
                int index = i;
                string answerText = data.QuizAnswers[i];

                GameObject btnObj = CreateStyledButton(quizOptionsParent, answerText, new Color(0.18f, 0.22f, 0.3f, 1f));
                activeOptionButtons.Add(btnObj);

                var button = btnObj.GetComponent<Button>();
                button.onClick.AddListener(() => OnQuizAnswerSelected(index, data, btnObj));
            }
        }

        private void OnQuizAnswerSelected(int selectedIndex, LessonData data, GameObject clickedBtn)
        {
            var img = clickedBtn.GetComponent<Image>();

            if (selectedIndex == data.CorrectQuizIndex)
            {
                if (img != null) img.color = new Color(0.18f, 0.65f, 0.28f, 1f); // Green
                ShowFeedback(true, $"Верно! {data.QuizExplanation}");
                isCurrentLessonSolved = true;
                SetCompleteButtonState(true);
            }
            else
            {
                if (img != null) img.color = new Color(0.8f, 0.25f, 0.22f, 1f); // Red
                ShowFeedback(false, "Не совсем так. Вспомните теорию слева и попробуйте другой вариант!");
            }
        }

        private void SetupFindBug(LessonData data)
        {
            if (quizContainer != null) quizContainer.SetActive(false);
            if (bugContainer != null) bugContainer.SetActive(true);

            if (bugTaskText != null) bugTaskText.text = data.BugTaskDescription;

            ClearDynamicItems(activeBugLineButtons);

            if (bugLinesParent == null) return;

            for (int i = 0; i < data.CodeLines.Length; i++)
            {
                int lineIndex = i;
                string lineText = data.CodeLines[i];

                GameObject lineObj = CreateStyledButton(bugLinesParent, lineText, new Color(0.14f, 0.16f, 0.22f, 1f), true);
                activeBugLineButtons.Add(lineObj);

                var button = lineObj.GetComponent<Button>();
                button.onClick.AddListener(() => OnBugLineSelected(lineIndex, data, lineObj));
            }
        }

        private void OnBugLineSelected(int selectedLineIndex, LessonData data, GameObject clickedLine)
        {
            var img = clickedLine.GetComponent<Image>();

            if (selectedLineIndex == data.BugLineIndex)
            {
                if (img != null) img.color = new Color(0.18f, 0.65f, 0.28f, 1f); // Green
                ShowFeedback(true, $"Отлично! {data.BugExplanation}");
                isCurrentLessonSolved = true;
                SetCompleteButtonState(true);
            }
            else
            {
                if (img != null) img.color = new Color(0.8f, 0.25f, 0.22f, 1f); // Red
                string hint = !string.IsNullOrEmpty(data.GeneralHint) ? data.GeneralHint : "В этой строке синтаксис верный. Проверьте другие строки!";
                ShowFeedback(false, $"Ошибка в другой строке! {hint}");
            }
        }

        private void ShowFeedback(bool isSuccess, string message)
        {
            if (feedbackPanel != null) feedbackPanel.SetActive(true);
            if (feedbackText != null) feedbackText.text = message;

            if (feedbackBackground != null)
            {
                feedbackBackground.color = isSuccess
                    ? new Color(0.12f, 0.45f, 0.2f, 0.95f)
                    : new Color(0.6f, 0.2f, 0.18f, 0.95f);
            }
        }

        private void SetCompleteButtonState(bool enabled)
        {
            if (completeLessonButton != null)
            {
                completeLessonButton.interactable = enabled;
                var img = completeLessonButton.GetComponent<Image>();
                if (img != null)
                {
                    img.color = enabled ? new Color(0.15f, 0.65f, 0.35f, 1f) : new Color(0.3f, 0.35f, 0.4f, 0.5f);
                }
            }
        }

        private void OnCompleteLessonClicked()
        {
            if (currentLessonData != null && ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.UnlockNextLevel(currentLessonData.LessonNumber);
            }

            CloseLesson();
            ShowPrompt("🎉 Урок успешно пройден! Следующий домик открыт!");
        }

        public void CloseLesson()
        {
            isWindowOpen = false;
            if (windowPanel != null) windowPanel.SetActive(false);

            // Resume player movement and lock cursor
            if (playerController != null) playerController.CanMove = true;
            if (thirdPersonCamera != null) thirdPersonCamera.SetCursorState(true);
        }

        private void ClearDynamicItems(List<GameObject> list)
        {
            foreach (var item in list)
            {
                if (item != null) Destroy(item);
            }
            list.Clear();
        }

        private GameObject CreateStyledButton(Transform parent, string text, Color bgColor, bool alignLeft = false)
        {
            GameObject btnObj = new GameObject("DynamicButton");
            btnObj.transform.SetParent(parent, false);

            var rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 42);

            var img = btnObj.AddComponent<Image>();
            img.color = bgColor;

            var button = btnObj.AddComponent<Button>();
            ColorBlock cb = button.colors;
            cb.highlightedColor = new Color(bgColor.r + 0.12f, bgColor.g + 0.12f, bgColor.b + 0.12f, 1f);
            cb.pressedColor = new Color(bgColor.r - 0.08f, bgColor.g - 0.08f, bgColor.b - 0.08f, 1f);
            button.colors = cb;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(12, 4);
            textRect.offsetMax = new Vector2(-12, -4);

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 18;
            tmp.alignment = alignLeft ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.raycastTarget = false;

            return btnObj;
        }
    }
}
