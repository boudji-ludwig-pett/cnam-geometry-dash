using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class LevelsLoader : MonoBehaviour
{
    public Level level;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);

        //  var jsonTextFile = Resources.Load<TextAsset>("Text/jsonFile01");
        //Then use JsonUtility.FromJson<T>() to deserialize jsonTextFile into an object

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
