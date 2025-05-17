using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public LevelLoader levelLoader;
    public AudioSource sfxSource;
    public Slider volumeSlider;

    public void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            levelLoader.musicSource.volume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = levelLoader.musicSource.volume;
        }
        else
        {
            levelLoader.musicSource.volume = 1f;
            volumeSlider.value = 1f;
        }
    }

    public void ChangeVolume()
    {
        levelLoader.musicSource.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", levelLoader.musicSource.volume);
    }

    public void Pause()
    {
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();

        Time.timeScale = 0;
        levelLoader.musicSource.Pause();

        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void Home()
    {
        Time.timeScale = 1;
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();

        StartCoroutine(LevelHomeButton.PlaySoundAndLoadScene(sfxSource, "HomeScene"));
    }

    public void Resume()
    {
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();

        Time.timeScale = 1;
        levelLoader.musicSource.Play();

        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }
}
