using System.IO;
using UnityEngine;

public class LevelsLoader : MonoBehaviour
{
    public Level level;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);

        TextAsset jsonTextFile = Resources.Load<TextAsset>(Path.Combine("Levels", "BackOnTrack"));
        level = JsonUtility.FromJson<Level>(jsonTextFile.text);
    }
}
