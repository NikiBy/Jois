using UnityEngine;
using Jois.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Jois.Level
{
    public class HouseTrigger : MonoBehaviour
    {
        [Header("Lesson Data Asset")]
        [SerializeField] private LessonData lessonData;

        [Header("Fallback Data (если Asset не назначен)")]
        [SerializeField] private int lessonNumber = 1;
        [SerializeField] private string lessonTitle = "Урок 1: Введение в C++";
        [TextArea(4, 10)]
        [SerializeField] private string lessonTheory = 
            "Добро пожаловать в мир C++!\n\n" +
            "Любая программа на C++ начинается с функции main():\n" +
            "int main() {\n" +
            "    std::cout << \"Hello, World!\";\n" +
            "    return 0;\n" +
            "}\n\n" +
            "• std::cout — вывод текста на экран.\n" +
            "• return 0 — сообщает системе об успешном завершении программы.";

        [Header("Visual Indicators")]
        [SerializeField] private GameObject activeBeacon;
        [SerializeField] private GameObject lockVisual;

        [Header("Settings")]
        [SerializeField] private string promptMessage = "Нажмите [E], чтобы войти в Урок";

        private bool isPlayerNearby = false;

        public int LessonNumber => lessonData != null ? lessonData.LessonNumber : lessonNumber;
        public LessonData CurrentLessonData => lessonData;

        public void SetLessonData(LessonData data)
        {
            lessonData = data;
            UpdateVisuals();
        }

        private void Start()
        {
            if (ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.OnLevelUnlocked += HandleProgressionChanged;
            }
            UpdateVisuals();
        }

        private void OnDestroy()
        {
            if (ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.OnLevelUnlocked -= HandleProgressionChanged;
            }
        }

        private void HandleProgressionChanged(int unlockedLevel)
        {
            UpdateVisuals();
        }

        public bool IsUnlocked()
        {
            if (ProgressionManager.Instance == null) return true;
            return ProgressionManager.Instance.IsLevelUnlocked(LessonNumber);
        }

        public void UpdateVisuals()
        {
            bool unlocked = IsUnlocked();
            int currentTarget = ProgressionManager.Instance != null ? ProgressionManager.Instance.CurrentUnlockedLevel : 1;

            if (activeBeacon != null)
            {
                // Active beacon shines if this is the exact current lesson to solve
                activeBeacon.SetActive(unlocked && LessonNumber == currentTarget);
            }

            if (lockVisual != null)
            {
                // Lock visual is active if the level is not yet unlocked
                lockVisual.SetActive(!unlocked);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = true;
                if (LessonUI.Instance != null && !LessonUI.Instance.IsWindowOpen)
                {
                    if (IsUnlocked())
                    {
                        string title = lessonData != null ? lessonData.LessonTitle : lessonTitle;
                        LessonUI.Instance.ShowPrompt($"Нажмите [E], чтобы открыть «{title}»");
                    }
                    else
                    {
                        LessonUI.Instance.ShowPrompt($"🔒 Этот уровень закрыт! Сначала пройдите Урок {LessonNumber - 1}");
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = false;
                if (LessonUI.Instance != null)
                {
                    LessonUI.Instance.HidePrompt();
                }
            }
        }

        private void Update()
        {
            if (isPlayerNearby && LessonUI.Instance != null && !LessonUI.Instance.IsWindowOpen)
            {
                if (IsInteractPressed())
                {
                    if (IsUnlocked())
                    {
                        if (lessonData != null)
                        {
                            LessonUI.Instance.OpenLesson(lessonData);
                        }
                        else
                        {
                            LessonUI.Instance.OpenLesson(lessonTitle, lessonTheory);
                        }
                    }
                    else
                    {
                        LessonUI.Instance.ShowPrompt($"🔒 Этот уровень закрыт! Сначала пройдите Урок {LessonNumber - 1}");
                    }
                }
            }
        }

        private bool IsInteractPressed()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return Keyboard.current.eKey.wasPressedThisFrame;
            }
            return false;
#else
            return Input.GetKeyDown(KeyCode.E);
#endif
        }
    }
}
