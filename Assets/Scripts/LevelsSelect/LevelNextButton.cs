using System.IO;
using UnityEngine;

public class LevelNextButton : MonoBehaviour
{
    public AudioSource sfxSource;
    public LevelsLoader levelsLoader;

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextLevel();
        }
    }

    public void NextLevel()
    {
        levelsLoader.NextLevel();
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();
    }
}
