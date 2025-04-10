using UnityEngine;
using System.Collections.Generic;
using System;

public class AchievementSystem : MonoBehaviour
{
    public static AchievementSystem Instance { get; private set; }

    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public Sprite icon;
        public bool isUnlocked;
        public int progressCurrent;
        public int progressRequired;
        public Action<Achievement> onUnlock;

        public float Progress => (float)progressCurrent / progressRequired;
    }

    [Header("Achievement Definitions")]
    [SerializeField] private List<Achievement> achievements = new List<Achievement>
    {
        new Achievement 
        { 
            id = "FIRST_FLIGHT",
            title = "First Flight",
            description = "Play your first game",
            progressRequired = 1
        },
        new Achievement 
        { 
            id = "SCORE_10",
            title = "Getting Started",
            description = "Score 10 points in a single game",
            progressRequired = 10
        },
        new Achievement 
        { 
            id = "SCORE_50",
            title = "High Flyer",
            description = "Score 50 points in a single game",
            progressRequired = 50
        },
        new Achievement 
        { 
            id = "TOTAL_GAMES_100",
            title = "Dedicated Player",
            description = "Play 100 games",
            progressRequired = 100
        },
        new Achievement 
        { 
            id = "TOTAL_SCORE_1000",
            title = "Score Master",
            description = "Accumulate a total score of 1000",
            progressRequired = 1000
        }
    };

    private Dictionary<string, Achievement> achievementDict = new Dictionary<string, Achievement>();

    public event System.Action<Achievement> OnAchievementUnlocked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievementDict[achievement.id] = achievement;
            
            // Load saved progress
            achievement.isUnlocked = PlayerPrefs.GetInt($"Achievement_{achievement.id}_Unlocked", 0) == 1;
            achievement.progressCurrent = PlayerPrefs.GetInt($"Achievement_{achievement.id}_Progress", 0);
        }
    }

    public void UpdateProgress(string achievementId, int progress)
    {
        if (achievementDict.TryGetValue(achievementId, out Achievement achievement))
        {
            if (achievement.isUnlocked) return;

            achievement.progressCurrent = Mathf.Min(achievement.progressCurrent + progress, achievement.progressRequired);
            
            // Save progress
            PlayerPrefs.SetInt($"Achievement_{achievement.id}_Progress", achievement.progressCurrent);
            PlayerPrefs.Save();

            CheckAchievement(achievement);
        }
    }

    public void SetProgress(string achievementId, int progress)
    {
        if (achievementDict.TryGetValue(achievementId, out Achievement achievement))
        {
            if (achievement.isUnlocked) return;

            achievement.progressCurrent = Mathf.Min(progress, achievement.progressRequired);
            
            // Save progress
            PlayerPrefs.SetInt($"Achievement_{achievement.id}_Progress", achievement.progressCurrent);
            PlayerPrefs.Save();

            CheckAchievement(achievement);
        }
    }

    private void CheckAchievement(Achievement achievement)
    {
        if (!achievement.isUnlocked && achievement.progressCurrent >= achievement.progressRequired)
        {
            UnlockAchievement(achievement);
        }
    }

    private void UnlockAchievement(Achievement achievement)
    {
        achievement.isUnlocked = true;
        PlayerPrefs.SetInt($"Achievement_{achievement.id}_Unlocked", 1);
        PlayerPrefs.Save();

        // Show notification
        UIManager.Instance?.ShowAchievementNotification(achievement);
        
        // Play sound effect
        AudioManager.Instance?.PlaySound("Achievement");

        // Trigger achievement-specific callback if any
        achievement.onUnlock?.Invoke(achievement);

        // Trigger global event
        OnAchievementUnlocked?.Invoke(achievement);
    }

    public void CheckGameplayAchievements(int currentScore, int totalGames, int totalScore)
    {
        // First Flight
        UpdateProgress("FIRST_FLIGHT", 1);

        // Score based achievements
        if (currentScore >= 10)
        {
            UpdateProgress("SCORE_10", 1);
        }
        if (currentScore >= 50)
        {
            UpdateProgress("SCORE_50", 1);
        }

        // Total games achievement
        SetProgress("TOTAL_GAMES_100", totalGames);

        // Total score achievement
        SetProgress("TOTAL_SCORE_1000", totalScore);
    }

    public List<Achievement> GetAllAchievements()
    {
        return new List<Achievement>(achievements);
    }

    public Achievement GetAchievement(string id)
    {
        return achievementDict.TryGetValue(id, out Achievement achievement) ? achievement : null;
    }

    public void ResetAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.isUnlocked = false;
            achievement.progressCurrent = 0;
            
            PlayerPrefs.DeleteKey($"Achievement_{achievement.id}_Unlocked");
            PlayerPrefs.DeleteKey($"Achievement_{achievement.id}_Progress");
        }
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        // Ensure all progress is saved
        PlayerPrefs.Save();
    }
}
