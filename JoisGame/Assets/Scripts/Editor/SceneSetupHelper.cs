#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Jois.UI;
using Jois.Level;

namespace Jois.Editor
{
    public static class SceneSetupHelper
    {
        [MenuItem("Tools/Jois/1. Создать или обновить Canvas (UI Урока + HUD)", false, 1)]
        [MenuItem("Window/Jois/1. Создать или обновить Canvas (UI Урока + HUD)", false, 1)]
        public static void CreateLessonUI()
        {
            // 1. Ensure ProgressionManager in scene
            var progManager = Object.FindAnyObjectByType<ProgressionManager>();
            if (progManager == null)
            {
                var mgrObj = new GameObject("ProgressionManager");
                progManager = mgrObj.AddComponent<ProgressionManager>();
                Undo.RegisterCreatedObjectUndo(mgrObj, "Create ProgressionManager");
            }

            // 2. Find or create Canvas
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            GameObject canvasObj;

            if (canvas == null)
            {
                canvasObj = new GameObject("LessonCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
                canvasObj.AddComponent<GraphicRaycaster>();
                Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            }
            else
            {
                canvasObj = canvas.gameObject;
            }

            // 3. Ensure EventSystem with Input System UI module
            var eventSystem = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                var eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
#if ENABLE_INPUT_SYSTEM
                eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif
                Undo.RegisterCreatedObjectUndo(eventSystemObj, "Create EventSystem");
            }

            // 4. Add or get LessonUI component
            var lessonUI = canvasObj.GetComponent<LessonUI>();
            if (lessonUI == null)
            {
                lessonUI = canvasObj.AddComponent<LessonUI>();
            }

            // 5. Prompt Panel (Bottom-center hint)
            Transform existingPrompt = canvasObj.transform.Find("PromptPanel");
            if (existingPrompt != null) Object.DestroyImmediate(existingPrompt.gameObject);

            GameObject promptObj = new GameObject("PromptPanel");
            promptObj.transform.SetParent(canvasObj.transform, false);
            var promptRect = promptObj.AddComponent<RectTransform>();
            promptRect.anchorMin = new Vector2(0.5f, 0.12f);
            promptRect.anchorMax = new Vector2(0.5f, 0.12f);
            promptRect.pivot = new Vector2(0.5f, 0.5f);
            promptRect.sizeDelta = new Vector2(550, 50);

            var promptBg = promptObj.AddComponent<Image>();
            promptBg.color = new Color(0.08f, 0.1f, 0.15f, 0.9f);

            GameObject promptTextObj = new GameObject("Text");
            promptTextObj.transform.SetParent(promptObj.transform, false);
            var promptTextRect = promptTextObj.AddComponent<RectTransform>();
            promptTextRect.anchorMin = Vector2.zero;
            promptTextRect.anchorMax = Vector2.one;
            promptTextRect.sizeDelta = Vector2.zero;
            var promptTmp = promptTextObj.AddComponent<TextMeshProUGUI>();
            promptTmp.text = "Нажмите [E], чтобы открыть урок";
            promptTmp.fontSize = 20;
            promptTmp.alignment = TextAlignmentOptions.Center;
            promptTmp.color = Color.white;
            promptTmp.raycastTarget = false;

            // 6. HUD Panel (Top-left)
            Transform existingHUD = canvasObj.transform.Find("HUDPanel");
            if (existingHUD != null) Object.DestroyImmediate(existingHUD.gameObject);

            GameObject hudObj = new GameObject("HUDPanel");
            hudObj.transform.SetParent(canvasObj.transform, false);
            var hudRect = hudObj.AddComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0.02f, 0.92f);
            hudRect.anchorMax = new Vector2(0.25f, 0.97f);
            hudRect.pivot = new Vector2(0f, 1f);
            hudRect.sizeDelta = Vector2.zero;

            var hudBg = hudObj.AddComponent<Image>();
            hudBg.color = new Color(0.1f, 0.12f, 0.16f, 0.85f);

