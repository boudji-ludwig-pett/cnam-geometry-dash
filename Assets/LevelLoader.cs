using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.Json;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    public Text levelNameText;

    void Start()
    {
        levelNameText.text = "Coucou";
        string filePath = "level.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Level level = JsonSerializer.Deserialize<Level>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine($"Name: {level.Name}");
            Console.WriteLine($"MusicPath: {level.MusicPath}");
            Console.WriteLine($"TotalJumps: {level.TotalJumps}");
            Console.WriteLine($"TotalAttempts: {level.TotalAttempts}");
            Console.WriteLine($"KilledCount: {level.KilledCount}");
        }
        else
        {
            Console.WriteLine("JSON file not found.");
        }

    }

    void Update()
    {

    }
}
