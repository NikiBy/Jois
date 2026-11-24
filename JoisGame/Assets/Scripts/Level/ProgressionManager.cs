using UnityEngine;
using System;

namespace Jois.Level
{
    public class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance { get; private set; }

        private const string PROGRESS_KEY = "Jois_UnlockedLevel";

        [Header("Progression Status")]
        [SerializeField] private int currentUnlockedLevel = 1;
        [SerializeField] private int maxLevels = 10;

        public int CurrentUnlockedLevel => currentUnlockedLevel;
        public int MaxLevels => maxLevels;

        public event Action<int> OnLevelUnlocked;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            LoadProgress();
        }

        public bool IsLevelUnlocked(int levelNumber)
        {
            return levelNumber <= currentUnlockedLevel;
        }

        public void UnlockNextLevel(int completedLevel)
        {
            if (completedLevel >= currentUnlockedLevel && currentUnlockedLevel < maxLevels)
            {
                currentUnlockedLevel = completedLevel + 1;
                SaveProgress();
                OnLevelUnlocked?.Invoke(currentUnlockedLevel);
                Debug.Log($"[ProgressionManager] Level {completedLevel} completed! Unlocked Level {currentUnlockedLevel}");
            }
            else
            {
                OnLevelUnlocked?.Invoke(currentUnlockedLevel);
            }
        }

        public void SaveProgress()
        {
            PlayerPrefs.SetInt(PROGRESS_KEY, currentUnlockedLevel);
            PlayerPrefs.Save();
        }

        public void LoadProgress()
        {
            currentUnlockedLevel = PlayerPrefs.GetInt(PROGRESS_KEY, 1);
            if (currentUnlockedLevel < 1) currentUnlockedLevel = 1;
            if (currentUnlockedLevel > maxLevels) currentUnlockedLevel = maxLevels;
        }

        public void ResetProgress()
        {
            currentUnlockedLevel = 1;
            SaveProgress();
            OnLevelUnlocked?.Invoke(currentUnlockedLevel);
            Debug.Log("[ProgressionManager] Progress has been reset to Level 1.");
        }
    }
}