            GameObject hudTextObj = new GameObject("HUDText");
            hudTextObj.transform.SetParent(hudObj.transform, false);
            var hudTextRect = hudTextObj.AddComponent<RectTransform>();
            hudTextRect.anchorMin = Vector2.zero;
            hudTextRect.anchorMax = Vector2.one;
            hudTextRect.offsetMin = new Vector2(15, 0);
            hudTextRect.offsetMax = new Vector2(-15, 0);
            var hudTmp = hudTextObj.AddComponent<TextMeshProUGUI>();
            hudTmp.text = "Прогресс: 1 / 10 уровней";
            hudTmp.fontSize = 18;
            hudTmp.fontStyle = FontStyles.Bold;
            hudTmp.alignment = TextAlignmentOptions.MidlineLeft;
            hudTmp.color = new Color(0.4f, 0.85f, 1f, 1f);
            hudTmp.raycastTarget = false;

            // 7. Modal Window Panel (2-column layout)
            Transform existingWindow = canvasObj.transform.Find("LessonModalWindow");
            if (existingWindow != null) Object.DestroyImmediate(existingWindow.gameObject);

            GameObject windowObj = new GameObject("LessonModalWindow");
            windowObj.transform.SetParent(canvasObj.transform, false);
            var winRect = windowObj.AddComponent<RectTransform>();
            winRect.anchorMin = new Vector2(0.5f, 0.5f);
            winRect.anchorMax = new Vector2(0.5f, 0.5f);
            winRect.pivot = new Vector2(0.5f, 0.5f);
            winRect.sizeDelta = new Vector2(1080, 680);

            var winBg = windowObj.AddComponent<Image>();
            winBg.color = new Color(0.1f, 0.12f, 0.16f, 0.98f);

            // Title
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(windowObj.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.04f, 0.90f);
            titleRect.anchorMax = new Vector2(0.96f, 0.98f);
            titleRect.sizeDelta = Vector2.zero;
            var titleTmp = titleObj.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "Урок 1: Введение в C++";
            titleTmp.fontSize = 28;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.alignment = TextAlignmentOptions.MidlineLeft;
            titleTmp.color = new Color(0.35f, 0.78f, 1f, 1f);

            // Left Column (Theory Card)
            GameObject theoryCard = new GameObject("TheoryCard");
            theoryCard.transform.SetParent(windowObj.transform, false);
            var theoryCardRect = theoryCard.AddComponent<RectTransform>();
            theoryCardRect.anchorMin = new Vector2(0.04f, 0.14f);
            theoryCardRect.anchorMax = new Vector2(0.48f, 0.88f);
            theoryCardRect.sizeDelta = Vector2.zero;

            var theoryCardBg = theoryCard.AddComponent<Image>();
            theoryCardBg.color = new Color(0.14f, 0.16f, 0.22f, 0.8f);

            GameObject theoryTextObj = new GameObject("TheoryText");
            theoryTextObj.transform.SetParent(theoryCard.transform, false);
            var theoryTextRect = theoryTextObj.AddComponent<RectTransform>();
            theoryTextRect.anchorMin = Vector2.zero;
            theoryTextRect.anchorMax = Vector2.one;
            theoryTextRect.offsetMin = new Vector2(16, 16);
            theoryTextRect.offsetMax = new Vector2(-16, -16);
            var theoryTmp = theoryTextObj.AddComponent<TextMeshProUGUI>();
            theoryTmp.text = "Теоретический материал урока...";
            theoryTmp.fontSize = 17;
            theoryTmp.alignment = TextAlignmentOptions.TopLeft;
            theoryTmp.color = new Color(0.9f, 0.92f, 0.96f, 1f);

            // Right Column (Challenge Area)
            GameObject taskCard = new GameObject("TaskCard");
            taskCard.transform.SetParent(windowObj.transform, false);
            var taskCardRect = taskCard.AddComponent<RectTransform>();
            taskCardRect.anchorMin = new Vector2(0.52f, 0.26f);
            taskCardRect.anchorMax = new Vector2(0.96f, 0.88f);
            taskCardRect.sizeDelta = Vector2.zero;

            var taskCardBg = taskCard.AddComponent<Image>();
            taskCardBg.color = new Color(0.13f, 0.15f, 0.2f, 0.8f);

            // 7.1 Quiz Container
            GameObject quizCont = new GameObject("QuizContainer");
            quizCont.transform.SetParent(taskCard.transform, false);
            var quizRect = quizCont.AddComponent<RectTransform>();
            quizRect.anchorMin = Vector2.zero;
            quizRect.anchorMax = Vector2.one;
            quizRect.offsetMin = new Vector2(16, 16);
            quizRect.offsetMax = new Vector2(-16, -16);

