using UnityEngine;
using Jois.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Jois.Level
{
    public class HouseTrigger : MonoBehaviour
    {
        [Header("Lesson Data")]
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

        [Header("Settings")]
        [SerializeField] private string promptMessage = "Нажмите [E], чтобы войти в Урок";

        private bool isPlayerNearby = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = true;
                if (LessonUI.Instance != null && !LessonUI.Instance.IsWindowOpen)
                {
                    LessonUI.Instance.ShowPrompt(promptMessage);
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
                    LessonUI.Instance.OpenLesson(lessonTitle, lessonTheory);
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
