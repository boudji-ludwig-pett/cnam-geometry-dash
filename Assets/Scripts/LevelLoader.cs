using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public LevelsLoader levelsLoader;
    public AudioSource audioSource;

    void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        levelsLoader.IncreaseTotalAttempts();

        audioSource.clip = Resources.Load<AudioClip>(Path.Combine("Musics", levelsLoader.levelCurrent.musicName));
        audioSource.Play();

        Level current = levelsLoader.levelCurrent;
        // Debug.Log("Level: " + current.name);
        // for (int i = 0; i < current.elements.Count; i++)
        // {
        //     LevelElement element = current.elements[i];
        //     Debug.Log("Element: " + element.type + " " + element.x + " " + element.y);
        // }
    }

    void Update()
    {

    }
}