            GameObject quizQObj = new GameObject("QuestionText");
            quizQObj.transform.SetParent(quizCont.transform, false);
            var quizQRect = quizQObj.AddComponent<RectTransform>();
            quizQRect.anchorMin = new Vector2(0f, 0.65f);
            quizQRect.anchorMax = new Vector2(1f, 1f);
            quizQRect.sizeDelta = Vector2.zero;
            var quizQTmp = quizQObj.AddComponent<TextMeshProUGUI>();
            quizQTmp.text = "Вопрос испытания?";
            quizQTmp.fontSize = 20;
            quizQTmp.fontStyle = FontStyles.Bold;
            quizQTmp.alignment = TextAlignmentOptions.TopLeft;
            quizQTmp.color = Color.white;

            GameObject quizOpts = new GameObject("OptionsParent");
            quizOpts.transform.SetParent(quizCont.transform, false);
            var quizOptsRect = quizOpts.AddComponent<RectTransform>();
            quizOptsRect.anchorMin = new Vector2(0f, 0f);
            quizOptsRect.anchorMax = new Vector2(1f, 0.62f);
            quizOptsRect.sizeDelta = Vector2.zero;
            var quizLayout = quizOpts.AddComponent<VerticalLayoutGroup>();
            quizLayout.spacing = 10;
            quizLayout.childControlHeight = true;
            quizLayout.childControlWidth = true;
            quizLayout.childForceExpandHeight = false;

            // 7.2 Bug Container
            GameObject bugCont = new GameObject("BugContainer");
            bugCont.transform.SetParent(taskCard.transform, false);
            var bugRect = bugCont.AddComponent<RectTransform>();
            bugRect.anchorMin = Vector2.zero;
            bugRect.anchorMax = Vector2.one;
            bugRect.offsetMin = new Vector2(16, 16);
            bugRect.offsetMax = new Vector2(-16, -16);

            GameObject bugTObj = new GameObject("BugTaskText");
            bugTObj.transform.SetParent(bugCont.transform, false);
            var bugTRect = bugTObj.AddComponent<RectTransform>();
            bugTRect.anchorMin = new Vector2(0f, 0.82f);
            bugTRect.anchorMax = new Vector2(1f, 1f);
            bugTRect.sizeDelta = Vector2.zero;
            var bugTTmp = bugTObj.AddComponent<TextMeshProUGUI>();
            bugTTmp.text = "Найдите строку с ошибкой в коде ниже:";
            bugTTmp.fontSize = 18;
            bugTTmp.fontStyle = FontStyles.Bold;
            bugTTmp.alignment = TextAlignmentOptions.TopLeft;
            bugTTmp.color = Color.white;

            GameObject bugLines = new GameObject("BugLinesParent");
            bugLines.transform.SetParent(bugCont.transform, false);
            var bugLinesRect = bugLines.AddComponent<RectTransform>();
            bugLinesRect.anchorMin = new Vector2(0f, 0f);
            bugLinesRect.anchorMax = new Vector2(1f, 0.80f);
            bugLinesRect.sizeDelta = Vector2.zero;
            var bugLayout = bugLines.AddComponent<VerticalLayoutGroup>();
            bugLayout.spacing = 4;
            bugLayout.childControlHeight = false;
            bugLayout.childControlWidth = true;
            bugLayout.childForceExpandHeight = false;

            // 7.3 Feedback Panel
            GameObject feedbackObj = new GameObject("FeedbackPanel");
            feedbackObj.transform.SetParent(windowObj.transform, false);
            var feedbackRect = feedbackObj.AddComponent<RectTransform>();
            feedbackRect.anchorMin = new Vector2(0.52f, 0.14f);
            feedbackRect.anchorMax = new Vector2(0.96f, 0.24f);
            feedbackRect.sizeDelta = Vector2.zero;

            var feedbackBg = feedbackObj.AddComponent<Image>();
            feedbackBg.color = new Color(0.12f, 0.45f, 0.2f, 0.95f);

