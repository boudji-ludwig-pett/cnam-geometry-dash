using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelElement
{
    public enum Type
    {
        Spike,
        Obstacle
    }

    public Type type;
    public float x;
    public float y;
}

[System.Serializable]
public class Level
{
    public string JsonName { get; set; }
    public int TotalJumps { get; set; }
    public int TotalAttempts { get; set; }

    public string name;
    public string musicName;
    public int order;

    public List<LevelElement> elements;

    public static Level CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Level>(jsonString);
    }
}
