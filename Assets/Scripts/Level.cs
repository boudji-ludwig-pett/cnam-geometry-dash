using UnityEngine;

[System.Serializable]
public class Level
{
    public string JsonName { get; set; }
    public int TotalJumps { get; set; }
    public int TotalAttempts { get; set; }

    public string name;
    public string musicName;
    public int order;

    public static Level CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Level>(jsonString);
    }
}