            GameObject feedbackTextObj = new GameObject("FeedbackText");
            feedbackTextObj.transform.SetParent(feedbackObj.transform, false);
            var fbTextRect = feedbackTextObj.AddComponent<RectTransform>();
            fbTextRect.anchorMin = Vector2.zero;
            fbTextRect.anchorMax = Vector2.one;
            fbTextRect.offsetMin = new Vector2(12, 6);
            fbTextRect.offsetMax = new Vector2(-12, -6);
            var fbTmp = feedbackTextObj.AddComponent<TextMeshProUGUI>();
            fbTmp.text = "Пояснение ответа...";
            fbTmp.fontSize = 16;
            fbTmp.alignment = TextAlignmentOptions.MidlineLeft;
            fbTmp.color = Color.white;

            // 7.4 Action Buttons (Bottom Bar)
            // Close Button
            GameObject closeBtnObj = new GameObject("CloseButton");
            closeBtnObj.transform.SetParent(windowObj.transform, false);
            var closeBtnRect = closeBtnObj.AddComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(0.04f, 0.04f);
            closeBtnRect.anchorMax = new Vector2(0.24f, 0.11f);
            closeBtnRect.sizeDelta = Vector2.zero;

            var closeBtnImg = closeBtnObj.AddComponent<Image>();
            closeBtnImg.color = new Color(0.25f, 0.28f, 0.35f, 1f);
            var closeBtn = closeBtnObj.AddComponent<Button>();

            GameObject closeTextObj = new GameObject("Text");
            closeTextObj.transform.SetParent(closeBtnObj.transform, false);
            var closeTextRect = closeTextObj.AddComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            var closeTmp = closeTextObj.AddComponent<TextMeshProUGUI>();
            closeTmp.text = "Закрыть (Esc)";
            closeTmp.fontSize = 18;
            closeTmp.alignment = TextAlignmentOptions.Center;
            closeTmp.color = Color.white;
            closeTmp.raycastTarget = false;

            // Complete Lesson Button
            GameObject compBtnObj = new GameObject("CompleteLessonButton");
            compBtnObj.transform.SetParent(windowObj.transform, false);
            var compBtnRect = compBtnObj.AddComponent<RectTransform>();
            compBtnRect.anchorMin = new Vector2(0.68f, 0.04f);
            compBtnRect.anchorMax = new Vector2(0.96f, 0.11f);
            compBtnRect.sizeDelta = Vector2.zero;

            var compBtnImg = compBtnObj.AddComponent<Image>();
            compBtnImg.color = new Color(0.15f, 0.65f, 0.35f, 1f);
            var compBtn = compBtnObj.AddComponent<Button>();

            GameObject compTextObj = new GameObject("Text");
            compTextObj.transform.SetParent(compBtnObj.transform, false);
            var compTextRect = compTextObj.AddComponent<RectTransform>();
            compTextRect.anchorMin = Vector2.zero;
            compTextRect.anchorMax = Vector2.one;
            var compTmp = compTextObj.AddComponent<TextMeshProUGUI>();
            compTmp.text = "Завершить урок ✔";
            compTmp.fontSize = 18;
            compTmp.fontStyle = FontStyles.Bold;
            compTmp.alignment = TextAlignmentOptions.Center;
            compTmp.color = Color.white;
            compTmp.raycastTarget = false;

            // Bind Serialized References
            SerializedObject so = new SerializedObject(lessonUI);
            so.FindProperty("windowPanel").objectReferenceValue = windowObj;
            so.FindProperty("promptPanel").objectReferenceValue = promptObj;
            so.FindProperty("quizContainer").objectReferenceValue = quizCont;
            so.FindProperty("bugContainer").objectReferenceValue = bugCont;
            so.FindProperty("feedbackPanel").objectReferenceValue = feedbackObj;

            so.FindProperty("titleText").objectReferenceValue = titleTmp;
            so.FindProperty("theoryText").objectReferenceValue = theoryTmp;
            so.FindProperty("promptText").objectReferenceValue = promptTmp;
            so.FindProperty("quizQuestionText").objectReferenceValue = quizQTmp;
            so.FindProperty("bugTaskText").objectReferenceValue = bugTTmp;
            so.FindProperty("feedbackText").objectReferenceValue = fbTmp;

