using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[System.Serializable]
public class LevelElement
{
    public string type;
    public float x;
    public float y;
    public float scaleX = -1;
    public float scaleY = -1;
    public float rotationZ = 0;
}

[System.Serializable]
public class Level
{
    public static readonly int LAST_X = 25;
    public string JsonName { get; set; }
    public int TotalJumps { get; set; }
    public int TotalAttempts { get; set; }
    public int ProgressionPercent { get; set; }
    public int ProgressionPercentMax { get; set; }

    public string name;
    public string musicName;
    public int order;
    public int difficulty;

    public List<LevelElement> elements;

    public float LastX
    {
        get
        {
            float maxX = LAST_X;
            foreach (var element in elements)
            {
                if (element.x > maxX)
                {
                    maxX = element.x;
                }
            }
            return maxX + LAST_X;
        }
    }

    public static Level CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Level>(jsonString);
    }
}
