using UnityEngine;
using System.Collections.Generic;
using System.IO;

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
            levels.Add(level);
        }
        levels.Sort((x, y) => x.order.CompareTo(y.order));
    }

    private void SaveLevelCurrent()
    {
        string json = JsonUtility.ToJson(levelCurrent, true) + "\n";
        File.WriteAllText(Path.Combine(Application.dataPath, "Resources", "Levels", levelCurrent.JsonName + ".json"), json);
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
        levelCurrent.totalJumps += 1;
        SaveLevelCurrent();
    }

    public void IncreaseTotalAttempts()
    {
        levelCurrent.totalAttempts += 1;
        SaveLevelCurrent();
    }
}