            so.FindProperty("quizOptionsParent").objectReferenceValue = quizOpts.transform;
            so.FindProperty("bugLinesParent").objectReferenceValue = bugLines.transform;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.FindProperty("completeLessonButton").objectReferenceValue = compBtn;
            so.FindProperty("feedbackBackground").objectReferenceValue = feedbackBg;
            so.FindProperty("hudProgressText").objectReferenceValue = hudTmp;
            so.ApplyModifiedProperties();

            // Set default active states
            windowObj.SetActive(false);
            promptObj.SetActive(false);
            feedbackObj.SetActive(false);

            Selection.activeGameObject = canvasObj;
            EditorUtility.SetDirty(canvasObj);

            Debug.Log("🎉 [Jois] Canvas с UI Урока и HUD успешно создан и сконфигурирован!");
        }

        [MenuItem("Tools/Jois/2. Создать деревню из 10 домиков (Village Setup)", false, 2)]
        [MenuItem("Window/Jois/2. Создать деревню из 10 домиков (Village Setup)", false, 2)]
        public static void CreateVillageOfHouses()
        {
            // Find or create parent
            GameObject villageParent = GameObject.Find("Village_Houses");
            if (villageParent != null)
            {
                Undo.DestroyObjectImmediate(villageParent);
            }
            villageParent = new GameObject("Village_Houses");
            Undo.RegisterCreatedObjectUndo(villageParent, "Create Village Houses");

            // Setup colors for visual variety
            Color[] roofColors = new Color[]
            {
                new Color(0.2f, 0.5f, 0.8f), // Blue
                new Color(0.8f, 0.4f, 0.2f), // Orange
                new Color(0.3f, 0.7f, 0.4f), // Green
                new Color(0.7f, 0.2f, 0.5f), // Purple
                new Color(0.9f, 0.7f, 0.2f), // Yellow
                new Color(0.2f, 0.7f, 0.7f), // Cyan
                new Color(0.8f, 0.3f, 0.3f), // Red
                new Color(0.5f, 0.4f, 0.7f), // Indigo
                new Color(0.4f, 0.6f, 0.3f), // Lime
                new Color(0.85f, 0.85f, 0.3f) // Gold Final
            };

            for (int i = 0; i < 10; i++)
            {
                int levelNum = i + 1;

                // Arrange houses in a semi-circular pleasant village street
                float angle = (i - 4.5f) * 16f * Mathf.Deg2Rad;
                float radius = 32f;
                float x = Mathf.Sin(angle) * radius;
                float z = Mathf.Cos(angle) * radius - 15f;

                GameObject house = GameObject.CreatePrimitive(PrimitiveType.Cube);
                house.name = $"House_Level_{levelNum}";
                house.transform.SetParent(villageParent.transform, false);
                house.transform.position = new Vector3(x, 1.75f, z);
                house.transform.localScale = new Vector3(4f, 3.5f, 4f);
                house.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 0) - house.transform.position);

                // Roof
                GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                roof.name = "Roof";
                roof.transform.SetParent(house.transform, false);
                roof.transform.localPosition = new Vector3(0f, 0.65f, 0f);
                roof.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                roof.transform.localScale = new Vector3(0.85f, 0.6f, 0.85f);

                // Entrance Trigger
                GameObject triggerObj = new GameObject("EntranceTrigger");
                triggerObj.transform.SetParent(house.transform, false);
                triggerObj.transform.localPosition = new Vector3(0f, -0.2f, -0.65f);
                triggerObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                var boxCol = triggerObj.AddComponent<BoxCollider>();
                boxCol.isTrigger = true;

                var trigger = triggerObj.AddComponent<HouseTrigger>();

                // Active Beacon (Visual marker above roof)
                GameObject beacon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                beacon.name = "ActiveBeacon";
                beacon.transform.SetParent(house.transform, false);
                beacon.transform.localPosition = new Vector3(0f, 1.8f, 0f);
                beacon.transform.localScale = new Vector3(0.2f, 0.8f, 0.2f);
                var beaconCol = beacon.GetComponent<Collider>();
                if (beaconCol != null) Object.DestroyImmediate(beaconCol);

                // Lock Visual (Red lock indicator)
                GameObject lockVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lockVis.name = "LockVisual";
                lockVis.transform.SetParent(house.transform, false);
                lockVis.transform.localPosition = new Vector3(0f, 0.2f, -0.55f);
                lockVis.transform.localScale = new Vector3(0.3f, 0.3f, 0.15f);
                var lockCol = lockVis.GetComponent<Collider>();
                if (lockCol != null) Object.DestroyImmediate(lockCol);

