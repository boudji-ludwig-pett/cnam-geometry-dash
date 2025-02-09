using UnityEngine;

[System.Serializable]
public class Level
{
    public string name;
    public string musicName;
    public int totalJumps;
    public int totalAttempts;
    public int order;

    public static Level CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Level>(jsonString);
    }
}
