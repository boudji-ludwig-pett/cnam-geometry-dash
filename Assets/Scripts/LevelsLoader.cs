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

        foreach (TextAsset jsonTextFile in levelFiles)
        {
            Level level = Level.CreateFromJSON(jsonTextFile.text);
            level.JsonName = jsonTextFile.name;

            string statPath = Path.Combine(Application.persistentDataPath, level.JsonName + ".json");
            if (File.Exists(statPath))
            {
                string statJson = File.ReadAllText(statPath);
                LevelStat levelStat = JsonUtility.FromJson<LevelStat>(statJson);

                level.TotalAttempts = levelStat.totalAttempts;
                level.TotalJumps = levelStat.totalJumps;
                level.ProgressionPercentMax = levelStat.progressionPercent;
            }
            else
            {
                level.TotalAttempts = 0;
                level.TotalJumps = 0;
                level.ProgressionPercentMax = 0;
            }

            level.ProgressionPercent = 0;
            levels.Add(level);
        }

        levels.Sort((x, y) => x.order.CompareTo(y.order));
    }

    private void SaveLevelCurrent()
    {
        if (levelCurrent == null) return;

        LevelStat levelStat = new()
        {
            JsonName = levelCurrent.JsonName,
            totalJumps = levelCurrent.TotalJumps,
            totalAttempts = levelCurrent.TotalAttempts,
            progressionPercent = levelCurrent.ProgressionPercentMax,
        };

        string levelStatJson = JsonUtility.ToJson(levelStat, true) + "\n";

        string savePath = Path.Combine(Application.persistentDataPath, levelCurrent.JsonName + ".json");
        File.WriteAllText(savePath, levelStatJson);
    }

    public void NextLevel()
    {
        if (levels.Count == 0) return;

        int currentIndex = levels.IndexOf(levelCurrent);
        levelCurrent = levels[(currentIndex + 1) % levels.Count];
    }

    public void PreviousLevel()
    {
        if (levels.Count == 0) return;

        int currentIndex = levels.IndexOf(levelCurrent);
        levelCurrent = levels[(currentIndex - 1 + levels.Count) % levels.Count];
    }

    public void IncreaseTotalJumps()
    {
        if (levelCurrent == null) return;

        levelCurrent.TotalJumps += 1;
        SaveLevelCurrent();
    }

    public void IncreaseTotalAttempts()
    {
        if (levelCurrent == null) return;

        levelCurrent.TotalAttempts += 1;
        SaveLevelCurrent();
    }

    public int CalculateCurrentProgressionPercent(Vector3 playerPosition)
    {
        if (levelCurrent == null) return 0;

        float lastX = levelCurrent.LastX;
        GameObject winnerWallPrefab = Resources.Load<GameObject>("Prefabs/WinnerWall");
        float winnerWallWidth = winnerWallPrefab != null
            ? winnerWallPrefab.GetComponent<Renderer>().bounds.size.x
            : 0f;
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