                // Serialized link in HouseTrigger
                SerializedObject so = new SerializedObject(trigger);
                so.FindProperty("lessonNumber").intValue = levelNum;
                so.FindProperty("lessonTitle").stringValue = $"Урок {levelNum}";
                so.FindProperty("activeBeacon").objectReferenceValue = beacon;
                so.FindProperty("lockVisual").objectReferenceValue = lockVis;

                // Try to auto-link LessonData asset if already created
                string assetPath = $"Assets/Data/Lessons/LessonData_Level{levelNum}.asset";
                var lessonAsset = AssetDatabase.LoadAssetAtPath<LessonData>(assetPath);
                if (lessonAsset != null)
                {
                    so.FindProperty("lessonData").objectReferenceValue = lessonAsset;
                }

                so.ApplyModifiedProperties();
            }

            Debug.Log("🎉 [Jois] 10 домиков-уровней успешно размещены на карте!");
        }

        [MenuItem("Tools/Jois/3. Сгенерировать все 10 ассетов уроков C++ (ScriptableObjects)", false, 3)]
        [MenuItem("Window/Jois/3. Сгенерировать все 10 ассетов уроков C++ (ScriptableObjects)", false, 3)]
        public static void GenerateAllLessonAssets()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
            {
                AssetDatabase.CreateFolder("Assets", "Data");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Data/Lessons"))
            {
                AssetDatabase.CreateFolder("Assets/Data", "Lessons");
            }

            // Level 1: Quiz
            CreateOrUpdateLesson(1, "Урок 1: Врата C++",
                "Любая исполняемая программа на C++ обязана содержать точку входа — функцию main().\n\n" +
                "Директива #include <iostream> подключает заголовочный файл для работы со стандартными потоками вывода (std::cout) и ввода (std::cin).\n\n" +
                "Инструкция return 0; сигнализирует операционной системе, что программа завершилась успешно.",
                LessonType.Quiz,
                "С какой функции операционная система всегда начинает выполнение программы на C++?",
                new string[] { "start()", "int main()", "program()" },
                1,
                "Стандарт языка C++ строго определяет функцию int main() как обязательную точку входа в программу.",
                "", new string[0], 0, "", "");

            // Level 2: FindBug
            CreateOrUpdateLesson(2, "Урок 2: Лавка типов",
                "Язык C++ — язык со статической строгой типизацией. Переменная каждого типа может хранить только совместимые с ней значения:\n\n" +
                "• int — целые числа (42, -5)\n" +
                "• double / float — числа с плавающей точкой (3.14)\n" +
                "• char — одиночный символ в одинарных кавычках ('A')\n" +
                "• bool — логическое значение (true / false)\n\n" +
                "Двойные кавычки (\"...\") задают строковый литерал, а не одиночный символ char!",
                LessonType.FindBug,
                "", new string[0], 0, "",
                "Найдите строку с некорректным присваиванием типа данных:",
                new string[] {
                    "1: #include <iostream>",
                    "2: int main() {",
                    "3:     int age = 19;",
                    "4:     double gpa = 4.75;",
                    "5:     char group = \"A\";",
                    "6:     bool isStudent = true;",
                    "7:     return 0;",
                    "8: }"
                },
                4, // 0-based index corresponding to line 5
                "Для типа char используются одинарные кавычки ('A'). Двойные кавычки задают строку!",
                "Обратите внимание на типы кавычек для переменной символьного типа char.");

            // Level 3: Quiz
            CreateOrUpdateLesson(3, "Урок 3: Почта ввода-вывода",
                "Для вывода данных на экран используется поток std::cout в паре с оператором вставки << (стрелки направлены к экрану).\n\n" +
                "Для считывания данных с клавиатуры используется поток std::cin с оператором извлечения >> (стрелки направлены в переменную).\n\n" +
                "Пример: std::cout << x; // Вывод x\nstd::cin >> x; // Ввод x",
                LessonType.Quiz,
                "Какой оператор используется для передачи значения переменной в поток вывода std::cout?",
                new string[] { ">>", "<<", "->" },
                1,
                "Оператор << (stream insertion) направляет поток данных в std::cout.",
                "", new string[0], 0, "", "");

            // Level 4: FindBug
            CreateOrUpdateLesson(4, "Урок 4: Развилка решений",
                "В языке C++ оператор = — это оператор присваивания, а оператор == — оператор проверки на равенство.\n\n" +
                "Написание if (x = 5) не сравнивает x с 5, а записывает 5 в x, возвращая true.\n" +
                "Всегда используйте == для проверки равенства двух значений!",
                LessonType.FindBug,
                "", new string[0], 0, "",
                "Найдите логическую ошибку в проверке условия:",
                new string[] {
                    "1: #include <iostream>",
                    "2: int main() {",
                    "3:     int score = 100;",
                    "4:     if (score = 100) {",
                    "5:         std::cout << \"Победа!\";",
                    "6:     }",
                    "7:     return 0;",
                    "8: }"
                },
                3, // line 4
                "В условии if случайно использовано присваивание (=) вместо проверки на равенство (==).",
                "Проверьте, действительно ли происходит сравнение или же это присваивание.");

            // Level 5: Quiz
            CreateOrUpdateLesson(5, "Урок 5: Мельница циклов",
                "Цикл for состоит из трёх секций:\nfor (инициализация; условие; шаг) { тело_цикла; }\n\n" +
                "Если счетчик начинает с 0 и цикл идет пока i < 5, то i последовательно примет значения: 0, 1, 2, 3, 4.\n" +
                "При i = 5 условие 5 < 5 станет ложным, и цикл остановится.",
                LessonType.Quiz,
                "Сколько раз выполнится тело цикла for (int i = 0; i < 5; i++)?",
                new string[] { "4 раза", "5 раз", "6 раз" },
                1,
                "Цикл выполнится ровно 5 раз: для значений i = 0, 1, 2, 3, 4.",
                "", new string[0], 0, "", "");

            // Level 6: FindBug
            CreateOrUpdateLesson(6, "Урок 6: Склад массивов",
                "Массивы в C++ индексируются строго с нуля (0).\n\n" +
                "Массив размера N (например, int numbers[5]) содержит элементы с индексами от 0 до N - 1 (то есть 0, 1, 2, 3, 4).\n\n" +
                "Обращение к numbers[5] является выходом за границы массива!",
                LessonType.FindBug,
                "", new string[0], 0, "",
                "Найдите строку с выходом за пределы массива:",
                new string[] {
                    "1: #include <iostream>",
                    "2: int main() {",
                    "3:     int numbers[5] = {10, 20, 30, 40, 50};",
                    "4:     std::cout << numbers[0] << std::endl;",
                    "5:     std::cout << numbers[5] << std::endl;",
                    "6:     return 0;",
                    "7: }"
                },
                4, // line 5
                "В массиве из 5 элементов допустимы только индексы 0..4. Обращение к numbers[5] ошибочно!",
                "Помните: если массив размера 5, максимальный допустимый индекс равен 4.");

            // Level 7: Quiz
            CreateOrUpdateLesson(7, "Урок 7: Библиотека строк",
                "В C++ класс std::string из заголовочного файла <string> представляет удобную работу с текстом.\n\n" +
                "Для получения длины строки (количества символов) используются методы .length() или .size().\n\n" +
                "Строки можно складывать через оператор +, выполняя конкатенацию.",
                LessonType.Quiz,
                "Какой метод класса std::string возвращает количество символов в строке?",
                new string[] { ".count()", ".length()", ".sizeof()" },
                1,
                "Методы .length() и .size() в std::string являются синонимами и возвращают текущую длину строки.",
                "", new string[0], 0, "", "");

            // Level 8: FindBug
            CreateOrUpdateLesson(8, "Урок 8: Мастерская функций",
                "Если функция объявлена с возвращаемым типом, отличным от void (например, int, double, bool), она обязана вернуть значение соответствующего типа через оператор return.\n\n" +
                "Отсутствие return в не-void функции приводит к непредсказуемым ошибкам!",
                LessonType.FindBug,
                "", new string[0], 0, "",
                "Найдите ошибку в теле функции:",
                new string[] {
                    "1: #include <iostream>",
                    "2: int multiply(int a, int b) {",
                    "3:     int result = a * b;",
                    "4: }",
                    "5: int main() {",
                    "6:     int val = multiply(4, 5);",
                    "7:     return 0;",
                    "8: }"
                },
                2, // line 3/4 missing return
                "Функция multiply имеет тип возврата int, но в ее теле отсутствует оператор return result;!",
                "Проверьте, возвращает ли функция multiply посчитанное значение.");

            // Level 9: Quiz
            CreateOrUpdateLesson(9, "Урок 9: Башня Памяти",
                "Указатель (например, int* ptr) — это переменная, хранящая адрес ячейки памяти другого объекта.\n\n" +
                "• Оператор & (взятие адреса): ptr = &x;\n" +
                "• Оператор * (разыменование): *ptr возвращает значение переменной x, хранящейся по этому адресу.",
                LessonType.Quiz,
                "Что возвращает операция разыменования *ptr, если ptr хранит адрес переменной x = 42?",
                new string[] { "Адрес памяти переменной x", "Значение 42", "null" },
                1,
                "Операция разыменования *ptr переходит по адресу памяти и возвращает само значение переменной (42).",
                "", new string[0], 0, "", "");

            // Level 10: FindBug
            CreateOrUpdateLesson(10, "Урок 10: Финальный Экзамен",
                "Память, выделенная оператором new[] (для динамических массивов), обязана освобождаться соответствующим оператором delete[].\n\n" +
                "Использование одиночного delete вместо delete[] для массива приводит к утечкам памяти и повреждению кучи (Heap Corruption)!",
                LessonType.FindBug,
                "", new string[0], 0, "",
                "Найдите ошибку освобождения динамической памяти:",
                new string[] {
                    "1: #include <iostream>",
                    "2: void processData() {",
                    "3:     int* array = new int[100];",
                    "4:     array[0] = 42;",
                    "5:     delete array;",
                    "6: }",
                    "7: int main() {",
                    "8:     processData();",
                    "9:     return 0;",
                    "10: }"
                },
                4, // line 5
                "Массив был создан через new int[100], поэтому удаляться обязан через delete[] array;!",
                "Посмотрите, как выделялся массив (new[]) и как он удаляется.");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("🎉 [Jois] Все 10 ассетов уроков C++ успешно созданы в Assets/Data/Lessons/!");
        }

        private static void CreateOrUpdateLesson(int num, string title, string theory, LessonType type,
            string qQuestion, string[] qAnswers, int correctQ, string qExpl,
            string bugTask, string[] codeLines, int bugLine, string bugExpl, string hint)
        {
            string path = $"Assets/Data/Lessons/LessonData_Level{num}.asset";
            var lesson = AssetDatabase.LoadAssetAtPath<LessonData>(path);

            if (lesson == null)
            {
                lesson = ScriptableObject.CreateInstance<LessonData>();
                AssetDatabase.CreateAsset(lesson, path);
            }

            SerializedObject so = new SerializedObject(lesson);
            so.FindProperty("lessonNumber").intValue = num;
            so.FindProperty("lessonTitle").stringValue = title;
            so.FindProperty("theoryText").stringValue = theory;
            so.FindProperty("lessonType").enumValueIndex = (int)type;

            so.FindProperty("quizQuestion").stringValue = qQuestion;
            var answersProp = so.FindProperty("quizAnswers");
            answersProp.arraySize = qAnswers.Length;
            for (int i = 0; i < qAnswers.Length; i++)
            {
                answersProp.GetArrayElementAtIndex(i).stringValue = qAnswers[i];
            }
            so.FindProperty("correctQuizIndex").intValue = correctQ;
            so.FindProperty("quizExplanation").stringValue = qExpl;

            so.FindProperty("bugTaskDescription").stringValue = bugTask;
            var linesProp = so.FindProperty("codeLines");
            linesProp.arraySize = codeLines.Length;
            for (int i = 0; i < codeLines.Length; i++)
            {
                linesProp.GetArrayElementAtIndex(i).stringValue = codeLines[i];
            }
            so.FindProperty("bugLineIndex").intValue = bugLine;
            so.FindProperty("bugExplanation").stringValue = bugExpl;
            so.FindProperty("generalHint").stringValue = hint;

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(lesson);
        }
    }
}
#endif
