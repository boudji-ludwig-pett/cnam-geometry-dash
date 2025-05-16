using System.IO;
using UnityEngine;

public class LevelPreviousButton : MonoBehaviour
{
    public AudioSource sfxSource;
    public LevelsLoader levelsLoader;

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousLevel();
        }
    }

    public void PreviousLevel()
    {
        levelsLoader.PreviousLevel();
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();
    }
}
