using UnityEngine;

namespace Jois.Level
{
    public enum LessonType
    {
        Quiz,
        FindBug
    }

    [CreateAssetMenu(fileName = "NewLessonData", menuName = "Jois/Lesson Data", order = 1)]
    public class LessonData : ScriptableObject
    {
        [Header("Общая информация")]
        [SerializeField] private int lessonNumber = 1;
        [SerializeField] private string lessonTitle = "Урок 1: Введение";
        [TextArea(4, 12)]
        [SerializeField] private string theoryText = "Теоретический материал...";

        [Header("Тип испытания")]
        [SerializeField] private LessonType lessonType = LessonType.Quiz;

        [Header("Настройки Quiz")]
        [TextArea(2, 4)]
        [SerializeField] private string quizQuestion = "Вопрос?";
        [SerializeField] private string[] quizAnswers = new string[3] { "Вариант 1", "Вариант 2", "Вариант 3" };
        [Tooltip("Индекс правильного ответа (0-based)")]
        [SerializeField] private int correctQuizIndex = 0;
        [TextArea(2, 4)]
        [SerializeField] private string quizExplanation = "Пояснение, почему этот ответ верный.";

        [Header("Настройки FindBug")]
        [TextArea(1, 3)]
        [SerializeField] private string bugTaskDescription = "Найдите строку с синтаксической или логической ошибкой:";
        [TextArea(6, 16)]
        [SerializeField] private string[] codeLines = new string[0];
        [Tooltip("Номер строки с ошибкой (0-based индекс)")]
        [SerializeField] private int bugLineIndex = 0;
        [TextArea(2, 4)]
        [SerializeField] private string bugExplanation = "В этой строке допущена ошибка: ...";
        [TextArea(2, 4)]
        [SerializeField] private string generalHint = "Подсказка: обратите внимание на типы данных / операторы.";

        // Public Getters
        public int LessonNumber => lessonNumber;
        public string LessonTitle => lessonTitle;
        public string TheoryText => theoryText;
        public LessonType LessonType => lessonType;

        public string QuizQuestion => quizQuestion;
        public string[] QuizAnswers => quizAnswers;
        public int CorrectQuizIndex => correctQuizIndex;
        public string QuizExplanation => quizExplanation;

        public string BugTaskDescription => bugTaskDescription;
        public string[] CodeLines => codeLines;
        public int BugLineIndex => bugLineIndex;
        public string BugExplanation => bugExplanation;
        public string GeneralHint => generalHint;
    }
}
