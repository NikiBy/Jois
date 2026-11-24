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
        [MenuItem("Tools/Jois/1. Создать красивый UI Урока (Canvas)", false, 1)]
        [MenuItem("Window/Jois/1. Создать красивый UI Урока (Canvas)", false, 1)]
        [MenuItem("GameObject/Jois/1. Создать красивый UI Урока (Canvas)", false, 10)]
        public static void CreateLessonUI()
        {
            // Find or create Canvas
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
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            else
            {
                canvasObj = canvas.gameObject;
            }

            // Ensure EventSystem with modern Input System module
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
            }

            // Add or get LessonUI component
            var lessonUI = canvasObj.GetComponent<LessonUI>();
            if (lessonUI == null)
            {
                lessonUI = canvasObj.AddComponent<LessonUI>();
            }

            // 1. Create Prompt Panel (bottom-center hint)
            Transform existingPrompt = canvasObj.transform.Find("PromptPanel");
            if (existingPrompt != null) Object.DestroyImmediate(existingPrompt.gameObject);

            GameObject promptObj = new GameObject("PromptPanel");
            promptObj.transform.SetParent(canvasObj.transform, false);
            var promptRect = promptObj.AddComponent<RectTransform>();
            promptRect.anchorMin = new Vector2(0.5f, 0.15f);
            promptRect.anchorMax = new Vector2(0.5f, 0.15f);
            promptRect.pivot = new Vector2(0.5f, 0.5f);
            promptRect.sizeDelta = new Vector2(450, 60);

            var promptBg = promptObj.AddComponent<Image>();
            promptBg.color = new Color(0.1f, 0.1f, 0.15f, 0.85f);

            GameObject promptTextObj = new GameObject("Text");
            promptTextObj.transform.SetParent(promptObj.transform, false);
            var promptTextRect = promptTextObj.AddComponent<RectTransform>();
            promptTextRect.anchorMin = Vector2.zero;
            promptTextRect.anchorMax = Vector2.one;
            promptTextRect.sizeDelta = Vector2.zero;
            var promptTmp = promptTextObj.AddComponent<TextMeshProUGUI>();
            promptTmp.text = "Нажмите [E], чтобы открыть урок";
            promptTmp.fontSize = 24;
            promptTmp.alignment = TextAlignmentOptions.Center;
            promptTmp.color = Color.white;

            // 2. Create Modal Window Panel
            Transform existingWindow = canvasObj.transform.Find("LessonModalWindow");
            if (existingWindow != null) Object.DestroyImmediate(existingWindow.gameObject);

            GameObject windowObj = new GameObject("LessonModalWindow");
            windowObj.transform.SetParent(canvasObj.transform, false);
            var winRect = windowObj.AddComponent<RectTransform>();
            winRect.anchorMin = new Vector2(0.5f, 0.5f);
            winRect.anchorMax = new Vector2(0.5f, 0.5f);
            winRect.pivot = new Vector2(0.5f, 0.5f);
            winRect.sizeDelta = new Vector2(900, 650);

            var winBg = windowObj.AddComponent<Image>();
            winBg.color = new Color(0.12f, 0.14f, 0.18f, 0.95f);

            // Title
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(windowObj.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.05f, 0.85f);
            titleRect.anchorMax = new Vector2(0.95f, 0.96f);
            titleRect.sizeDelta = Vector2.zero;
            var titleTmp = titleObj.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "Урок 1: Введение в C++";
            titleTmp.fontSize = 32;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = new Color(0.35f, 0.75f, 1f, 1f);

            // Theory content
            GameObject theoryObj = new GameObject("TheoryText");
            theoryObj.transform.SetParent(windowObj.transform, false);
            var theoryRect = theoryObj.AddComponent<RectTransform>();
            theoryRect.anchorMin = new Vector2(0.06f, 0.22f);
            theoryRect.anchorMax = new Vector2(0.94f, 0.82f);
            theoryRect.sizeDelta = Vector2.zero;
            var theoryTmp = theoryObj.AddComponent<TextMeshProUGUI>();
            theoryTmp.text = "Добро пожаловать в изучение C++!\nЗдесь будет отображаться текст урока.";
            theoryTmp.fontSize = 22;
            theoryTmp.alignment = TextAlignmentOptions.TopLeft;
            theoryTmp.color = new Color(0.9f, 0.9f, 0.9f, 1f);

            // Close Button
            GameObject btnObj = new GameObject("CloseButton");
            btnObj.transform.SetParent(windowObj.transform, false);
            var btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.08f);
            btnRect.anchorMax = new Vector2(0.5f, 0.08f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.sizeDelta = new Vector2(260, 55);

            var btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.6f, 0.9f, 1f);

            var button = btnObj.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(0.3f, 0.75f, 1f, 1f);
            colors.pressedColor = new Color(0.15f, 0.45f, 0.7f, 1f);
            button.colors = colors;

            GameObject btnTextObj = new GameObject("ButtonText");
            btnTextObj.transform.SetParent(btnObj.transform, false);
            var btnTextRect = btnTextObj.AddComponent<RectTransform>();
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            btnTextRect.sizeDelta = Vector2.zero;
            var btnTmp = btnTextObj.AddComponent<TextMeshProUGUI>();
            btnTmp.text = "Понятно (Закрыть)";
            btnTmp.fontSize = 22;
            btnTmp.fontStyle = FontStyles.Bold;
            btnTmp.alignment = TextAlignmentOptions.Center;
            btnTmp.color = Color.white;
            btnTmp.raycastTarget = false; // Never block button click

            // Connect OnClick to CloseLesson()
            UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, lessonUI.CloseLesson);

            // Link serialized fields on LessonUI
            SerializedObject so = new SerializedObject(lessonUI);
            so.FindProperty("windowPanel").objectReferenceValue = windowObj;
            so.FindProperty("promptPanel").objectReferenceValue = promptObj;
            so.FindProperty("titleText").objectReferenceValue = titleTmp;
            so.FindProperty("theoryText").objectReferenceValue = theoryTmp;
            so.FindProperty("promptText").objectReferenceValue = promptTmp;
            so.FindProperty("closeButton").objectReferenceValue = button;
            so.ApplyModifiedProperties();

            // Hide window by default, hide prompt by default
            windowObj.SetActive(false);
            promptObj.SetActive(false);

            Selection.activeGameObject = canvasObj;
            EditorUtility.SetDirty(canvasObj);

            Debug.Log("🎉 [Jois] Интерфейс урока успешно создан и настроен!");
        }

        [MenuItem("Tools/Jois/2. Создать первый тестовый Домик-Урок", false, 2)]
        [MenuItem("Window/Jois/2. Создать первый тестовый Домик-Урок", false, 2)]
        [MenuItem("GameObject/Jois/2. Создать первый тестовый Домик-Урок", false, 11)]
        public static void CreateTestHouse()
        {
            // Create root House
            GameObject house = GameObject.CreatePrimitive(PrimitiveType.Cube);
            house.name = "House_Lesson_1";
            house.transform.position = new Vector3(0f, 1.75f, 10f);
            house.transform.localScale = new Vector3(4f, 3.5f, 4f);

            // Create roof
            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            roof.name = "Roof";
            roof.transform.SetParent(house.transform, false);
            roof.transform.localPosition = new Vector3(0f, 0.7f, 0f);
            roof.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            roof.transform.localScale = new Vector3(0.8f, 0.6f, 0.8f);

            // Create Trigger zone at the door
            GameObject triggerObj = new GameObject("EntranceTrigger");
            triggerObj.transform.SetParent(house.transform, false);
            triggerObj.transform.localPosition = new Vector3(0f, -0.2f, -0.7f);
            triggerObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            var boxCol = triggerObj.AddComponent<BoxCollider>();
            boxCol.isTrigger = true;

            triggerObj.AddComponent<HouseTrigger>();

            Selection.activeGameObject = house;
            Undo.RegisterCreatedObjectUndo(house, "Create Test House");

            Debug.Log("🎉 [Jois] Домик Урока 1 успешно создан перед игроком!");
        }
    }
}
#endif
