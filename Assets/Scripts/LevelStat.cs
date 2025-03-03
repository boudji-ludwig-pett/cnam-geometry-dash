using UnityEngine;

[System.Serializable]
public class LevelStat
{
    public string JsonName { get; set; }

    public int totalJumps;
    public int totalAttempts;

    public static LevelStat CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<LevelStat>(jsonString);
    }
}
