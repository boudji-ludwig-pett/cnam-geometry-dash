using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public LevelLoader levelLoader;
    public Slider volumeSlider;

    public void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            levelLoader.audioSource.volume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = levelLoader.audioSource.volume;
        }
        else
        {
            levelLoader.audioSource.volume = 1f;
            volumeSlider.value = 1f;
        }
    }

    public void ChangeVolume()
    {
        levelLoader.audioSource.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", levelLoader.audioSource.volume);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        levelLoader.audioSource.Pause();

        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("HomeScene");
    }

    public void Resume()
    {
        Time.timeScale = 1;
        levelLoader.audioSource.Play();

        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }
}
