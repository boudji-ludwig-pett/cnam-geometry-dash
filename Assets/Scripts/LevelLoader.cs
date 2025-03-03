using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public LevelsLoader levelsLoader;
    public AudioSource audioSource;
    public GameObject obstaclePrefab;
    public GameObject spikePrefab;

    private GameObject GetPrefab(LevelElement.Type type)
    {
        if (type == LevelElement.Type.Spike)
        {
            return spikePrefab;
        }
        return obstaclePrefab;
    }

    private void LoadAudio()
    {
        audioSource.clip = Resources.Load<AudioClip>(Path.Combine("Musics", levelsLoader.levelCurrent.musicName));
        audioSource.Play();
    }

    private void LoadElements()
    {
        obstaclePrefab = Resources.Load<GameObject>("Prefabs/Obstacle");
        spikePrefab = Resources.Load<GameObject>("Prefabs/Spike");
        Level current = levelsLoader.levelCurrent;
        foreach (var element in current.elements)
        {
            GameObject prefab = GetPrefab(element.type);
            Instantiate(prefab, new Vector3(element.x, element.y, 0), Quaternion.identity);
        }
    }

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        levelsLoader.IncreaseTotalAttempts();

        LoadAudio();
        LoadElements();
    }

    public void Update()
    {

    }
}
