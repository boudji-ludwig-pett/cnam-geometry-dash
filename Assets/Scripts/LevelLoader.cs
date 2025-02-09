using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Text levelNameText;
    public Level level;

    void Start()
    {
        LoadLevel();
        if (level != null)
        {
            levelNameText.text = level.Name;
        }
        else
        {
            levelNameText.text = "Failed to Load Level";
        }
    }

    void LoadLevel()
    {
        string path = Path.Combine(Application.dataPath, "Levels", "back-on-track.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Level));
                level = (Level)serializer.ReadObject(stream);
            }
        }
        else
        {
            Debug.LogError("Level file not found: " + path);
        }
    }
}
