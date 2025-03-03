using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public LevelsLoader levelsLoader;
    public AudioSource audioSource;
    public GameObject obstaclePrefab;
    public GameObject spikePrefab;

    void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        levelsLoader.IncreaseTotalAttempts();

        audioSource.clip = Resources.Load<AudioClip>(Path.Combine("Musics", levelsLoader.levelCurrent.musicName));
        audioSource.Play();

        obstaclePrefab = Resources.Load<GameObject>("Prefabs/Obstacle");
        spikePrefab = Resources.Load<GameObject>("Prefabs/Spike");

        Level current = levelsLoader.levelCurrent;
        // Debug.Log("Level: " + current.name);
        // for (int i = 0; i < current.elements.Count; i++)
        // {
        //     LevelElement element = current.elements[i];
        //     Debug.Log("Element: " + element.type + " " + element.x + " " + element.y);
        // }

        for (int index = 0; index < current.elements.Count; index++)
        {
            LevelElement element = current.elements[index];
            GameObject prefab = obstaclePrefab;

            if (element.type == LevelElement.Type.Spike)
            {
                prefab = spikePrefab;
            }

            Instantiate(prefab, new Vector3(element.x, element.y, 0), Quaternion.identity);
        }

        // // Obstacle
        // // x=-6.684, y=-2.897, 0
        // // scale=0.96055, 0.2326, 1
        // Instantiate(obstaclePrefab, new Vector3(-6.684f, -2.897f, 0), Quaternion.identity);

        // // Spike
        // // -3.06, -2.93
        // // scale=0.15, 0.15, 1
        // Instantiate(spikePrefab, new Vector3(-3.06f, -2.93f, 0), Quaternion.identity);
    }

    void Update()
    {

    }
}
