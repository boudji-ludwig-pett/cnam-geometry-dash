using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class LevelsLoader : MonoBehaviour
{
    public List<Level> levels = new();
    public Level levelCurrent;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllLevels();
        levelCurrent = levels[0];
    }

    private void LoadAllLevels()
    {
        TextAsset[] levelFiles = Resources.LoadAll<TextAsset>("Levels");
        TextAsset[] levelStatsFiles = Resources.LoadAll<TextAsset>("LevelsStats");

        Dictionary<string, LevelStat> levelStatsMap = new();
        foreach (TextAsset jsonTextFileStats in levelStatsFiles)
        {
            LevelStat levelStat = LevelStat.CreateFromJSON(jsonTextFileStats.text);
            levelStat.JsonName = jsonTextFileStats.name;
            levelStatsMap[levelStat.JsonName] = levelStat;
        }

        foreach (TextAsset jsonTextFile in levelFiles)
        {
            Level level = Level.CreateFromJSON(jsonTextFile.text);
            level.JsonName = jsonTextFile.name;
            level.TotalAttempts = 0;
            level.TotalJumps = 0;
            level.ProgressionPercent = 0;
            level.ProgressionPercentMax = 0;

            if (levelStatsMap.TryGetValue(level.JsonName, out LevelStat levelStat))
            {
                level.TotalAttempts = levelStat.totalAttempts;
                level.TotalJumps = levelStat.totalJumps;
                level.ProgressionPercentMax = levelStat.progressionPercent;
            }
            else
            {
                levelStat = new LevelStat { JsonName = level.JsonName, totalJumps = 0, totalAttempts = 0 };
                levelStatsMap[level.JsonName] = levelStat;
            }

            levels.Add(level);
        }
        levels.Sort((x, y) => x.order.CompareTo(y.order));
    }

    private void SaveLevelCurrent()
    {
        string levelJson = JsonUtility.ToJson(levelCurrent, true) + "\n";
        File.WriteAllText(Path.Combine(Application.dataPath, "Resources", "Levels", levelCurrent.JsonName + ".json"), levelJson);

        LevelStat levelStat = new()
        {
            JsonName = levelCurrent.JsonName,
            totalJumps = levelCurrent.TotalJumps,
            totalAttempts = levelCurrent.TotalAttempts,
            progressionPercent = levelCurrent.ProgressionPercentMax,
        };
        string levelStatJson = JsonUtility.ToJson(levelStat, true) + "\n";
        File.WriteAllText(Path.Combine(Application.dataPath, "Resources", "LevelsStats", levelCurrent.JsonName + ".json"), levelStatJson);
    }

    public void NextLevel()
    {
        int currentIndex = levels.IndexOf(levelCurrent);
        levelCurrent = levels[(currentIndex + 1) % levels.Count];
    }

    public void PreviousLevel()
    {
        int currentIndex = levels.IndexOf(levelCurrent);
        levelCurrent = levels[(currentIndex - 1 + levels.Count) % levels.Count];
    }

    public void IncreaseTotalJumps()
    {
        levelCurrent.TotalJumps += 1;
        SaveLevelCurrent();
    }

    public void IncreaseTotalAttempts()
    {
        levelCurrent.TotalAttempts += 1;
        SaveLevelCurrent();
    }

    public int CalculateCurrentProgressionPercent(Vector3 playerPosition)
    {
        float lastX = levelCurrent.LastX;
        GameObject winnerWallPrefab = Resources.Load<GameObject>("Prefabs/WinnerWall");
        float winnerWallWidth = winnerWallPrefab.GetComponent<Renderer>().bounds.size.x;
        float marginError = 0.5f;
        float totalDistance = lastX - (winnerWallWidth / 2) - marginError;

        float rawPercentage = (playerPosition.x / totalDistance) * 100;
        int clampedPercentage = Mathf.Clamp(Mathf.RoundToInt(rawPercentage), 0, 100);

        levelCurrent.ProgressionPercent = clampedPercentage;
        levelCurrent.ProgressionPercentMax = Math.Max(levelCurrent.ProgressionPercentMax, levelCurrent.ProgressionPercent);
        SaveLevelCurrent();

        return clampedPercentage;
    }

    public void RefreshLevels()
    {
        levels.Clear();
        LoadAllLevels();
        if (levels.Count > 0)
            levelCurrent = levels[0];
    }
}
