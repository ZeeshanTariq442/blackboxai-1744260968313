using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [System.Serializable]
    public class GameData
    {
        public int highScore;
        public float musicVolume;
        public float sfxVolume;
        public int difficulty;
        public bool isMuted;
        public string playerName;
        
        // Statistics
        public int totalGamesPlayed;
        public int totalScore;
        public float bestTime;
        public int totalPipesPassed;
    }

    private GameData gameData;
    private string saveFilePath;
    private const string fileName = "flappybird.save";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSaveSystem()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, fileName);
        LoadGame();
    }

    public void SaveGame()
    {
        try
        {
            gameData = new GameData
            {
                highScore = ScoreManager.Instance.GetHighScore(),
                musicVolume = SettingsManager.Instance.GetMusicVolume(),
                sfxVolume = SettingsManager.Instance.GetSFXVolume(),
                difficulty = (int)SettingsManager.Instance.GetCurrentDifficulty(),
                isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1,
                playerName = PlayerPrefs.GetString("PlayerName", "Player"),
                totalGamesPlayed = PlayerPrefs.GetInt("TotalGamesPlayed", 0),
                totalScore = PlayerPrefs.GetInt("TotalScore", 0),
                bestTime = PlayerPrefs.GetFloat("BestTime", 0f),
                totalPipesPassed = PlayerPrefs.GetInt("TotalPipesPassed", 0)
            };

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(saveFilePath, FileMode.Create))
            {
                formatter.Serialize(stream, gameData);
            }
            Debug.Log("Game saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(saveFilePath, FileMode.Open))
                {
                    gameData = (GameData)formatter.Deserialize(stream);
                }

                // Apply loaded data
                ApplyLoadedData();
                Debug.Log("Game loaded successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading game: {e.Message}");
                CreateNewSaveData();
            }
        }
        else
        {
            CreateNewSaveData();
        }
    }

    private void CreateNewSaveData()
    {
        gameData = new GameData
        {
            highScore = 0,
            musicVolume = 0.7f,
            sfxVolume = 1f,
            difficulty = (int)SettingsManager.Difficulty.Medium,
            isMuted = false,
            playerName = "Player",
            totalGamesPlayed = 0,
            totalScore = 0,
            bestTime = 0f,
            totalPipesPassed = 0
        };
        SaveGame();
    }

    private void ApplyLoadedData()
    {
        if (gameData != null)
        {
            // Apply settings
            SettingsManager.Instance.SetMusicVolume(gameData.musicVolume);
            SettingsManager.Instance.SetSFXVolume(gameData.sfxVolume);
            SettingsManager.Instance.SetDifficulty((SettingsManager.Difficulty)gameData.difficulty);

            // Set PlayerPrefs
            PlayerPrefs.SetInt("IsMuted", gameData.isMuted ? 1 : 0);
            PlayerPrefs.SetString("PlayerName", gameData.playerName);
            PlayerPrefs.SetInt("TotalGamesPlayed", gameData.totalGamesPlayed);
            PlayerPrefs.SetInt("TotalScore", gameData.totalScore);
            PlayerPrefs.SetFloat("BestTime", gameData.bestTime);
            PlayerPrefs.SetInt("TotalPipesPassed", gameData.totalPipesPassed);
            PlayerPrefs.Save();
        }
    }

    public void UpdateStatistics(int score, float time, int pipesPassed)
    {
        if (gameData != null)
        {
            gameData.totalGamesPlayed++;
            gameData.totalScore += score;
            if (time > gameData.bestTime)
            {
                gameData.bestTime = time;
            }
            gameData.totalPipesPassed += pipesPassed;
            SaveGame();
        }
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    public void ResetSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
        CreateNewSaveData();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }
}
